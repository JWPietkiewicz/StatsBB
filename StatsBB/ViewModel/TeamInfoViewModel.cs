using System;
using System.Collections.ObjectModel;
using StatsBB.Domain;
using StatsBB.Model;
using StatsBB.MVVM;
using Microsoft.Win32;
using System.IO;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace StatsBB.ViewModel;

public class TeamInfoViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;
    public Game Game { get; } = new();
    public ObservableCollection<TeamColorOption> ColorOptions { get; } = new();
    
    // Flag to prevent circular updates during captain selection
    private bool _updatingCaptain = false;

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
            OnPropertyChanged(nameof(CanConfirmHomeTeam));
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
            OnPropertyChanged(nameof(CanConfirmAwayTeam));
            OnPropertyChanged(nameof(AreTeamsConfirmed));
        }
    }

    public bool AreTeamsConfirmed => HomeTeamConfirmed && AwayTeamConfirmed;
    public bool HomeColorEnabled => !HomeTeamConfirmed;
    public bool AwayColorEnabled => !AwayTeamConfirmed;
    
    // Computed properties for team confirmation enablement
    public bool CanConfirmHomeTeam => !HomeTeamConfirmed && TeamAColorOption != null;
    public bool CanConfirmAwayTeam => !AwayTeamConfirmed && TeamBColorOption != null;
    
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
        
        // Subscribe to player property changes to handle captain selection
        SubscribeToPlayerChanges();

        LoadHomeTeamCommand = new RelayCommand(_ => LoadTeam(Game.HomeTeam, true));
        SaveHomeTeamCommand = new RelayCommand(_ => SaveTeam(Game.HomeTeam));
        LoadAwayTeamCommand = new RelayCommand(_ => LoadTeam(Game.AwayTeam, false));
        SaveAwayTeamCommand = new RelayCommand(_ => SaveTeam(Game.AwayTeam));
        ConfirmHomeTeamCommand = new RelayCommand(_ =>
        {
            if (TeamAColorOption == null)
            {
                System.Windows.MessageBox.Show("Please select a team color before confirming.", "Color Required", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                return;
            }
            HomeTeamConfirmed = true;
            _main.RegenerateTeamsFromInfo();
        });
        ConfirmAwayTeamCommand = new RelayCommand(_ =>
        {
            if (TeamBColorOption == null)
            {
                System.Windows.MessageBox.Show("Please select a team color before confirming.", "Color Required", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                return;
            }
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
            {
                OnPropertyChanged(nameof(TeamAColorOption));
                OnPropertyChanged(nameof(CanConfirmHomeTeam));
            }
            if (e.PropertyName == nameof(MainWindowViewModel.TeamBColorOption))
            {
                OnPropertyChanged(nameof(TeamBColorOption));
                OnPropertyChanged(nameof(CanConfirmAwayTeam));
            }
        };
    }

    private static void EnsurePlayers(Team team)
    {
        while (team.Players.Count < 16)
        {
            var player = new Player()
            {
                IsPlaying = false,  // Set as inactive by default
                Number = 0          // No number assigned by default
            };
            player.ParentTeam = team;
            team.Players.Add(player);
        }
        
        // Ensure all existing players have their parent team set
        foreach (var player in team.Players)
        {
            player.ParentTeam = team;
            // Only assign numbers and validate for active players or players with names
            bool hasContent = !string.IsNullOrWhiteSpace(player.FirstName) || !string.IsNullOrWhiteSpace(player.LastName);
            
            if (player.IsPlaying || hasContent)
            {
                if (player.Number <= 0)
                {
                    player.Number = team.GetNextAvailableNumber();
                }
                else
                {
                    team.EnsureUniquePlayerNumber(player);
                }
            }
        }
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
    
    public Player? HomeCaptain
    {
        get => Game.HomeTeam.Captain;
        set
        {
            Game.HomeTeam.Captain = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanConfirmHomeTeam));
        }
    }
    
    public Player? AwayCaptain
    {
        get => Game.AwayTeam.Captain;
        set
        {
            Game.AwayTeam.Captain = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanConfirmAwayTeam));
        }
    }

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
        
        // Notify property changed for the sorted collection
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
    
    private void SubscribeToPlayerChanges()
    {
        // Subscribe to changes for all existing players
        foreach (var player in Game.HomeTeam.Players)
        {
            player.PropertyChanged += OnPlayerPropertyChanged;
        }
        foreach (var player in Game.AwayTeam.Players)
        {
            player.PropertyChanged += OnPlayerPropertyChanged;
        }
        
        // Subscribe to collection changes to handle new players
        Game.HomeTeam.Players.CollectionChanged += (s, e) =>
        {
            if (e.NewItems != null)
            {
                foreach (Player player in e.NewItems)
                {
                    player.PropertyChanged += OnPlayerPropertyChanged;
                }
            }
        };
        
        Game.AwayTeam.Players.CollectionChanged += (s, e) =>
        {
            if (e.NewItems != null)
            {
                foreach (Player player in e.NewItems)
                {
                    player.PropertyChanged += OnPlayerPropertyChanged;
                }
            }
        };
    }
    
    private void OnPlayerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Player.IsCaptain) && sender is Player player && !_updatingCaptain)
        {
            if (player.IsCaptain)
            {
                _updatingCaptain = true;
                try
                {
                    // When a player becomes captain, set them as the team captain
                    // The Team.Captain setter will handle deselecting other captains
                    if (player.ParentTeam == Game.HomeTeam)
                    {
                        Game.HomeTeam.Captain = player;
                        OnPropertyChanged(nameof(HomeCaptain));
                        OnPropertyChanged(nameof(CanConfirmHomeTeam));
                    }
                    else if (player.ParentTeam == Game.AwayTeam)
                    {
                        Game.AwayTeam.Captain = player;
                        OnPropertyChanged(nameof(AwayCaptain));
                        OnPropertyChanged(nameof(CanConfirmAwayTeam));
                    }
                }
                finally
                {
                    _updatingCaptain = false;
                }
            }
        }
    }
    
    #region Input Validation Methods
    
    /// <summary>
    /// Validates player name input
    /// </summary>
    /// <param name="name">Name to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    public static bool IsValidPlayerName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;
            
        // Check length (2-50 characters)
        if (name.Trim().Length < 2 || name.Trim().Length > 50)
            return false;
            
        // Allow letters, spaces, hyphens, apostrophes, and periods
        var namePattern = @"^[a-zA-Z\s\-'.]+$";
        return Regex.IsMatch(name.Trim(), namePattern);
    }
    
    /// <summary>
    /// Validates player jersey number
    /// </summary>
    /// <param name="number">Number to validate</param>
    /// <param name="team">Team to check for duplicates</param>
    /// <param name="excludePlayer">Player to exclude from duplicate check</param>
    /// <returns>Validation result with message</returns>
    public static (bool IsValid, string Message) ValidatePlayerNumber(int number, Team team, Player? excludePlayer = null)
    {
        // Check range (0-99 for basketball)
        if (number < 0 || number > 99)
            return (false, "Jersey number must be between 0 and 99.");
        
        // Check for duplicates
        var duplicatePlayer = team.Players.FirstOrDefault(p => p.Number == number && p != excludePlayer);
        if (duplicatePlayer != null)
        {
            var playerName = !string.IsNullOrWhiteSpace(duplicatePlayer.FirstName + duplicatePlayer.LastName) 
                ? $"{duplicatePlayer.FirstName} {duplicatePlayer.LastName}".Trim()
                : "Another player";
            return (false, $"Jersey number {number} is already used by {playerName}.");
        }
        
        return (true, string.Empty);
    }
    
    /// <summary>
    /// Validates team name input
    /// </summary>
    /// <param name="teamName">Team name to validate</param>
    /// <returns>Validation result with message</returns>
    public static (bool IsValid, string Message) ValidateTeamName(string? teamName)
    {
        if (string.IsNullOrWhiteSpace(teamName))
            return (false, "Team name is required.");
            
        var trimmed = teamName.Trim();
        
        if (trimmed.Length < 2)
            return (false, "Team name must be at least 2 characters long.");
            
        if (trimmed.Length > 50)
            return (false, "Team name cannot exceed 50 characters.");
            
        // Allow letters, numbers, spaces, and common punctuation
        var teamNamePattern = @"^[a-zA-Z0-9\s\-'.&]+$";
        if (!Regex.IsMatch(trimmed, teamNamePattern))
            return (false, "Team name contains invalid characters. Only letters, numbers, spaces, hyphens, apostrophes, periods, and ampersands are allowed.");
            
        return (true, string.Empty);
    }
    
    /// <summary>
    /// Validates that a team has the minimum required active players
    /// </summary>
    /// <param name="team">Team to validate</param>
    /// <returns>Validation result with message</returns>
    public static (bool IsValid, string Message) ValidateTeamComposition(Team team)
    {
        var activePlayers = team.Players.Where(p => p.IsPlaying).ToList();
        
        if (activePlayers.Count < 5)
            return (false, $"Team must have at least 5 active players. Currently has {activePlayers.Count}.");
            
        // Check that all active players have valid names and numbers
        var invalidPlayers = activePlayers.Where(p => 
            string.IsNullOrWhiteSpace(p.FirstName) && string.IsNullOrWhiteSpace(p.LastName) ||
            p.Number <= 0).ToList();
            
        if (invalidPlayers.Any())
            return (false, "All active players must have valid names and jersey numbers.");
            
        // Check for captain
        if (team.Captain == null)
            return (false, "Team must have a designated captain.");
            
        return (true, string.Empty);
    }
    
    /// <summary>
    /// Shows a user-friendly validation error message
    /// </summary>
    /// <param name="message">Error message to display</param>
    /// <param name="title">Dialog title</param>
    public static void ShowValidationError(string message, string title = "Validation Error")
    {
        System.Windows.MessageBox.Show(
            message,
            title,
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Warning);
    }
    
    #endregion
}
