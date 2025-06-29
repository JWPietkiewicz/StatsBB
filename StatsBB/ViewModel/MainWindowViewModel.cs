using StatsBB.Model;
using StatsBB.MVVM;
using StatsBB.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace StatsBB.ViewModel;

public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<Player> Players { get; set; } = new();
    public ObservableCollection<PlayerPositionViewModel> TeamAPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> TeamBPlayers { get; } = new();

    private readonly ResourceDictionary _resources;

    public ICommand SelectActionCommand { get; }

    // Action button styles
    public Style MadeButtonStyle => GetActionStyle("MADE");
    public Style MissedButtonStyle => GetActionStyle("MISSED");
    public Style FoulButtonStyle => GetActionStyle("FOUL");
    public Style TurnoverButtonStyle => GetActionStyle("TURNOVER");

    private Style GetActionStyle(string action)
    {
        if (!IsActionSelectionActive)
            return (Style)_resources["ActionDisabledButtonStyle"];

        if (SelectedAction == action)
        {
            return action switch
            {
                "MADE" => (Style)_resources["ActionSelectedMadeStyle"],
                "MISS" or "MISSED" or "FOUL" or "TURNOVER" => (Style)_resources["ActionSelectedFailedStyle"],
                _ => (Style)_resources["ActionSelectableButtonStyle"],
            };
        }

        return (Style)_resources["ActionSelectableButtonStyle"];
    }

    public MainWindowViewModel(ResourceDictionary resources)
    {
        _resources = resources;

        SelectActionCommand = new RelayCommand(
            param => SelectAction(param?.ToString()),
            param => IsActionSelectionActive // Only allow command if a point is selected
        );

        Players.CollectionChanged += Players_CollectionChanged;

        PlayerLayoutService.PopulateTeams(Players);
        RegenerateTeams();
    }

    private void SelectAction(string? action)
    {
        if (string.IsNullOrWhiteSpace(action))
            return;

        SelectedAction = action;
    }

    private void Players_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        RegenerateTeams();
    }

    private void RegenerateTeams()
    {
        TeamAPlayers.Clear();
        TeamBPlayers.Clear();

        var teamA = PlayerLayoutService.CreatePositionedPlayers(Players.Where(p => p.IsTeamA), _resources, OnPlayerSelected);
        var teamB = PlayerLayoutService.CreatePositionedPlayers(Players.Where(p => !p.IsTeamA), _resources, OnPlayerSelected);

        foreach (var pvm in teamA)
            TeamAPlayers.Add(pvm);

        foreach (var pvm in teamB)
            TeamBPlayers.Add(pvm);
    }

    private void OnPlayerSelected(Player player)
    {
        if (!IsPlayerSelectionActive)
            return;
        var playerStatus = player.IsActive ? "ON COURT" : "ON BENCH";
        Debug.WriteLine($"Action '{SelectedAction}' by {player.Number}.{player.Name} - {playerStatus} at point {SelectedPoint}");

        // TODO: Save event/stat here

        // Reset
        SelectedAction = null;
        SelectedPoint = null;
    }

    private CourtPointData? _selectedPoint;
    public CourtPointData? SelectedPoint
    {
        get => _selectedPoint;
        set
        {
            _selectedPoint = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsPlayerSelectionActive));
            OnPropertyChanged(nameof(IsActionSelectionActive));
            UpdatePlayerStyles();
            UpdateActionButtonStyles();
            CommandManager.InvalidateRequerySuggested(); // Refresh command can-execute
        }
    }

    private string? _selectedAction;
    public string? SelectedAction
    {
        get => _selectedAction;
        set
        {
            _selectedAction = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsPlayerSelectionActive));
            UpdatePlayerStyles();
            UpdateActionButtonStyles();
        }
    }

    public bool IsPlayerSelectionActive => SelectedPoint != null && !string.IsNullOrEmpty(SelectedAction);
    public bool IsActionSelectionActive => SelectedPoint != null;

    private void UpdatePlayerStyles()
    {
        foreach (var playerVm in TeamAPlayers.Concat(TeamBPlayers))
        {
            playerVm.UpdateButtonStyle(IsActionSelectionActive, IsPlayerSelectionActive, GetTeamColor(playerVm));
        }
    }

    private void UpdateActionButtonStyles()
    {
        OnPropertyChanged(nameof(MadeButtonStyle));
        OnPropertyChanged(nameof(MissedButtonStyle));
        OnPropertyChanged(nameof(FoulButtonStyle));
        OnPropertyChanged(nameof(TurnoverButtonStyle));
    }

    private Brush GetTeamColor(PlayerPositionViewModel vm)
    {
        return vm.Player.IsTeamA
            ? (Brush)_resources["CourtAColor"]
            : (Brush)_resources["CourtBColor"];
    }
}
