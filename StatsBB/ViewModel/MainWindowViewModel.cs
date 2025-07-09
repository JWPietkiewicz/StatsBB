using StatsBB.Model;
using StatsBB.MVVM;
using StatsBB.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
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

    public ObservableCollection<PlayerPositionViewModel> EligibleTeamACourtTurnoverPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleTeamABenchTurnoverPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleTeamBCourtTurnoverPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleTeamBBenchTurnoverPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleStealCourtPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleStealBenchPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleTeamACourtShotPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleTeamABenchShotPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleTeamBCourtShotPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleTeamBBenchShotPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleTeamACourtFoulCommitterPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleTeamABenchFoulCommitterPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleTeamBCourtFoulCommitterPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleTeamBBenchFoulCommitterPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleFoulOnCourtPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleFoulOnBenchPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleFreeThrowCourtPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> EligibleFreeThrowBenchPlayers { get; } = new();

    public StatsTabViewModel StatsVM { get; }

    public ObservableCollection<PlayCardViewModel> PlayByPlayCards { get; } = new();

    public ObservableCollection<Player> TeamACourtPlayers =>
        new(Players.Where(p => p.IsTeamA && p.IsActive));
    public ObservableCollection<Player> TeamBCourtPlayers =>
        new(Players.Where(p => !p.IsTeamA && p.IsActive));
    public ObservableCollection<Player> TeamABenchPlayers =>
        new(Players.Where(p => p.IsTeamA && !p.IsActive));
    public ObservableCollection<Player> TeamBBenchPlayers =>
        new(Players.Where(p => !p.IsTeamA && !p.IsActive));

    private string _teamAName = "Team A";
    public string TeamAName
    {
        get => _teamAName;
        set
        {
            _teamAName = value;
            OnPropertyChanged();
        }
    }

    private string _teamBName = "Team B";
    public string TeamBName
    {
        get => _teamBName;
        set
        {
            _teamBName = value;
            OnPropertyChanged();
        }
    }

    private int _teamAScore;
    public int TeamAScore
    {
        get => _teamAScore;
        set
        {
            _teamAScore = value;
            OnPropertyChanged();
        }
    }

    private int _teamBScore;
    public int TeamBScore
    {
        get => _teamBScore;
        set
        {
            _teamBScore = value;
            OnPropertyChanged();
        }
    }

    private int _teamATimeOutsLeft = 3;
    public int TeamATimeOutsLeft
    {
        get => _teamATimeOutsLeft;
        set
        {
            _teamATimeOutsLeft = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TeamATimeoutsText));
        }
    }

    private int _teamATotalTimeouts = 3;
    public int TeamATotalTimeouts
    {
        get => _teamATotalTimeouts;
        set
        {
            _teamATotalTimeouts = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TeamATimeoutsText));
        }
    }

    public string TeamATimeoutsText => $"{TeamATimeOutsLeft}/{TeamATotalTimeouts}";

    private int _teamBTimeOutsLeft = 3;
    public int TeamBTimeOutsLeft
    {
        get => _teamBTimeOutsLeft;
        set
        {
            _teamBTimeOutsLeft = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TeamBTimeoutsText));
        }
    }

    private int _teamBTotalTimeouts = 3;
    public int TeamBTotalTimeouts
    {
        get => _teamBTotalTimeouts;
        set
        {
            _teamBTotalTimeouts = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TeamBTimeoutsText));
        }
    }

    public string TeamBTimeoutsText => $"{TeamBTimeOutsLeft}/{TeamBTotalTimeouts}";

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
    public ICommand StartTurnoverCommand { get; }
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
    public ICommand CoachTechnicalTeamACommand { get; }
    public ICommand CoachTechnicalTeamBCommand { get; }
    public ICommand BenchTechnicalTeamACommand { get; }
    public ICommand BenchTechnicalTeamBCommand { get; }
    public ICommand ConfirmFreeThrowsAwardedCommand { get; }
    public ICommand SelectFreeThrowShooterCommand { get; }
    public ICommand SelectFreeThrowAssistCommand { get; }
    public ICommand NoAssistFreeThrowCommand { get; }


    public event Action<Point, Brush, bool>? MarkerRequested;
    public event Action<Point>? TempMarkerRequested;
    public event Action? TempMarkerRemoved;

    public MainWindowViewModel(ResourceDictionary resources)
    {
        _resources = resources;
        StartSubstitutionCommand = new RelayCommand(_ => BeginSubstitution());

        SelectActionCommand = new RelayCommand(
            param => SelectAction(param?.ToString()),
            param => IsActionSelectionActive ||
                     (param?.ToString()?.Equals("Foul",
                         StringComparison.InvariantCultureIgnoreCase) ?? false)
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
            param => SelectedFreeThrowCount = Convert.ToInt32(param)
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
        StartTurnoverCommand = new RelayCommand(_ => BeginTurnover());

        CoachTechnicalTeamACommand = new RelayCommand(_ => OnCoachTechnical("Team A"));
        CoachTechnicalTeamBCommand = new RelayCommand(_ => OnCoachTechnical("Team B"));
        BenchTechnicalTeamACommand = new RelayCommand(_ => OnBenchTechnical("Team A"));
        BenchTechnicalTeamBCommand = new RelayCommand(_ => OnBenchTechnical("Team B"));
        ConfirmFreeThrowsAwardedCommand = new RelayCommand(_ => OnConfirmFreeThrowsAwarded());
        SelectFreeThrowShooterCommand = new RelayCommand(p => SelectFreeThrowShooter(p as Player));
        SelectFreeThrowAssistCommand = new RelayCommand(p => ToggleFreeThrowAssist(p as Player));
        NoAssistFreeThrowCommand = new RelayCommand(_ => SetNoFreeThrowAssist());


        Players.CollectionChanged += Players_CollectionChanged;

        PlayerLayoutService.PopulateTeams(Players);
        RegenerateTeams();
        StatsVM = new StatsTabViewModel(Players);

        //GenerateSamplePlayByPlayData();
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

    public void HandleCourtClick(CourtPointData data)
    {
        ResetSelectionState();
        SelectedPoint = data;

        if (data.MouseButton == MouseButton.Left)
        {
            SelectedAction = "MADE";
            IsQuickShotSelectionActive = true;
        }
        else if (data.MouseButton == MouseButton.Right)
        {
            SelectedAction = "MISSED";
            IsQuickShotSelectionActive = true;
        }
    }

    private void BeginTurnover()
    {
        ResetSelectionState();
        IsTurnoverSelectionActive = true;
    }

    private void CompleteTimeoutSelection(string team)
    {
        Debug.WriteLine($"{GameClockService.TimeLeftString} Timeout called by {team}");
        IsTimeOutSelectionActive = false;
        var isTeamA = team == "Team A";
        AddPlayCard(new[] { CreateTeamAction(isTeamA, "TIMEOUT") });
    }

    private void OnCoachTechnical(string team)
    {
        Debug.WriteLine($"Coach Technical on {team}");
        var isTeamA = team == "Team A";
        AddPlayCard(new[] { CreateTeamAction(isTeamA, "FOUL TECHNICAL") });
        _defaultFreeThrows = 1;
        _freeThrowTeamIsTeamA = team != "Team A";
        BeginFreeThrowsAwardedSelection();
    }

    private void OnBenchTechnical(string team)
    {
        Debug.WriteLine($"Bench Technical on {team}");
        var isTeamA = team == "Team A";
        AddPlayCard(new[] { CreateTeamAction(isTeamA, "FOUL TECHNICAL") });
        _defaultFreeThrows = 1;
        _freeThrowTeamIsTeamA = team != "Team A";
        BeginFreeThrowsAwardedSelection();
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
    private bool _pendingIsThreePoint;
    private bool _wasBlocked;
    private Player? _blocker;
    private Player? _foulCommiter;
    private Player? _fouledPlayer;
    private string? _foulType;
    private int _defaultFreeThrows;
    private bool _freeThrowTeamIsTeamA;
    private int _selectedFreeThrowCount;
    private Player? _selectedFreeThrowShooter;
    private Player? _selectedFreeThrowAssist;
    private readonly List<PlayActionViewModel> _currentPlayActions = new();

    private void OnPlayerSelected(Player player)
    {
        if (IsQuickShotSelectionActive)
        {
            IsQuickShotSelectionActive = false;
        }
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
                Debug.WriteLine($"{GameClockService.TimeLeftString} Fouled player must be on the opposing team.");
                return;
            }

            _fouledPlayer = player;
            IsFouledPlayerSelectionActive = false;

            if (_fouledPlayer != null)
                _currentPlayActions.Add(CreateAction(_fouledPlayer, "FOULED"));

            AddPlayCard(_currentPlayActions.ToList());
            _currentPlayActions.Clear();

            if (_foulType?.ToLowerInvariant() == "offensive")
            {
                Debug.WriteLine($"{GameClockService.TimeLeftString} Offensive foul by {_foulCommiter?.Number}.{_foulCommiter?.Name} on {_fouledPlayer?.Number}.{_fouledPlayer?.Name} — no free throws");
                ResetFoulState();
            }
            else
            {
                BeginFreeThrowsAwardedSelection();
            }

            return;
        }

        if (IsFreeThrowsAwardedSelectionActive)
        {
            SelectedFreeThrowShooter = player;
            return;
        }

        if (IsFreeThrowsSelectionActive)
        {
            _pendingShooter = player;
            UpdateAssistPlayerStyles();
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
        TempMarkerRequested?.Invoke(position);



        if (actionType == ActionType.Other)
        {
            MarkerRequested?.Invoke(position, Brushes.Transparent, false);
        }
        else
        {
            TempMarkerRemoved?.Invoke();
            Brush teamColor = GetTeamColorFromPlayer(player);
            bool isFilled = actionType == ActionType.Made;

            MarkerRequested?.Invoke(position, teamColor, isFilled);
        }

        Debug.WriteLine($"{GameClockService.TimeLeftString} Action '{SelectedAction}' by {player.Number}.{player.Name} at {position} ({actionType})");

        _pendingShooter = player;
        _pendingIsThreePoint = SelectedPoint.IsThreePoint;
        _wasBlocked = false;
        _blocker = null;
        _currentPlayActions.Clear();

        if (actionType == ActionType.Turnover)
        {
            _currentPlayActions.Add(CreateAction(player, "TURNOVER"));
            IsTurnoverSelectionActive = true;
        }
        else if (actionType == ActionType.Made)
        {
            IsAssistSelectionActive = true;
        }
        else if (actionType == ActionType.Missed)
        {
            IsReboundSelectionActive = true;
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

        _currentPlayActions.Clear();
        if (_foulCommiter != null)
            _currentPlayActions.Add(CreateAction(_foulCommiter, $"FOUL {foulType.ToUpperInvariant()}"));

        var lowerType = foulType.ToLowerInvariant();
        _defaultFreeThrows = 0;

        switch (lowerType)
        {
            case "personal":
                Debug.WriteLine($"Personal foul by {_foulCommiter?.Number}.{_foulCommiter?.Name}");
                IsFouledPlayerSelectionActive = true;
                break;
            case "shooting":
                Debug.WriteLine($"Shooting foul by {_foulCommiter?.Number}.{_foulCommiter?.Name}");
                _defaultFreeThrows = 2;
                IsFouledPlayerSelectionActive = true;
                break;
            case "offensive":
                Debug.WriteLine($"Offensive foul by {_foulCommiter?.Number}.{_foulCommiter?.Name}");
                IsFouledPlayerSelectionActive = true;
                break;
            case "double personal":
                Debug.WriteLine($"Double personal foul by {_foulCommiter?.Number}.{_foulCommiter?.Name}");
                IsFouledPlayerSelectionActive = true;
                break;
            case "unsportsmanlike":
                Debug.WriteLine($"Unsportsmanlike foul by {_foulCommiter?.Number}.{_foulCommiter?.Name}");
                _defaultFreeThrows = 2;
                IsFouledPlayerSelectionActive = true;
                break;
            case "unsportsmanlike turnover":
                Debug.WriteLine($"Unsportsmanlike foul with turnover by {_foulCommiter?.Number}.{_foulCommiter?.Name}");
                _defaultFreeThrows = 2;
                IsFouledPlayerSelectionActive = true;
                break;
            case "disqualifying":
                Debug.WriteLine($"Disqualifying foul by {_foulCommiter?.Number}.{_foulCommiter?.Name}");
                _defaultFreeThrows = 2;
                IsFouledPlayerSelectionActive = true;
                break;
            case "disqualifying turnover":
                Debug.WriteLine($"Disqualifying foul with turnover by {_foulCommiter?.Number}.{_foulCommiter?.Name}");
                _defaultFreeThrows = 2;
                IsFouledPlayerSelectionActive = true;
                break;
            case "technical":
                Debug.WriteLine($"Technical foul by {_foulCommiter?.Number}.{_foulCommiter?.Name}");
                _defaultFreeThrows = 1;
                BeginFreeThrowsAwardedSelection();
                break;
            default:
                IsFouledPlayerSelectionActive = true;
                break;
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
        IsFreeThrowsAwardedSelectionActive = false;
        FreeThrowResultRows.Clear();
        IsFreeThrowsSelectionActive = false;
        SelectedFreeThrowCount = 0;
        SelectedFreeThrowShooter = null;
        SelectedFreeThrowAssist = null;
        _defaultFreeThrows = 0;
        EligibleFreeThrowCourtPlayers.Clear();
        EligibleFreeThrowBenchPlayers.Clear();
        EligibleTeamACourtFoulCommitterPlayers.Clear();
        EligibleTeamABenchFoulCommitterPlayers.Clear();
        EligibleTeamBCourtFoulCommitterPlayers.Clear();
        EligibleTeamBBenchFoulCommitterPlayers.Clear();
        EligibleFoulOnCourtPlayers.Clear();
        EligibleFoulOnBenchPlayers.Clear();
        ResetSelectionState();
    }

    private void UpdateFoulCommiterPlayerStyles()
    {
        EligibleTeamACourtFoulCommitterPlayers.Clear();
        EligibleTeamABenchFoulCommitterPlayers.Clear();
        EligibleTeamBCourtFoulCommitterPlayers.Clear();
        EligibleTeamBBenchFoulCommitterPlayers.Clear();

        foreach (var vm in TeamAPlayers.Concat(TeamBPlayers))
        {
            bool isSelectable = IsFoulCommiterSelectionActive;
            vm.SetFoulCommiterSelectionMode(isSelectable);

            if (!isSelectable) continue;

            if (vm.Player.IsTeamA)
            {
                if (vm.Player.IsActive)
                    EligibleTeamACourtFoulCommitterPlayers.Add(vm);
                else
                    EligibleTeamABenchFoulCommitterPlayers.Add(vm);
            }
            else
            {
                if (vm.Player.IsActive)
                    EligibleTeamBCourtFoulCommitterPlayers.Add(vm);
                else
                    EligibleTeamBBenchFoulCommitterPlayers.Add(vm);
            }
        }

        OnPropertyChanged(nameof(EligibleTeamACourtFoulCommitterPlayers));
        OnPropertyChanged(nameof(EligibleTeamABenchFoulCommitterPlayers));
        OnPropertyChanged(nameof(EligibleTeamBCourtFoulCommitterPlayers));
        OnPropertyChanged(nameof(EligibleTeamBBenchFoulCommitterPlayers));
    }

    private void UpdateFouledPlayerStyles()
    {
        EligibleFoulOnCourtPlayers.Clear();
        EligibleFoulOnBenchPlayers.Clear();

        foreach (var vm in TeamAPlayers.Concat(TeamBPlayers))
        {
            bool isSelectable = false;

            if (IsFouledPlayerSelectionActive && _foulCommiter != null)
            {
                isSelectable = vm.Player.IsTeamA != _foulCommiter.IsTeamA;
            }

            vm.SetFouledPlayerSelectionMode(isSelectable);

            if (!isSelectable) continue;

            if (vm.Player.IsActive)
                EligibleFoulOnCourtPlayers.Add(vm);
            else
                EligibleFoulOnBenchPlayers.Add(vm);
        }

        OnPropertyChanged(nameof(EligibleFoulOnCourtPlayers));
        OnPropertyChanged(nameof(EligibleFoulOnBenchPlayers));
    }

    private void UpdateFreeThrowPlayerStyles()
    {
        EligibleFreeThrowCourtPlayers.Clear();
        EligibleFreeThrowBenchPlayers.Clear();

        var players = _freeThrowTeamIsTeamA ? TeamAPlayers : TeamBPlayers;

        foreach (var vm in players)
        {
            if (vm.Player.IsActive)
                EligibleFreeThrowCourtPlayers.Add(vm);
            else
                EligibleFreeThrowBenchPlayers.Add(vm);
        }

        OnPropertyChanged(nameof(EligibleFreeThrowCourtPlayers));
        OnPropertyChanged(nameof(EligibleFreeThrowBenchPlayers));

        UpdateFreeThrowShooterStyles();
    }

    private void UpdateFreeThrowShooterStyles()
    {
        foreach (var vm in TeamAPlayers.Concat(TeamBPlayers))
        {
            vm.IsSelectedAsFreeThrowShooter = vm.Player == SelectedFreeThrowShooter;
        }
    }

    private void BeginFreeThrowsAwardedSelection()
    {
        _freeThrowTeamIsTeamA = _fouledPlayer?.IsTeamA ?? _freeThrowTeamIsTeamA;
        SelectedFreeThrowCount = _defaultFreeThrows;
        SelectedFreeThrowShooter = _fouledPlayer;
        UpdateFreeThrowPlayerStyles();
        UpdateFreeThrowCountButtonStyles();
        IsFreeThrowsAwardedSelectionActive = true;
    }

    private void OnConfirmFreeThrowsAwarded()
    {
        FreeThrowCount = SelectedFreeThrowCount;
        _pendingShooter = SelectedFreeThrowShooter;
        StartFreeThrows(FreeThrowCount);
    }

    private void SelectFreeThrowShooter(Player? player)
    {
        if (player == null) return;

        if (SelectedFreeThrowShooter == player)
            SelectedFreeThrowShooter = null;
        else
            SelectedFreeThrowShooter = player;

        if (IsFreeThrowsSelectionActive)
        {
            _pendingShooter = SelectedFreeThrowShooter;
            SelectedFreeThrowAssist = null;
            UpdateAssistPlayerStyles();
        }
    }

    private void ToggleFreeThrowAssist(Player? player)
    {
        if (player == null) return;

        if (SelectedFreeThrowAssist == player)
            SelectedFreeThrowAssist = null;
        else
            SelectedFreeThrowAssist = player;
    }

    private void SetNoFreeThrowAssist()
    {
        SelectedFreeThrowAssist = null;
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
        IsQuickShotSelectionActive = false;
    }

    public void CancelCurrentAction()
    {
        TempMarkerRemoved?.Invoke();
        ResetSelectionState();
    }



    private void CompleteAssistSelection(Player? assistPlayer)
    {
        if (_pendingShooter != null)
        {
            if (assistPlayer?.Number == _pendingShooter.Number)
            {
                // Invalid: same player attempted to get an assist
                Debug.WriteLine($"{GameClockService.TimeLeftString} Assist not awarded — shooter cannot assist their own shot.");
            }
            else
            {
                var assist = assistPlayer != null
                ? $"Assist by {assistPlayer.Number}.{assistPlayer.Name}"
                : "No assist";
            Debug.WriteLine($"{GameClockService.TimeLeftString} {assist}");
                if (assistPlayer != null)
                    _currentPlayActions.Add(CreateAction(assistPlayer, "ASSIST"));
            }

            var shot = FormatShotAction(_pendingIsThreePoint, _wasBlocked ? "BLOCKED" : "MADE");
            _currentPlayActions.Insert(0, CreateAction(_pendingShooter, shot));
            AddPlayCard(_currentPlayActions.ToList());
            _currentPlayActions.Clear();
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

            Debug.WriteLine($"{GameClockService.TimeLeftString} {log} after miss by {_pendingShooter.Number}.{_pendingShooter.Name}");
            if (reboundSource is Player rp)
            {
                _currentPlayActions.Add(CreateAction(rp, "REBOUND"));
            }
            else if (reboundSource is string team && (team == "TeamA" || team == "TeamB"))
            {
                bool teamA = team == "TeamA";
                _currentPlayActions.Add(CreateTeamAction(teamA, "REBOUND"));
            }
            var shot = FormatShotAction(_pendingIsThreePoint, _wasBlocked ? "BLOCKED" : "MISSED");
            _currentPlayActions.Insert(0, CreateAction(_pendingShooter, shot));
            AddPlayCard(_currentPlayActions.ToList());
            _currentPlayActions.Clear();
        }

        ResetSelectionState();
    }

    private void CompleteBlockSelection(Player blocker)
    {
        if (_pendingShooter != null && blocker != null)
        {
            Debug.WriteLine($"{GameClockService.TimeLeftString} Block by {blocker.Number}.{blocker.Name} on {_pendingShooter.Number}.{_pendingShooter.Name}");
            _wasBlocked = true;
            _blocker = blocker;
            _currentPlayActions.Add(CreateAction(blocker, "BLOCK"));
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
            Debug.WriteLine($"{GameClockService.TimeLeftString} Turnover by {p.Number}.{p.Name}");
            _pendingShooter = p;
            _currentPlayActions.Clear();
            _currentPlayActions.Add(CreateAction(p, "TURNOVER"));
            IsTurnoverSelectionActive = false;
            IsStealSelectionActive = true; // move to steal selection
        }
        else if (source is string team)
        {
            Debug.WriteLine($"{GameClockService.TimeLeftString} Team turnover by {team}");
            bool teamA = team == "TeamA" || team == "Team A";
            AddPlayCard(new[] { CreateTeamAction(teamA, "TURNOVER") });
            ResetSelectionState();
        }
    }
    private void UpdateTurnoverPlayerStyles()
    {
        EligibleTeamACourtTurnoverPlayers.Clear();
        EligibleTeamABenchTurnoverPlayers.Clear();
        EligibleTeamBCourtTurnoverPlayers.Clear();
        EligibleTeamBBenchTurnoverPlayers.Clear();

        foreach (var vm in TeamAPlayers.Concat(TeamBPlayers))
        {
            bool isSelectable = IsTurnoverSelectionActive;
            vm.SetTurnoverSelectionMode(isSelectable);

            if (!isSelectable) continue;

            if (vm.Player.IsTeamA)
            {
                if (vm.Player.IsActive)
                    EligibleTeamACourtTurnoverPlayers.Add(vm);
                else
                    EligibleTeamABenchTurnoverPlayers.Add(vm);
            }
            else
            {
                if (vm.Player.IsActive)
                    EligibleTeamBCourtTurnoverPlayers.Add(vm);
                else
                    EligibleTeamBBenchTurnoverPlayers.Add(vm);
            }
        }

        OnPropertyChanged(nameof(EligibleTeamACourtTurnoverPlayers));
        OnPropertyChanged(nameof(EligibleTeamABenchTurnoverPlayers));
        OnPropertyChanged(nameof(EligibleTeamBCourtTurnoverPlayers));
        OnPropertyChanged(nameof(EligibleTeamBBenchTurnoverPlayers));
    }

    private void CompleteStealSelection(Player? stealer)
    {
        if (_pendingShooter == null)
            return;

        if (stealer == null)
        {
            Debug.WriteLine($"{GameClockService.TimeLeftString} No steal awarded on turnover by {_pendingShooter.Number}.{_pendingShooter.Name}");
            AddPlayCard(_currentPlayActions.ToList());
            _currentPlayActions.Clear();
            ResetSelectionState();
            return;
        }

        // If selected player is on same team, ignore the input and stay in steal mode
        if (stealer.IsTeamA == _pendingShooter.IsTeamA)
        {
            Debug.WriteLine($"{GameClockService.TimeLeftString} Invalid steal selection: {stealer.Number}.{stealer.Name} is on same team as turnover. Waiting for valid selection.");
            return;
        }

        // Valid steal
        Debug.WriteLine($"{GameClockService.TimeLeftString} Steal by {stealer.Number}.{stealer.Name} from {_pendingShooter.Number}.{_pendingShooter.Name}");
        _currentPlayActions.Add(CreateAction(stealer, "STEAL"));
        AddPlayCard(_currentPlayActions.ToList());
        _currentPlayActions.Clear();
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

        if (_pendingShooter == null)
        {
            OnPropertyChanged(nameof(EligibleCourtAssistPlayers));
            OnPropertyChanged(nameof(EligibleBenchAssistPlayers));
            return;
        }

        var players = _pendingShooter.IsTeamA ? TeamAPlayers : TeamBPlayers;

        foreach (var vm in players)
        {
            bool isSelectable = (IsAssistSelectionActive || IsFreeThrowsSelectionActive) &&
                               vm.Player.Number != _pendingShooter.Number;

            vm.SetAssistSelectionMode(isSelectable);
            vm.IsSelectedAsAssist = vm.Player == SelectedFreeThrowAssist;

            if (vm.Player.Number == _pendingShooter.Number) continue;

            if (vm.Player.IsActive)
                EligibleCourtAssistPlayers.Add(vm);
            else
                EligibleBenchAssistPlayers.Add(vm);
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

    private bool _isQuickShotSelectionActive;
    public bool IsQuickShotSelectionActive
    {
        get => _isQuickShotSelectionActive;
        set
        {
            _isQuickShotSelectionActive = value;
            OnPropertyChanged();
            UpdateQuickShotPlayerStyles();
            OnPropertyChanged(nameof(QuickShotPanelVisibility));
        }
    }

    public Visibility QuickShotPanelVisibility =>
        IsQuickShotSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    private void UpdateStealPlayerStyles()
    {
        EligibleStealCourtPlayers.Clear();
        EligibleStealBenchPlayers.Clear();

        foreach (var vm in TeamAPlayers.Concat(TeamBPlayers))
        {
            bool isSelectable = false;

            if (IsStealSelectionActive && _pendingShooter != null)
            {
                isSelectable = vm.Player.IsTeamA != _pendingShooter.IsTeamA;
            }

            vm.SetStealSelectionMode(isSelectable);

            if (isSelectable)
            {
                if (vm.Player.IsActive)
                    EligibleStealCourtPlayers.Add(vm);
                else
                    EligibleStealBenchPlayers.Add(vm);
            }
        }

        OnPropertyChanged(nameof(EligibleStealCourtPlayers));
        OnPropertyChanged(nameof(EligibleStealBenchPlayers));
    }

    private void UpdateQuickShotPlayerStyles()
    {
        EligibleTeamACourtShotPlayers.Clear();
        EligibleTeamABenchShotPlayers.Clear();
        EligibleTeamBCourtShotPlayers.Clear();
        EligibleTeamBBenchShotPlayers.Clear();

        foreach (var vm in TeamAPlayers.Concat(TeamBPlayers))
        {
            if (!IsQuickShotSelectionActive) continue;

            if (vm.Player.IsTeamA)
            {
                if (vm.Player.IsActive)
                    EligibleTeamACourtShotPlayers.Add(vm);
                else
                    EligibleTeamABenchShotPlayers.Add(vm);
            }
            else
            {
                if (vm.Player.IsActive)
                    EligibleTeamBCourtShotPlayers.Add(vm);
                else
                    EligibleTeamBBenchShotPlayers.Add(vm);
            }
        }

        OnPropertyChanged(nameof(EligibleTeamACourtShotPlayers));
        OnPropertyChanged(nameof(EligibleTeamABenchShotPlayers));
        OnPropertyChanged(nameof(EligibleTeamBCourtShotPlayers));
        OnPropertyChanged(nameof(EligibleTeamBBenchShotPlayers));
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
            OnPropertyChanged(nameof(FoulCommiterPanelVisibility));
            if (!value)
            {
                EligibleTeamACourtFoulCommitterPlayers.Clear();
                EligibleTeamABenchFoulCommitterPlayers.Clear();
                EligibleTeamBCourtFoulCommitterPlayers.Clear();
                EligibleTeamBBenchFoulCommitterPlayers.Clear();
                OnPropertyChanged(nameof(EligibleTeamACourtFoulCommitterPlayers));
                OnPropertyChanged(nameof(EligibleTeamABenchFoulCommitterPlayers));
                OnPropertyChanged(nameof(EligibleTeamBCourtFoulCommitterPlayers));
                OnPropertyChanged(nameof(EligibleTeamBBenchFoulCommitterPlayers));
            }
        }
    }

    public Visibility FoulCommiterPanelVisibility =>
        IsFoulCommiterSelectionActive ? Visibility.Visible : Visibility.Collapsed;

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
            if (!value)
            {
                EligibleFoulOnCourtPlayers.Clear();
                EligibleFoulOnBenchPlayers.Clear();
                OnPropertyChanged(nameof(EligibleFoulOnCourtPlayers));
                OnPropertyChanged(nameof(EligibleFoulOnBenchPlayers));
            }
        }
    }

    private bool _isFreeThrowsAwardedSelectionActive;
    public bool IsFreeThrowsAwardedSelectionActive
    {
        get => _isFreeThrowsAwardedSelectionActive;
        set
        {
            _isFreeThrowsAwardedSelectionActive = value;
            OnPropertyChanged();
            if (value)
                UpdateFreeThrowPlayerStyles();
            OnPropertyChanged(nameof(FreeThrowsAwardedPanelVisibility));
        }
    }

    public Visibility FoulTypePanelVisibility =>
    IsFoulTypeSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    public Visibility FreeThrowsAwardedPanelVisibility =>
        IsFreeThrowsAwardedSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    public int FreeThrowCount { get; private set; } = 0;
    public int SelectedFreeThrowCount
    {
        get => _selectedFreeThrowCount;
        set
        {
            _selectedFreeThrowCount = value;
            if (value == 0)
                SelectedFreeThrowShooter = null;
            OnPropertyChanged();
            UpdateFreeThrowCountButtonStyles();
            OnPropertyChanged(nameof(FreeThrowShooterVisibility));
        }
    }

    public Player? SelectedFreeThrowShooter
    {
        get => _selectedFreeThrowShooter;
        set
        {
            _selectedFreeThrowShooter = value;
            UpdateFreeThrowShooterStyles();
            if (IsFreeThrowsSelectionActive)
            {
                _pendingShooter = value;
                SelectedFreeThrowAssist = null;
                UpdateAssistPlayerStyles();
            }
            OnPropertyChanged();
        }
    }

    public Player? SelectedFreeThrowAssist
    {
        get => _selectedFreeThrowAssist;
        set
        {
            _selectedFreeThrowAssist = value;
            UpdateAssistPlayerStyles();
            OnPropertyChanged();
        }
    }

    public Visibility FreeThrowShooterVisibility =>
        SelectedFreeThrowCount == 0 ? Visibility.Collapsed : Visibility.Visible;

    private bool _isFreeThrowsSelectionActive;
    public bool IsFreeThrowsSelectionActive
    {
        get => _isFreeThrowsSelectionActive;
        set
        {
            _isFreeThrowsSelectionActive = value;
            OnPropertyChanged();
            if (value)
                UpdateFreeThrowPlayerStyles();
            OnPropertyChanged(nameof(FreeThrowsPanelVisibility));
            OnPropertyChanged(nameof(FreeThrowResultRows));
        }
    }

    public Visibility FreeThrowsPanelVisibility =>
        IsFreeThrowsSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    private void StartFreeThrows(int count)
    {
        IsFreeThrowsAwardedSelectionActive = false;
        SelectedFreeThrowAssist = null;

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

        UpdateAssistPlayerStyles();

        IsFreeThrowsSelectionActive = true;
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
                Debug.WriteLine($"{GameClockService.TimeLeftString} {r.Label} {r.Result}");
            }

            var made = FreeThrowResultRows.Count(r => r.Result == "MADE");
            var missed = FreeThrowResultRows.Count(r => r.Result == "MISSED");

            if (_pendingShooter != null)
            {
                Debug.WriteLine($"{GameClockService.TimeLeftString} Free throws by {_pendingShooter.Number}.{_pendingShooter.Name}: {made} made, {missed} missed");
                if (made > 0 && SelectedFreeThrowAssist != null)
                {
                    Debug.WriteLine($"{GameClockService.TimeLeftString} Assist by {SelectedFreeThrowAssist.Number}.{SelectedFreeThrowAssist.Name}");
                }

                var actions = new List<PlayActionViewModel>();
                foreach (var r in FreeThrowResultRows)
                {
                    actions.Add(CreateAction(_pendingShooter, $"FTA {r.Result}"));
                }
                if (made > 0 && SelectedFreeThrowAssist != null)
                {
                    actions.Add(CreateAction(SelectedFreeThrowAssist, "ASSIST"));
                }
                AddPlayCard(actions);
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

    private void UpdateFreeThrowCountButtonStyles()
    {
        OnPropertyChanged(nameof(FreeThrowCount0Style));
        OnPropertyChanged(nameof(FreeThrowCount1Style));
        OnPropertyChanged(nameof(FreeThrowCount2Style));
        OnPropertyChanged(nameof(FreeThrowCount3Style));
    }



    // Action button styles
    public Style MadeButtonStyle => GetActionStyle("MADE");
    public Style MissedButtonStyle => GetActionStyle("MISSED");
    public Style FoulButtonStyle => GetActionStyle("FOUL");
    public Style TurnoverButtonStyle => GetActionStyle("TURNOVER");
    public Style FreeThrowCount0Style => GetFreeThrowCountStyle(0);
    public Style FreeThrowCount1Style => GetFreeThrowCountStyle(1);
    public Style FreeThrowCount2Style => GetFreeThrowCountStyle(2);
    public Style FreeThrowCount3Style => GetFreeThrowCountStyle(3);

    private Style GetActionStyle(string action)
    {
        if (!IsActionSelectionActive && action != "FOUL")
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

    private Style GetFreeThrowCountStyle(int count)
    {
        return SelectedFreeThrowCount == count
            ? (Style)_resources["FreeThrowCountSelectedStyle"]
            : (Style)_resources["FreeThrowCountButtonStyle"];
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
        var actions = new List<PlayActionViewModel>();

        foreach (var p in TeamASubIn)
            actions.Add(CreateAction(p, "SUB PLAYER IN"));
        foreach (var p in TeamASubOut)
            actions.Add(CreateAction(p, "SUB PLAYER OUT"));
        foreach (var p in TeamBSubIn)
            actions.Add(CreateAction(p, "SUB PLAYER IN"));
        foreach (var p in TeamBSubOut)
            actions.Add(CreateAction(p, "SUB PLAYER OUT"));

        if (actions.Count > 0)
            AddPlayCard(actions);

        ApplySubstitution(TeamASubIn, TeamASubOut);
        ApplySubstitution(TeamBSubIn, TeamBSubOut);

        TeamASubIn.Clear();
        TeamASubOut.Clear();
        TeamBSubIn.Clear();
        TeamBSubOut.Clear();

        RegenerateTeams();
        IsSubstitutionPanelVisible = false;
        OnPropertyChanged(nameof(IsSubstitutionConfirmEnabled));
    }

    private void ApplySubstitution(IEnumerable<Player> subIn, IEnumerable<Player> subOut)
    {
        foreach (var p in subOut)
            p.IsActive = false;

        foreach (var p in subIn)
            p.IsActive = true;
    }

    // ---- PlayByPlay log helpers ----

    private PlayActionViewModel CreateAction(Player player, string action)
    {
        Debug.WriteLine($"CreateAction: {player.Number} {player.Name} {action}");
        return new PlayActionViewModel
        {
            TeamColor = GetTeamColorFromPlayer(player),
            PlayerNumber = player.Number.ToString(),
            FirstName = GetFirstName(player.Name),
            LastName = GetLastName(player.Name),
            Action = action
        };
    }

    private PlayActionViewModel CreateTeamAction(bool teamA, string action)
    {
        var name = teamA ? TeamAName : TeamBName;
        Debug.WriteLine($"CreateTeamAction: {name} {action}");
        return new PlayActionViewModel
        {
            TeamColor = teamA ? (Brush)_resources["CourtAColor"] : (Brush)_resources["CourtBColor"],
            PlayerNumber = string.Empty,
            FirstName = name,
            LastName = string.Empty,
            Action = action
        };
    }

    private void AddPlayCard(IEnumerable<PlayActionViewModel> actions)
    {
        var card = new PlayCardViewModel
        {
            Time = GameClockService.TimeLeftString,
            TeamAScore = TeamAScore,
            TeamBScore = TeamBScore
        };
        foreach (var a in actions)
            card.Actions.Add(a);

        PlayByPlayCards.Insert(0, card);
        Debug.WriteLine($"Play card added: {card.Header}");
    }

    private static string FormatShotAction(bool isThree, string result)
    {
        var prefix = isThree ? "3P" : "2P";
        return result == "MADE"
            ? $"{prefix}M"
            : $"{prefix}A {result}";
    }

    private static string GetFirstName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;
        var parts = name.Split(' ');
        return parts.Length > 0 ? parts[0] : string.Empty;
    }

    private static string GetLastName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;
        var parts = name.Split(' ');
        return parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;
    }

    private void GenerateSamplePlayByPlayData()
    {
        if (Players.Count == 0) return;

        var shooter = Players.First();
        AddPlayCard(new[] { CreateAction(shooter, FormatShotAction(false, "MADE")) });

        var missShooter = Players.ElementAtOrDefault(3);
        var rebounder = Players.ElementAtOrDefault(8);
        if (missShooter != null && rebounder != null)
        {
            AddPlayCard(new[]
            {
                CreateAction(missShooter, FormatShotAction(false, "MISSED")),
                CreateAction(rebounder, "REBOUND")
            });
        }

        AddPlayCard(new[] { CreateTeamAction(true, "TIMEOUT") });
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
        Result == "MISSED" ? "LightGray" : "DarkGray";

    public string MissedButtonBackground =>
        Result == "MISSED" ? "Red" :
        Result == "MADE" ? "LightGray" : "DarkGray";

    public ICommand SelectResultCommand { get; }

    public FreeThrowResult(int index, int total, Action<int, string> onResultSelected)
    {
        Index = index;
        Total = total;
        SelectResultCommand = new RelayCommand(
            param => onResultSelected(Index, param?.ToString() ?? ""));
    }
}