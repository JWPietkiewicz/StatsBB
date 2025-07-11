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
