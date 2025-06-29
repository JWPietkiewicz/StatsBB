using StatsBB.Model;
using StatsBB.MVVM;
using StatsBB.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
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

    public event Action<Point, Brush, bool>? MarkerRequested;
    public event Action? TempMarkerRemoved;

    public MainWindowViewModel(ResourceDictionary resources)
    {
        _resources = resources;

        SelectActionCommand = new RelayCommand(
            param => SelectAction(param?.ToString()),
            param => IsActionSelectionActive
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

        var teamA = PlayerLayoutService.CreatePositionedPlayers(
            Players.Where(p => p.IsTeamA),
            _resources,
            OnPlayerSelected
        );
        var teamB = PlayerLayoutService.CreatePositionedPlayers(
            Players.Where(p => !p.IsTeamA),
            _resources,
            OnPlayerSelected
        );

        foreach (var p in teamA) TeamAPlayers.Add(p);
        foreach (var p in teamB) TeamBPlayers.Add(p);
    }

    private void OnPlayerSelected(Player player)
    {
        if (!IsPlayerSelectionActive || SelectedPoint == null || SelectedAction == null)
            return;

        var actionType = GetActionType(SelectedAction);
        var position = SelectedPoint.Point;

        TempMarkerRemoved?.Invoke();

        if (actionType == ActionType.Other)
        {
            // Remove final marker
            MarkerRequested?.Invoke(position, Brushes.Transparent, false);
        }
        else
        {
            Brush teamColor = GetTeamColorFromPlayer(player);
            bool isFilled = actionType == ActionType.Made;

            MarkerRequested?.Invoke(position, teamColor, isFilled);
        }

        Debug.WriteLine($"Action '{SelectedAction}' by {player.Number}.{player.Name} at {position} ({actionType})");

        // Reset state
        SelectedAction = null;
        SelectedPoint = null;
    }


    private Brush GetTeamColorFromPlayer(Player player)
    {
        return player.IsTeamA
            ? (Brush)_resources["CourtAColor"]
            : (Brush)_resources["CourtBColor"];
    }

    private ActionType GetActionType(string action) => action.ToUpperInvariant() switch
    {
        "MADE" => ActionType.Made,
        "MISSED" => ActionType.Missed,
        _ => ActionType.Other
    };

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
            CommandManager.InvalidateRequerySuggested();
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
        foreach (var vm in TeamAPlayers.Concat(TeamBPlayers))
        {
            vm.UpdateButtonStyle(IsActionSelectionActive, IsPlayerSelectionActive, GetTeamColor(vm));
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
                "MISSED" or "FOUL" or "TURNOVER" => (Style)_resources["ActionSelectedFailedStyle"],
                _ => (Style)_resources["ActionSelectableButtonStyle"]
            };
        }

        return (Style)_resources["ActionSelectableButtonStyle"];
    }
}
