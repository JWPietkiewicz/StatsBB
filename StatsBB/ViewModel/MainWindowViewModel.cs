using StatsBB.Model;
using StatsBB.MVVM;
using StatsBB.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace StatsBB.ViewModel;

public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<Player> Players { get; set; } = new();
    public ObservableCollection<PlayerPositionViewModel> TeamAPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> TeamBPlayers { get; } = new();
    public ObservableCollection<FreeThrowResult> FreeThrowResultRows { get; } = new();
    public ObservableCollection<Player> TeamASubIn { get; } = new();
    public ObservableCollection<Player> TeamASubOut { get; } = new();
    public ObservableCollection<Player> TeamBSubIn { get; } = new();
    public ObservableCollection<Player> TeamBSubOut { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleCourtAssistPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleBenchAssistPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleTeamACourtReboundPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleTeamABenchReboundPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleTeamBCourtReboundPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleTeamBBenchReboundPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleBlockCourtPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleBlockBenchPlayers { get; } = new();

    public ObservableCollection<Player> TeamACourtPlayers =>
        new(Players.Where(p => p.IsTeamA && p.IsActive));
    public ObservableCollection<Player> TeamBCourtPlayers =>
        new(Players.Where(p => !p.IsTeamA && p.IsActive));
    public ObservableCollection<Player> TeamABenchPlayers =>
        new(Players.Where(p => p.IsTeamA && !p.IsActive));
    public ObservableCollection<Player> TeamBBenchPlayers =>
        new(Players.Where(p => !p.IsTeamA && !p.IsActive));

    private bool _isSubstitutionPanelVisible;
    public bool IsSubstitutionPanelVisible
    {
        get => _isSubstitutionPanelVisible;
        set
        {
            _isSubstitutionPanelVisible = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SubstitutionPanelVisibility));
        }
    }
    public Visibility SubstitutionPanelVisibility =>
        IsSubstitutionPanelVisible ? Visibility.Visible : Visibility.Collapsed;

    private bool _isTimeOutSelectionActive;
    public bool IsTimeOutSelectionActive
    {
        get => _isTimeOutSelectionActive;
        set
        {
            _isTimeOutSelectionActive = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TimeOutPanelVisibility));
        }
    }
    public Visibility TimeOutPanelVisibility =>
        IsTimeOutSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    private readonly ResourceDictionary _resources;

    public ICommand SelectActionCommand { get; }
    public ICommand NoAssistCommand { get; }
    public ICommand ReboundTeamACommand { get; }
    public ICommand ReboundTeamBCommand { get; }
    public ICommand BlockCommand { get; }
    public ICommand ShotClockCommand { get; }
    public ICommand TurnoverTeamACommand { get; }
    public ICommand TurnoverTeamBCommand { get; }
    public ICommand NoStealCommand { get; }
    public ICommand SelectFoulTypeCommand { get; }
    public ICommand SelectFreeThrowCountCommand { get; }
    public ICommand StartSubstitutionCommand { get; }
    public ICommand ConfirmSubstitutionCommand { get; }
    public ICommand ToggleSubInCommand { get; }
    public ICommand ToggleSubOutCommand { get; }
    public ICommand StartTimeoutCommand { get; }
    public ICommand TimeoutTeamACommand { get; }
    public ICommand TimeoutTeamBCommand { get; }


    public event Action<Point, Brush, bool>? MarkerRequested;
    public event Action? TempMarkerRemoved;

    public MainWindowViewModel(ResourceDictionary resources)
    {
        _resources = resources;
        StartSubstitutionCommand = new RelayCommand(_ => BeginSubstitution());

        SelectActionCommand = new RelayCommand(
            param => SelectAction(param?.ToString()),
            param => IsActionSelectionActive
        );

        NoAssistCommand = new RelayCommand(
            _ => CompleteAssistSelection(null),
            _ => IsAssistSelectionActive
        );
        NoStealCommand = new RelayCommand(
            _ => CompleteStealSelection(null),
            _ => IsStealSelectionActive
        );

        SelectFoulTypeCommand = new RelayCommand(
            param => OnFoulTypeSelected(param?.ToString())
        );

        SelectFreeThrowCountCommand = new RelayCommand(
            param => OnFreeThrowSelected(Convert.ToInt32(param))
        );

        ReboundTeamACommand = new RelayCommand(_ => CompleteReboundSelection("TeamA"), _ => IsReboundSelectionActive);
        ReboundTeamBCommand = new RelayCommand(_ => CompleteReboundSelection("TeamB"), _ => IsReboundSelectionActive);
        BlockCommand = new RelayCommand(
    _ => EnterBlockerSelection(),
    _ => IsReboundSelectionActive
);
        ShotClockCommand = new RelayCommand(_ => CompleteReboundSelection("24"), _ => IsReboundSelectionActive);
        TurnoverTeamACommand = new RelayCommand(_ => CompleteTurnoverSelection("TeamA"), _ => IsTurnoverSelectionActive);
        TurnoverTeamBCommand = new RelayCommand(_ => CompleteTurnoverSelection("TeamB"), _ => IsTurnoverSelectionActive);

        StartSubstitutionCommand = new RelayCommand(_ => BeginSubstitution());
        ConfirmSubstitutionCommand = new RelayCommand(_ => ConfirmSubstitution(), _ => IsSubstitutionConfirmEnabled);
        ToggleSubInCommand = new RelayCommand(p => ToggleSubIn(p as Player));
        ToggleSubOutCommand = new RelayCommand(p => ToggleSubOut(p as Player));
        StartTimeoutCommand = new RelayCommand(_ => BeginTimeout());
        TimeoutTeamACommand = new RelayCommand(_ => CompleteTimeoutSelection("Team A"), _ => IsTimeOutSelectionActive);
        TimeoutTeamBCommand = new RelayCommand(_ => CompleteTimeoutSelection("Team B"), _ => IsTimeOutSelectionActive);


        Players.CollectionChanged += Players_CollectionChanged;

        PlayerLayoutService.PopulateTeams(Players);
        RegenerateTeams();
    }
    private void BeginSubstitution()
    {
        TeamASubIn.Clear();
        TeamASubOut.Clear();
        TeamBSubIn.Clear();
        TeamBSubOut.Clear();

        IsSubstitutionPanelVisible = true;
    }

    private void BeginTimeout()
    {
        IsTimeOutSelectionActive = true;
    }

    private void CompleteTimeoutSelection(string team)
    {
        Debug.WriteLine($"Timeout called by {team}");
        IsTimeOutSelectionActive = false;
    }

    private void SelectAction(string? action)
    {
        if (string.IsNullOrWhiteSpace(action))
            return;
        if (action.Equals("FOUL", StringComparison.InvariantCultureIgnoreCase))
        {
            IsFoulCommiterSelectionActive = true;
        }

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

    private Player? _pendingShooter;
    private Player? _foulCommiter;
    private Player? _fouledPlayer;
    private string? _foulType;

    private void OnPlayerSelected(Player player)
    {
        if (IsFoulCommiterSelectionActive)
        {
            _foulCommiter = player;
            IsFoulCommiterSelectionActive = false;
            IsFoulTypeSelectionActive = true;
            return;
        }

        if (IsFouledPlayerSelectionActive)
        {
            if (_foulCommiter != null && player.IsTeamA == _foulCommiter.IsTeamA)
            {
                Debug.WriteLine("Fouled player must be on the opposing team.");
                return;
            }

            _fouledPlayer = player;
            IsFouledPlayerSelectionActive = false;

            if (_foulType?.ToLowerInvariant() == "offensive")
            {
                Debug.WriteLine($"Offensive foul by {_foulCommiter?.Number}.{_foulCommiter?.Name} on {_fouledPlayer?.Number}.{_fouledPlayer?.Name} — no free throws");
                ResetFoulState();
            }
            else
            {
                IsFreeThrowSelectionActive = true;
            }

            return;
        }

        if (IsAssistSelectionActive)
        {
            CompleteAssistSelection(player);
            return;
        }

        if (IsReboundSelectionActive)
        {
            CompleteReboundSelection(player);
            return;
        }

        if (IsBlockerSelectionActive)
        {
            CompleteBlockSelection(player);
            return;
        }

        if (IsTurnoverSelectionActive)
        {
            CompleteTurnoverSelection(player);
            return;
        }
        if (IsStealSelectionActive)
        {
            CompleteStealSelection(player);
            return;
        }

        if (!IsPlayerSelectionActive || SelectedPoint == null || SelectedAction == null)
            return;

        var actionType = GetActionType(SelectedAction);
        var position = SelectedPoint.Point;

        TempMarkerRemoved?.Invoke();

        if (actionType == ActionType.Other)
        {
            MarkerRequested?.Invoke(position, Brushes.Transparent, false);
        }
        else
        {
            Brush teamColor = GetTeamColorFromPlayer(player);
            bool isFilled = actionType == ActionType.Made;

            MarkerRequested?.Invoke(position, teamColor, isFilled);
        }

        Debug.WriteLine($"Action '{SelectedAction}' by {player.Number}.{player.Name} at {position} ({actionType})");

        _pendingShooter = player;

        if (actionType == ActionType.Made)
        {
            IsAssistSelectionActive = true;
        }
        else if (actionType == ActionType.Missed)
        {
            IsReboundSelectionActive = true;
        }
        else if (actionType == ActionType.Turnover)
        {
            IsTurnoverSelectionActive = true;
        }
        else
        {
            ResetSelectionState();
        }
    }

    private void OnFoulTypeSelected(string? foulType)
    {
        if (string.IsNullOrWhiteSpace(foulType))
            return;

        _foulType = foulType;
        IsFoulTypeSelectionActive = false;

        var lowerType = foulType.ToLowerInvariant();

        if (lowerType == "technical")
        {
            IsFreeThrowSelectionActive = true;
        }
        else
        {
            IsFouledPlayerSelectionActive = true;
        }
    }

    private void ResetFoulState()
    {
        _foulCommiter = null;
        _fouledPlayer = null;
        _foulType = null;

        IsFoulCommiterSelectionActive = false;
        IsFoulTypeSelectionActive = false;
        IsFouledPlayerSelectionActive = false;
        IsFreeThrowSelectionActive = false;
        FreeThrowResultRows.Clear();
        IsFreeThrowResultSelectionActive = false;
        ResetSelectionState();
    }

    private void UpdateFoulCommiterPlayerStyles()
    {
        foreach (var vm in TeamAPlayers.Concat(TeamBPlayers))
        {
            vm.SetFoulCommiterSelectionMode(IsFoulCommiterSelectionActive);
        }
    }

    private void UpdateFouledPlayerStyles()
    {
        foreach (var vm in TeamAPlayers.Concat(TeamBPlayers))
        {
            bool isSelectable = false;

            if (IsFouledPlayerSelectionActive && _foulCommiter != null)
            {
                isSelectable = vm.Player.IsTeamA != _foulCommiter.IsTeamA;
            }

            vm.SetFouledPlayerSelectionMode(isSelectable);
        }
    }



    private void ResetSelectionState()
    {
        SelectedAction = null;
        SelectedPoint = null;
        _pendingShooter = null;
        IsAssistSelectionActive = false;
        IsReboundSelectionActive = false;
        IsTurnoverSelectionActive = false;
        IsStealSelectionActive = false;
    }



    private void CompleteAssistSelection(Player? assistPlayer)
    {
        if (_pendingShooter != null)
        {
            if (assistPlayer?.Number == _pendingShooter.Number)
            {
                // Invalid: same player attempted to get an assist
                Debug.WriteLine("Assist not awarded — shooter cannot assist their own shot.");
            }
            else
            {
                var assist = assistPlayer != null
                ? $"Assist by {assistPlayer.Number}.{assistPlayer.Name}"
                : "No assist";
            Debug.WriteLine($"{assist}");
            }
        }

        _pendingShooter = null;
        SelectedAction = null;
        SelectedPoint = null;
        IsAssistSelectionActive = false;
    }

    private void CompleteReboundSelection(object? reboundSource)
    {
        if (_pendingShooter != null)
        {
            string log = reboundSource switch
            {
                Player p => $"Rebound by {p.Number}.{p.Name}",
                "TeamA" => "Team A rebound",
                "TeamB" => "Team B rebound",
                "Block" => "Shot was blocked",
                "24" => "24-second violation",
                _ => "Unknown rebound result"
            };

            Debug.WriteLine($"{log} after miss by {_pendingShooter.Number}.{_pendingShooter.Name}");
        }

        ResetSelectionState();
    }

    private void CompleteBlockSelection(Player blocker)
    {
        if (_pendingShooter != null && blocker != null)
        {
            Debug.WriteLine($"Block by {blocker.Number}.{blocker.Name} on {_pendingShooter.Number}.{_pendingShooter.Name}");
        }

        // Reset block selection state
        IsBlockerSelectionActive = false;

        // Go to rebound selection next
        IsReboundSelectionActive = true;
        UpdateReboundPlayerStyles(); // if needed to populate rebound UI
    }

    private void UpdateBlockerPlayerStyles()
    {
        EligibleBlockCourtPlayers.Clear();
        EligibleBlockBenchPlayers.Clear();

        if (_pendingShooter == null)
            return;

        foreach (var vm in TeamAPlayers.Concat(TeamBPlayers))
        {
            var isEligible = IsBlockerSelectionActive && vm.Player.IsTeamA != _pendingShooter.IsTeamA;

            vm.SetBlockerSelectionMode(isEligible);

            if (isEligible)
            {
                if (vm.Player.IsActive)
                    EligibleBlockCourtPlayers.Add(vm);
                else
                    EligibleBlockBenchPlayers.Add(vm);
            }
        }

        OnPropertyChanged(nameof(EligibleBlockCourtPlayers));
        OnPropertyChanged(nameof(EligibleBlockBenchPlayers));
    }


    private void EnterBlockerSelection()
    {
        IsAssistSelectionActive = false;
        IsReboundSelectionActive = false;

        IsBlockerSelectionActive = true;

        UpdateBlockerPlayerStyles();
    }


    private void CompleteTurnoverSelection(object? source)
    {
        if (source is Player p)
        {
            Debug.WriteLine($"Turnover by {p.Number}.{p.Name}");
            _pendingShooter = p;
            IsTurnoverSelectionActive = false;
            IsStealSelectionActive = true; // move to steal selection
        }
        else if (source is string team)
        {
            Debug.WriteLine($"Team turnover by {team}");
            ResetSelectionState();
        }
    }
    private void UpdateTurnoverPlayerStyles()
    {
        foreach (var vm in TeamAPlayers.Concat(TeamBPlayers))
        {
            vm.SetTurnoverSelectionMode(IsTurnoverSelectionActive);
        }
    }

    private void CompleteStealSelection(Player? stealer)
    {
        if (_pendingShooter == null)
            return;

        if (stealer == null)
        {
            Debug.WriteLine($"No steal awarded on turnover by {_pendingShooter.Number}.{_pendingShooter.Name}");
            ResetSelectionState();
            return;
        }

        // If selected player is on same team, ignore the input and stay in steal mode
        if (stealer.IsTeamA == _pendingShooter.IsTeamA)
        {
            Debug.WriteLine($"Invalid steal selection: {stealer.Number}.{stealer.Name} is on same team as turnover. Waiting for valid selection.");
            return;
        }

        // Valid steal
        Debug.WriteLine($"Steal by {stealer.Number}.{stealer.Name} from {_pendingShooter.Number}.{_pendingShooter.Name}");
        ResetSelectionState();
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
        "TURNOVER" => ActionType.Turnover,
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

    private bool _isAssistSelectionActive;
    public bool IsAssistSelectionActive
    {
        get => _isAssistSelectionActive;
        set
        {
            _isAssistSelectionActive = value;
            OnPropertyChanged();
            UpdateAssistPlayerStyles();
            OnPropertyChanged(nameof(NoAssistButtonVisibility));
        }
    }

    private void UpdateAssistPlayerStyles()
    {
        EligibleCourtAssistPlayers.Clear();
        EligibleBenchAssistPlayers.Clear();

        foreach (var vm in TeamAPlayers.Concat(TeamBPlayers))
        {
            bool isSelectable = false;

            if (IsAssistSelectionActive && _pendingShooter != null)
            {
                isSelectable = vm.Player.IsTeamA == _pendingShooter.IsTeamA &&
                               vm.Player.Number != _pendingShooter.Number;
            }

            vm.SetAssistSelectionMode(isSelectable);

            if (isSelectable)
            {
                if (vm.Player.IsActive)
                    EligibleCourtAssistPlayers.Add(vm);
                else
                    EligibleBenchAssistPlayers.Add(vm);
            }
        }

        OnPropertyChanged(nameof(EligibleCourtAssistPlayers));
        OnPropertyChanged(nameof(EligibleBenchAssistPlayers));
    }


    private bool _isReboundSelectionActive;
    public bool IsReboundSelectionActive
    {
        get => _isReboundSelectionActive;
        set
        {
            _isReboundSelectionActive = value;
            OnPropertyChanged();
            UpdateReboundPlayerStyles();
            OnPropertyChanged(nameof(ReboundPanelVisibility));
        }
    }
    private void UpdateReboundPlayerStyles()
    {
        EligibleTeamACourtReboundPlayers.Clear();
        EligibleTeamABenchReboundPlayers.Clear();
        EligibleTeamBCourtReboundPlayers.Clear();
        EligibleTeamBBenchReboundPlayers.Clear();

        foreach (var vm in TeamAPlayers.Concat(TeamBPlayers))
        {
            bool isSelectable = IsReboundSelectionActive;
            vm.SetReboundSelectionMode(isSelectable);

            if (!isSelectable) continue;

            if (vm.Player.IsTeamA)
            {
                if (vm.Player.IsActive)
                    EligibleTeamACourtReboundPlayers.Add(vm);
                else
                    EligibleTeamABenchReboundPlayers.Add(vm);
            }
            else
            {
                if (vm.Player.IsActive)
                    EligibleTeamBCourtReboundPlayers.Add(vm);
                else
                    EligibleTeamBBenchReboundPlayers.Add(vm);
            }
        }

        OnPropertyChanged(nameof(EligibleTeamACourtReboundPlayers));
        OnPropertyChanged(nameof(EligibleTeamABenchReboundPlayers));
        OnPropertyChanged(nameof(EligibleTeamBCourtReboundPlayers));
        OnPropertyChanged(nameof(EligibleTeamBBenchReboundPlayers));
    }

    private void UpdateBlockPlayerStyles()
    {
        EligibleBlockCourtPlayers.Clear();
        EligibleBlockBenchPlayers.Clear();


    }

    public Visibility ReboundPanelVisibility =>
        IsReboundSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    private bool _isBlockerSelectionActive;
    public bool IsBlockerSelectionActive
    {
        get => _isBlockerSelectionActive;
        set
        {
            _isBlockerSelectionActive = value;
            OnPropertyChanged();
            UpdateBlockerPlayerStyles();
            OnPropertyChanged(nameof(BlockerSelectionVisibility));
        }
    }

    public Visibility BlockerSelectionVisibility =>
        IsBlockerSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    private bool _isTurnoverSelectionActive;
    public bool IsTurnoverSelectionActive
    {
        get => _isTurnoverSelectionActive;
        set
        {
            _isTurnoverSelectionActive = value;
            OnPropertyChanged();
            UpdateTurnoverPlayerStyles();
            OnPropertyChanged(nameof(TurnoverPanelVisibility));
        }
    }

    public Visibility TurnoverPanelVisibility =>
        IsTurnoverSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    private bool _isStealSelectionActive;
    public bool IsStealSelectionActive
    {
        get => _isStealSelectionActive;
        set
        {
            _isStealSelectionActive = value;
            OnPropertyChanged();
            UpdateStealPlayerStyles();
            OnPropertyChanged(nameof(StealPanelVisibility));
        }
    }

    public Visibility StealPanelVisibility =>
        IsStealSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    private void UpdateStealPlayerStyles()
    {
        foreach (var vm in TeamAPlayers.Concat(TeamBPlayers))
        {
            bool isSelectable = false;

            if (IsStealSelectionActive && _pendingShooter != null)
            {
                isSelectable = vm.Player.IsTeamA != _pendingShooter.IsTeamA;
            }

            vm.SetStealSelectionMode(isSelectable);
        }
    }

    private bool _isFoulCommiterSelectionActive;
    public bool IsFoulCommiterSelectionActive
    {
        get => _isFoulCommiterSelectionActive;
        set
        {
            _isFoulCommiterSelectionActive = value;
            OnPropertyChanged();
            UpdateFoulCommiterPlayerStyles();
        }
    }

    private bool _isFoulTypeSelectionActive;
    public bool IsFoulTypeSelectionActive
    {
        get => _isFoulTypeSelectionActive;
        set
        {
            _isFoulTypeSelectionActive = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FoulTypePanelVisibility));
        }
    }

    private bool _isFouledPlayerSelectionActive;
    public bool IsFouledPlayerSelectionActive
    {
        get => _isFouledPlayerSelectionActive;
        set
        {
            _isFouledPlayerSelectionActive = value;
            OnPropertyChanged();
            UpdateFouledPlayerStyles();
        }
    }

    private bool _isFreeThrowSelectionActive;
    public bool IsFreeThrowSelectionActive
    {
        get => _isFreeThrowSelectionActive;
        set
        {
            _isFreeThrowSelectionActive = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FreeThrowPanelVisibility));
        }
    }

    public Visibility FoulTypePanelVisibility =>
    IsFoulTypeSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    public Visibility FreeThrowPanelVisibility =>
        IsFreeThrowSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    public int FreeThrowCount { get; private set; } = 0;

    private bool _isFreeThrowResultSelectionActive;
    public bool IsFreeThrowResultSelectionActive
    {
        get => _isFreeThrowResultSelectionActive;
        set
        {
            _isFreeThrowResultSelectionActive = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FreeThrowResultPanelVisibility));
            OnPropertyChanged(nameof(FreeThrowResultRows));
        }
    }

    public Visibility FreeThrowResultPanelVisibility =>
    IsFreeThrowResultSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    private void OnFreeThrowSelected(int count)
    {
        IsFreeThrowSelectionActive = false;

        if (count <= 0)
        {
            ResetFoulState();
            return;
        }

        FreeThrowResultRows.Clear();

        for (int i = 0; i < count; i++)
        {
            var item = new FreeThrowResult(i, count, OnFreeThrowResultSelected);
            FreeThrowResultRows.Add(item);
        }

        IsFreeThrowResultSelectionActive = true;
    }

    private void OnFreeThrowResultSelected(int index, string result)
    {
        var item = FreeThrowResultRows.ElementAtOrDefault(index);
        if (item == null) return;

        result = result.ToUpperInvariant();
        if (result != "MADE" && result != "MISSED") return;

        item.Result = result;

        if (FreeThrowResultRows.All(r => r.Result != null))
        {
            foreach (var r in FreeThrowResultRows)
            {
                Debug.WriteLine($"{r.Label} {r.Result}");
            }

            ResetFoulState();
        }
    }

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

    public Visibility NoAssistButtonVisibility =>
        IsAssistSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    private void ToggleSubIn(Player? player)
    {
        if (player == null) return;
        var list = player.IsTeamA ? TeamASubIn : TeamBSubIn;

        if (list.Contains(player))
            list.Remove(player);
        else if (list.Count < 5)
            list.Add(player);

        OnPropertyChanged(nameof(IsSubstitutionConfirmEnabled));
    }

    private void ToggleSubOut(Player? player)
    {
        if (player == null) return;
        var list = player.IsTeamA ? TeamASubOut : TeamBSubOut;

        if (list.Contains(player))
            list.Remove(player);
        else if (list.Count < 5)
            list.Add(player);

        OnPropertyChanged(nameof(IsSubstitutionConfirmEnabled));
    }


    public bool IsSubstitutionConfirmEnabled =>
    TeamASubIn.Count == TeamASubOut.Count && TeamASubIn.Count <= 5 &&
    TeamBSubIn.Count == TeamBSubOut.Count && TeamBSubIn.Count <= 5;

    private void ConfirmSubstitution()
    {
        ApplySubstitution(TeamASubIn, TeamASubOut);
        ApplySubstitution(TeamBSubIn, TeamBSubOut);

        RegenerateTeams();
        IsSubstitutionPanelVisible = false;
    }

    private void ApplySubstitution(IEnumerable<Player> subIn, IEnumerable<Player> subOut)
    {
        foreach (var p in subOut)
            p.IsActive = false;

        foreach (var p in subIn)
            p.IsActive = true;
    }




}


public class FreeThrowResult : ViewModelBase
{
    public int Index { get; } // 0-based
    public int Total { get; }

    private string? _result;
    public string? Result
    {
        get => _result;
        set
        {
            _result = value;
            OnPropertyChanged(nameof(Result));
            OnPropertyChanged(nameof(MadeButtonBackground));
            OnPropertyChanged(nameof(MissedButtonBackground));
        }
    }

    public string Label => $"FT{Index + 1}of{Total}";

    public string MadeButtonBackground =>
        Result == "MADE" ? "Green" :
        Result == "MISSED" ? "LightGray" : "White";

    public string MissedButtonBackground =>
        Result == "MISSED" ? "Red" :
        Result == "MADE" ? "LightGray" : "White";

    public ICommand SelectResultCommand { get; }

    public FreeThrowResult(int index, int total, Action<int, string> onResultSelected)
    {
        Index = index;
        Total = total;
        SelectResultCommand = new RelayCommand(
            param => onResultSelected(Index, param?.ToString() ?? ""));
    }
}