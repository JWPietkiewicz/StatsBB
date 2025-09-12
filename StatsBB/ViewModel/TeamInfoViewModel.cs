using System;
using System.Collections.ObjectModel;
using StatsBB.Domain;
using StatsBB.Model;
using StatsBB.MVVM;
using Microsoft.Win32;
using System.IO;

namespace StatsBB.ViewModel;

public class TeamInfoViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;
    public Game Game { get; } = new();
    public ObservableCollection<TeamColorOption> ColorOptions { get; } = new();

    public RelayCommand LoadHomeTeamCommand { get; }
    public RelayCommand SaveHomeTeamCommand { get; }
    public RelayCommand LoadAwayTeamCommand { get; }
    public RelayCommand SaveAwayTeamCommand { get; }
    public RelayCommand ConfirmHomeTeamCommand { get; }
    public RelayCommand ConfirmAwayTeamCommand { get; }
    public RelayCommand SelectTeamAColorCommand { get; }
    public RelayCommand SelectTeamBColorCommand { get; }

    private bool _homeTeamConfirmed;
    public bool HomeTeamConfirmed
    {
        get => _homeTeamConfirmed;
        set
        {
            if (_homeTeamConfirmed == value) return;
            _homeTeamConfirmed = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HomeColorEnabled));
            OnPropertyChanged(nameof(AreTeamsConfirmed));
        }
    }

    private bool _awayTeamConfirmed;
    public bool AwayTeamConfirmed
    {
        get => _awayTeamConfirmed;
        set
        {
            if (_awayTeamConfirmed == value) return;
            _awayTeamConfirmed = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(AwayColorEnabled));
            OnPropertyChanged(nameof(AreTeamsConfirmed));
        }
    }

    public bool AreTeamsConfirmed => HomeTeamConfirmed && AwayTeamConfirmed;
    public bool HomeColorEnabled => !HomeTeamConfirmed;
    public bool AwayColorEnabled => !AwayTeamConfirmed;
    
    // Expose color options from main view model
    public TeamColorOption? TeamAColorOption => _main.TeamAColorOption;
    public TeamColorOption? TeamBColorOption => _main.TeamBColorOption;
    public TeamInfoViewModel(MainWindowViewModel main)
    {
        _main = main;
        Game.HomeTeam = new Team { IsHomeTeam = true };
        Game.AwayTeam = new Team { IsHomeTeam = false };
        EnsurePlayers(Game.HomeTeam);
        EnsurePlayers(Game.AwayTeam);
        HomeTeamName = _main.TeamAName;
        AwayTeamName = _main.TeamBName;
        ColorOptions = _main.ColorOptions;

        LoadHomeTeamCommand = new RelayCommand(_ => LoadTeam(Game.HomeTeam, true));
        SaveHomeTeamCommand = new RelayCommand(_ => SaveTeam(Game.HomeTeam));
        LoadAwayTeamCommand = new RelayCommand(_ => LoadTeam(Game.AwayTeam, false));
        SaveAwayTeamCommand = new RelayCommand(_ => SaveTeam(Game.AwayTeam));
        ConfirmHomeTeamCommand = new RelayCommand(_ =>
        {
            HomeTeamConfirmed = true;
            _main.RegenerateTeamsFromInfo();
        });
        ConfirmAwayTeamCommand = new RelayCommand(_ =>
        {
            AwayTeamConfirmed = true;
            _main.RegenerateTeamsFromInfo();
        });
        
        // Forward color selection commands to main view model
        SelectTeamAColorCommand = new RelayCommand(color => _main.SelectTeamAColorCommand.Execute(color));
        SelectTeamBColorCommand = new RelayCommand(color => _main.SelectTeamBColorCommand.Execute(color));
        
        // Subscribe to main view model color changes to notify UI
        _main.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(MainWindowViewModel.TeamAColorOption))
                OnPropertyChanged(nameof(TeamAColorOption));
            if (e.PropertyName == nameof(MainWindowViewModel.TeamBColorOption))
                OnPropertyChanged(nameof(TeamBColorOption));
        };
    }

    private static void EnsurePlayers(Team team)
    {
        while (team.Players.Count < 16)
            team.Players.Add(new Player());
    }

    public string HomeTeamName
    {
        get => Game.HomeTeam.TeamName;
        set
        {
            Game.HomeTeam.TeamName = value;
            OnPropertyChanged();
            _main.TeamAName = value;
        }
    }

    public string AwayTeamName
    {
        get => Game.AwayTeam.TeamName;
        set
        {
            Game.AwayTeam.TeamName = value;
            OnPropertyChanged();
            _main.TeamBName = value;
        }
    }
    
    public ObservableCollection<Player> HomePlayers => Game.HomeTeam.GetPlayers();
    public ObservableCollection<Player> AwayPlayers => Game.AwayTeam.GetPlayers();

    private static void WriteTeam(string file, Team team)
    {
        using var sw = new StreamWriter(file);
        sw.WriteLine("#NR, First Name, Last Name");
        foreach (var p in team.Players)
        {
            sw.WriteLine($"{p.Number},{p.FirstName},{p.LastName}");
        }
    }

    private void SaveTeam(Team team)
    {
        var dlg = new SaveFileDialog { Filter = "CSV files (*.csv)|*.csv" };
        if (dlg.ShowDialog() == true)
        {
            WriteTeam(dlg.FileName, team);
        }
    }

    private void LoadTeam(Team team, bool home)
    {
        var dlg = new OpenFileDialog { Filter = "CSV files (*.csv)|*.csv" };
        if (dlg.ShowDialog() != true)
            return;

        var name = Path.GetFileNameWithoutExtension(dlg.FileName);
        team.TeamName = name;
        team.Players.Clear();
        foreach (var line in File.ReadLines(dlg.FileName))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;
            var parts = line.Split(',');
            if (parts.Length >= 1)
            {
                var player = new Player
                {
                    Number = int.TryParse(parts[0], out var n) ? n : 0,
                    FirstName = parts.Length > 1 ? parts[1].Trim() : string.Empty,
                    LastName = parts.Length > 2 ? parts[2].Trim() : string.Empty,
                    IsPlaying = true
                };
                team.Players.Add(player);
            }
        }
        EnsurePlayers(team);
        OnPropertyChanged(home ? nameof(HomePlayers) : nameof(AwayPlayers));
        if (home)
        {
            HomeTeamName = team.TeamName;
        }
        else
        {
            AwayTeamName = team.TeamName;
        }
    }
}
