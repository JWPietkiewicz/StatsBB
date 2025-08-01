using StatsBB.Model;
using StatsBB.MVVM;
using StatsBB.Services;
using Domain = StatsBB.Domain;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using StatsBB.Domain;
using System.ComponentModel;

namespace StatsBB.ViewModel;

public class MainWindowViewModel : ViewModelBase
{
    public TeamInfoViewModel TeamInfoVM { get; }
    public Game Game => TeamInfoVM.Game;
    public ObservableCollection<Player> Players { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> TeamAPlayers { get; } = new();
    public ObservableCollection<PlayerPositionViewModel> TeamBPlayers { get; } = new();
    public ObservableCollection<FreeThrowResult> FreeThrowResultRows { get; } = new();
    public ObservableCollection<Player> TeamASubIn { get; } = new();
    public ObservableCollection<Player> TeamASubOut { get; } = new();
    public ObservableCollection<Player> TeamBSubIn { get; } = new();
    public ObservableCollection<Player> TeamBSubOut { get; } = new();
    public ObservableCollection<Player> TeamAJumpingPlayers { get; } = new();
    public ObservableCollection<Player> TeamBJumpingPlayers { get; } = new();
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

    private readonly ActionProcessor _actionProcessor;

    public PlayByPlayLogViewModel PlayLog { get; } = new();

    /// <summary>
    /// View model used by the scoreboard view.
    /// </summary>
    public ScoreBoardViewModel ScoreBoardVM { get; }

    public ObservableCollection<Player> TeamACourtPlayers { get; } = new();
    public ObservableCollection<Player> TeamBCourtPlayers { get; } = new();
    public ObservableCollection<Player> TeamABenchPlayers { get; } = new();
    public ObservableCollection<Player> TeamBBenchPlayers { get; } = new();
    public ObservableCollection<Player> TeamAStartingFivePlayers { get; } = new();
    public ObservableCollection<Player> TeamAStartingBenchPlayers { get; } = new();
    public ObservableCollection<Player> TeamBStartingFivePlayers { get; } = new();
    public ObservableCollection<Player> TeamBStartingBenchPlayers { get; } = new();

    public ObservableCollection<TeamColorOption> ColorOptions { get; } = new();

    public TeamInfo TeamAInfo { get; } = new();
    public TeamInfo TeamBInfo { get; } = new();

    private TeamColorOption? _teamAColorOption;
    public TeamColorOption? TeamAColorOption
    {
        get => _teamAColorOption;
        set
        {
            if (_teamAColorOption == value)
                return;

            _teamAColorOption = value;
            OnPropertyChanged();
            if (value != null)
            {
                _resources["PrimaryAColor"] = new SolidColorBrush(((SolidColorBrush)value.ColorBrush).Color);
                _resources["SecondaryAColor"] = new SolidColorBrush(((SolidColorBrush)value.TextBrush).Color);
            }
            if (TeamAInfo.Color != value)
                TeamAInfo.Color = value;
        }
    }

    private TeamColorOption? _teamBColorOption;
    public TeamColorOption? TeamBColorOption
    {
        get => _teamBColorOption;
        set
        {
            if (_teamBColorOption == value)
                return;

            _teamBColorOption = value;
            OnPropertyChanged();
            if (value != null)
            {
                _resources["PrimaryBColor"] = new SolidColorBrush(((SolidColorBrush)value.ColorBrush).Color);
                _resources["SecondaryBColor"] = new SolidColorBrush(((SolidColorBrush)value.TextBrush).Color);
            }
            if (TeamBInfo.Color != value)
                TeamBInfo.Color = value;
        }
    }

    private string _teamAName = "Team A";
    public string TeamAName
    {
        get => _teamAName;
        set
        {
            if (_teamAName == value)
                return;

            _teamAName = value;
            OnPropertyChanged();
            if (TeamAInfo.Name != value)
                TeamAInfo.Name = value;
        }
    }

    private string _teamAShortName = "A";
    public string TeamAShortName
    {
        get => _teamAShortName;
        set
        {
            if (_teamAShortName == value)
                return;

            _teamAShortName = value;
            OnPropertyChanged();
            if (TeamAInfo.ShortName != value)
                TeamAInfo.ShortName = value;
        }
    }

    private string _teamBName = "Team B";
    public string TeamBName
    {
        get => _teamBName;
        set
        {
            if (_teamBName == value)
                return;

            _teamBName = value;
            OnPropertyChanged();
            if (TeamBInfo.Name != value)
                TeamBInfo.Name = value;
        }
    }

    private string _teamBShortName = "B";
    public string TeamBShortName
    {
        get => _teamBShortName;
        set
        {
            if (_teamBShortName == value)
                return;

            _teamBShortName = value;
            OnPropertyChanged();
            if (TeamBInfo.ShortName != value)
                TeamBInfo.ShortName = value;
        }
    }


    public GameStateViewModel GameState { get; } = new();

    private bool _isSubstitutionPanelVisible;
    public bool IsSubstitutionPanelVisible
    {
        get => _isSubstitutionPanelVisible;
        set
        {
            _isSubstitutionPanelVisible = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SubstitutionPanelVisibility));
            NotifyActionInProgressChanged();
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
            NotifyActionInProgressChanged();
        }
    }
    public Visibility TimeOutPanelVisibility =>
        IsTimeOutSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    private bool _isStartingFivePanelVisible;
    public bool IsStartingFivePanelVisible
    {
        get => _isStartingFivePanelVisible;
        set
        {
            _isStartingFivePanelVisible = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StartingFivePanelVisibility));
            NotifyActionInProgressChanged();
        }
    }
    public Visibility StartingFivePanelVisibility =>
        IsStartingFivePanelVisible ? Visibility.Visible : Visibility.Collapsed;

    private bool _isStartingFiveButtonEnabled = true;
    public bool IsStartingFiveButtonEnabled
    {
        get => _isStartingFiveButtonEnabled;
        set
        {
            if (_isStartingFiveButtonEnabled == value)
                return;
            _isStartingFiveButtonEnabled = value;
            OnPropertyChanged();
        }
    }


    private bool _isGamePanelVisible = true;
    public bool IsGamePanelVisible
    {
        get => _isGamePanelVisible;
        set
        {
            _isGamePanelVisible = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(GamePanelVisibility));
        }
    }
    public Visibility GamePanelVisibility =>
        IsGamePanelVisible ? Visibility.Visible : Visibility.Collapsed;

    public bool AreTeamsConfirmed => TeamInfoVM.AreTeamsConfirmed;

    private bool _isGameFinalized;
    public bool IsGameFinalized
    {
        get => _isGameFinalized;
        set
        {
            if (_isGameFinalized == value)
                return;

            _isGameFinalized = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsTeamInfoEnabled));
            OnPropertyChanged(nameof(IsMainTabEnabled));
        }
    }

    public bool IsTeamInfoEnabled => !IsGameFinalized;
    public bool IsMainTabEnabled => AreTeamsConfirmed && !IsGameFinalized;

    private int _selectedTabIndex;
    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set
        {
            if (_selectedTabIndex == value)
                return;

            _selectedTabIndex = value;
            OnPropertyChanged();
        }
    }

    private bool _isJumpBallPanelVisible;
    public bool IsJumpBallPanelVisible
    {
        get => _isJumpBallPanelVisible;
        set
        {
            _isJumpBallPanelVisible = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(JumpBallPanelVisibility));
            NotifyActionInProgressChanged();
        }
    }
    public Visibility JumpBallPanelVisibility =>
        IsJumpBallPanelVisible ? Visibility.Visible : Visibility.Collapsed;

    private bool _isJumpBallChoicePanelVisible;
    public bool IsJumpBallChoicePanelVisible
    {
        get => _isJumpBallChoicePanelVisible;
        set
        {
            _isJumpBallChoicePanelVisible = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(JumpBallChoicePanelVisibility));
            NotifyActionInProgressChanged();
        }
    }
    public Visibility JumpBallChoicePanelVisibility =>
        IsJumpBallChoicePanelVisible ? Visibility.Visible : Visibility.Collapsed;

    private bool _isJumpBallContestedPanelVisible;
    public bool IsJumpBallContestedPanelVisible
    {
        get => _isJumpBallContestedPanelVisible;
        set
        {
            _isJumpBallContestedPanelVisible = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(JumpBallContestedPanelVisibility));
            NotifyActionInProgressChanged();
        }
    }
    public Visibility JumpBallContestedPanelVisibility =>
        IsJumpBallContestedPanelVisible ? Visibility.Visible : Visibility.Collapsed;

    private bool _isJumpWinnerPanelVisible;
    public bool IsJumpWinnerPanelVisible
    {
        get => _isJumpWinnerPanelVisible;
        set
        {
            _isJumpWinnerPanelVisible = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(JumpWinnerPanelVisibility));
            NotifyActionInProgressChanged();
        }
    }
    public Visibility JumpWinnerPanelVisibility =>
        IsJumpWinnerPanelVisible ? Visibility.Visible : Visibility.Collapsed;

    public bool IsJumpEnabled =>
        TeamAJumpingPlayers.Count == 1 && TeamBJumpingPlayers.Count == 1;

    private readonly ResourceDictionary _resources;

    public ICommand SelectActionCommand { get; }
    public ICommand NoAssistCommand { get; }
    public ICommand AssistTeamACommand { get; }
    public ICommand AssistTeamBCommand { get; }
    public ICommand ReboundTeamACommand { get; }
    public ICommand ReboundTeamBCommand { get; }
    public ICommand NoReboundCommand { get; }
    public ICommand BlockCommand { get; }
    public ICommand ShotClockCommand { get; }
    public ICommand TurnoverTeamACommand { get; }
    public ICommand TurnoverTeamBCommand { get; }
    public ICommand StartTurnoverCommand { get; }
    public ICommand StartAssistCommand { get; }
    public ICommand StartReboundCommand { get; }
    public ICommand StartStealCommand { get; }
    public ICommand StartFreeThrowsCommand { get; }
    public ICommand FreeThrowTeamACommand { get; }
    public ICommand FreeThrowTeamBCommand { get; }
    public ICommand NoStealCommand { get; }
    public ICommand StealTeamACommand { get; }
    public ICommand StealTeamBCommand { get; }
    public ICommand SelectFoulTypeCommand { get; }
    public ICommand SelectReboundTypeCommand { get; }
    public ICommand SelectFreeThrowCountCommand { get; }
    public ICommand StartSubstitutionCommand { get; }
    public ICommand StartStartingFiveCommand { get; }
    public ICommand ConfirmSubstitutionCommand { get; }
    public ICommand ConfirmStartingFiveCommand { get; }
    public ICommand ToggleSubInCommand { get; }
    public ICommand ToggleSubOutCommand { get; }
    public ICommand SubOutAllTeamACommand { get; }
    public ICommand SubOutAllTeamBCommand { get; }
    public ICommand ToggleStartingFiveCommand { get; }
    public ICommand StartTimeoutCommand { get; }
    public ICommand TimeoutTeamACommand { get; }
    public ICommand TimeoutTeamBCommand { get; }
    public ICommand SetupGameCommand { get; }
    public ICommand StartJumpBallCommand { get; }
    public ICommand ToggleJumpPlayerCommand { get; }
    public ICommand ConfirmJumpBallCommand { get; }
    public ICommand JumpTeamACommand { get; }
    public ICommand JumpTeamBCommand { get; }
    public ICommand JumpBallOptionCommand { get; }
    public ICommand ContestOptionCommand { get; }
    public ICommand ContestReboundCommand { get; }
    public ICommand ContestTurnoverCommand { get; }
    public ICommand CoachTechnicalTeamACommand { get; }
    public ICommand CoachTechnicalTeamBCommand { get; }
    public ICommand BenchTechnicalTeamACommand { get; }
    public ICommand BenchTechnicalTeamBCommand { get; }
    public ICommand ConfirmFreeThrowsAwardedCommand { get; }
    public ICommand SelectFreeThrowShooterCommand { get; }
    public ICommand SelectTeamAColorCommand { get; }
    public ICommand SelectTeamBColorCommand { get; }
    public ICommand SelectFreeThrowAssistCommand { get; }
    public ICommand NoAssistFreeThrowCommand { get; }
    public ICommand SwapSidesCommand { get; }
    public ICommand SwapPossessionCommand { get; }
    public ICommand SwapArrowCommand { get; }
    public ICommand SwapPossessionArrowCommand { get; }


    public event Action<Point, Brush, bool>? MarkerRequested;
    public event Action<Point>? TempMarkerRequested;
    public event Action? TempMarkerRemoved;
    public event Action? ClearMarkersRequested;

    public MainWindowViewModel(ResourceDictionary resources)
    {
        _resources = resources;
        TeamInfoVM = new TeamInfoViewModel(this);
        ScoreBoardVM = new ScoreBoardViewModel(this);
        SelectedTabIndex = 0;

        StartSubstitutionCommand = new RelayCommand(_ => BeginSubstitution(), _ => !IsActionInProgress);

        SelectActionCommand = new RelayCommand(
            param => SelectAction(param?.ToString()),
            param => (IsActionSelectionActive ||
                     (param?.ToString()?.Equals("Foul",
                         StringComparison.InvariantCultureIgnoreCase) ?? false)) &&
                     !IsActionInProgress
        );

        NoAssistCommand = new RelayCommand(
            _ => CompleteAssistSelection(null),
            _ => IsAssistSelectionActive
        );
        AssistTeamACommand = new RelayCommand(_ => SelectAssistTeam(true), _ => IsAssistTeamSelectionActive);
        AssistTeamBCommand = new RelayCommand(_ => SelectAssistTeam(false), _ => IsAssistTeamSelectionActive);
        NoStealCommand = new RelayCommand(
            _ => CompleteStealSelection(null),
            _ => IsStealSelectionActive
        );
        StealTeamACommand = new RelayCommand(_ => SelectStealTeam(true), _ => IsStealTeamSelectionActive);
        StealTeamBCommand = new RelayCommand(_ => SelectStealTeam(false), _ => IsStealTeamSelectionActive);

        SelectFoulTypeCommand = new RelayCommand(
            param => OnFoulTypeSelected(param?.ToString())
        );

        SelectReboundTypeCommand = new RelayCommand(
            param => OnReboundTypeSelected(param?.ToString())
        );

        SelectFreeThrowCountCommand = new RelayCommand(
            param => SelectedFreeThrowCount = Convert.ToInt32(param)
        );

        ReboundTeamACommand = new RelayCommand(_ => OnReboundTargetSelected("TeamA"), _ => IsReboundSelectionActive);
        ReboundTeamBCommand = new RelayCommand(_ => OnReboundTargetSelected("TeamB"), _ => IsReboundSelectionActive);
        NoReboundCommand = new RelayCommand(_ => CompleteReboundSelection(null), _ => IsReboundSelectionActive);
        BlockCommand = new RelayCommand(
            _ => EnterBlockerSelection(),
            _ => IsReboundSelectionActive && !_wasBlocked && !_pendingFreeThrowRebound
        );
        ShotClockCommand = new RelayCommand(_ => CompleteReboundSelection("24"), _ => IsReboundSelectionActive);
        TurnoverTeamACommand = new RelayCommand(_ => CompleteTurnoverSelection("TeamA"), _ => IsTurnoverSelectionActive);
        TurnoverTeamBCommand = new RelayCommand(_ => CompleteTurnoverSelection("TeamB"), _ => IsTurnoverSelectionActive);

        StartSubstitutionCommand = new RelayCommand(_ => BeginSubstitution(), _ => !IsActionInProgress);
        StartStartingFiveCommand = new RelayCommand(_ => BeginStartingFive(), _ => !IsActionInProgress);
        SetupGameCommand = new RelayCommand(_ => SetupGame());
        ConfirmSubstitutionCommand = new RelayCommand(_ => ConfirmSubstitution(), _ => IsSubstitutionConfirmEnabled);
        ConfirmStartingFiveCommand = new RelayCommand(_ => ConfirmStartingFive(), _ => IsStartingFiveConfirmEnabled);
        ToggleSubInCommand = new RelayCommand(p => ToggleSubIn(p as Player));
        ToggleSubOutCommand = new RelayCommand(p => ToggleSubOut(p as Player));
        SubOutAllTeamACommand = new RelayCommand(_ => ToggleSubOutAll(true));
        SubOutAllTeamBCommand = new RelayCommand(_ => ToggleSubOutAll(false));
        ToggleStartingFiveCommand = new RelayCommand(p => ToggleStartingFive(p as Player));
        StartTimeoutCommand = new RelayCommand(_ => BeginTimeout(), _ => !IsActionInProgress);
        StartJumpBallCommand = new RelayCommand(_ => BeginJumpBallChoice(), _ => !IsActionInProgress);
        TimeoutTeamACommand = new RelayCommand(
            _ => CompleteTimeoutSelection("Team A"),
            _ => IsTimeOutSelectionActive && GameState.TeamATimeOutsLeft > 0);
        TimeoutTeamBCommand = new RelayCommand(
            _ => CompleteTimeoutSelection("Team B"),
            _ => IsTimeOutSelectionActive && GameState.TeamBTimeOutsLeft > 0);
        ToggleJumpPlayerCommand = new RelayCommand(p => ToggleJumpPlayer(p as Player));
        ConfirmJumpBallCommand = new RelayCommand(_ => ConfirmJumpBall(), _ => IsJumpEnabled);
        JumpTeamACommand = new RelayCommand(_ => CompleteJumpBall(true));
        JumpTeamBCommand = new RelayCommand(_ => CompleteJumpBall(false));
        JumpBallOptionCommand = new RelayCommand(_ => OnJumpBallSelected());
        ContestOptionCommand = new RelayCommand(_ => OnContestSelected());
        ContestReboundCommand = new RelayCommand(_ => CompleteContestedRebound());
        ContestTurnoverCommand = new RelayCommand(_ => CompleteContestedTurnover());
        StartTurnoverCommand = new RelayCommand(_ => BeginTurnover(), _ => !IsActionInProgress);
        StartAssistCommand = new RelayCommand(_ => BeginAssist(), _ => !IsActionInProgress);
        StartReboundCommand = new RelayCommand(_ => BeginRebound(), _ => !IsActionInProgress);
        StartStealCommand = new RelayCommand(_ => BeginSteal(), _ => !IsActionInProgress);
        StartFreeThrowsCommand = new RelayCommand(_ => BeginFreeThrows(), _ => !IsActionInProgress);
        FreeThrowTeamACommand = new RelayCommand(_ => SelectFreeThrowTeam(true), _ => IsFreeThrowTeamSelectionActive);
        FreeThrowTeamBCommand = new RelayCommand(_ => SelectFreeThrowTeam(false), _ => IsFreeThrowTeamSelectionActive);

        CoachTechnicalTeamACommand = new RelayCommand(_ => OnCoachTechnical("Team A"));
        CoachTechnicalTeamBCommand = new RelayCommand(_ => OnCoachTechnical("Team B"));
        BenchTechnicalTeamACommand = new RelayCommand(_ => OnBenchTechnical("Team A"));
        BenchTechnicalTeamBCommand = new RelayCommand(_ => OnBenchTechnical("Team B"));
        ConfirmFreeThrowsAwardedCommand = new RelayCommand(_ => OnConfirmFreeThrowsAwarded());
        SelectFreeThrowShooterCommand = new RelayCommand(p => SelectFreeThrowShooter(p as Player));
        SelectFreeThrowAssistCommand = new RelayCommand(p => ToggleFreeThrowAssist(p as Player));
        NoAssistFreeThrowCommand = new RelayCommand(_ => SetNoFreeThrowAssist());

        SwapSidesCommand = new RelayCommand(_ => SwapSides());
        SwapPossessionCommand = new RelayCommand(_ => GameClockService.SwapPossession());
        SwapArrowCommand = new RelayCommand(_ => GameClockService.SwapArrow());
        SwapPossessionArrowCommand = new RelayCommand(_ => GameClockService.SwapPossessionAndArrow());

        SelectTeamAColorCommand = new RelayCommand(p =>
        {
            TeamAColorOption = p as TeamColorOption;
            TeamAInfo.Color = TeamAColorOption;
        });
        SelectTeamBColorCommand = new RelayCommand(p =>
        {
            TeamBColorOption = p as TeamColorOption;
            TeamBInfo.Color = TeamBColorOption;
        });

        ColorOptions.Add(new TeamColorOption("Yellow", "#FFFF00", "Black"));
        ColorOptions.Add(new TeamColorOption("Orange", "#FFA500", "Black"));
        ColorOptions.Add(new TeamColorOption("Blue", "#0000FF", "White"));
        ColorOptions.Add(new TeamColorOption("Light blue", "#ADD8E6", "Black"));
        ColorOptions.Add(new TeamColorOption("Dark blue", "#00008B", "White"));
        ColorOptions.Add(new TeamColorOption("Grey", "#808080", "Black"));
        ColorOptions.Add(new TeamColorOption("Black", "#000000", "White"));
        ColorOptions.Add(new TeamColorOption("Purple", "#800080", "White"));
        ColorOptions.Add(new TeamColorOption("Pink", "#FFC0CB", "Black"));
        ColorOptions.Add(new TeamColorOption("Red", "#FF0000", "Black"));
        ColorOptions.Add(new TeamColorOption("Green", "#008000", "White"));
        ColorOptions.Add(new TeamColorOption("White", "#FFFFFF", "Black"));
        ColorOptions.Add(new TeamColorOption("Teal", "#008080", "White"));
        ColorOptions.Add(new TeamColorOption("Lime", "#00FF00", "Black"));
        ColorOptions.Add(new TeamColorOption("Brown", "#A52A2A", "White"));
        ColorOptions.Add(new TeamColorOption("Maroon", "#800000", "White"));
        ColorOptions.Add(new TeamColorOption("Blue grey", "#6699CC", "Black"));
        ColorOptions.Add(new TeamColorOption("Mustard", "#FFDB58", "Black"));

        TeamAInfo.Name = TeamAName;
        TeamBInfo.Name = TeamBName;
        TeamAInfo.ShortName = TeamAShortName;
        TeamBInfo.ShortName = TeamBShortName;

        TeamAInfo.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(TeamInfo.Name) && TeamAName != TeamAInfo.Name)
                TeamAName = TeamAInfo.Name;
            if (e.PropertyName == nameof(TeamInfo.ShortName) && TeamAShortName != TeamAInfo.ShortName)
                TeamAShortName = TeamAInfo.ShortName;
            if (e.PropertyName == nameof(TeamInfo.Color) && TeamAColorOption != TeamAInfo.Color)
                TeamAColorOption = TeamAInfo.Color;
        };
        TeamBInfo.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(TeamInfo.Name) && TeamBName != TeamBInfo.Name)
                TeamBName = TeamBInfo.Name;
            if (e.PropertyName == nameof(TeamInfo.ShortName) && TeamBShortName != TeamBInfo.ShortName)
                TeamBShortName = TeamBInfo.ShortName;
            if (e.PropertyName == nameof(TeamInfo.Color) && TeamBColorOption != TeamBInfo.Color)
                TeamBColorOption = TeamBInfo.Color;
        };

        TeamInfoVM.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(TeamInfoViewModel.AreTeamsConfirmed))
            {
                OnPropertyChanged(nameof(AreTeamsConfirmed));
                OnPropertyChanged(nameof(IsMainTabEnabled));
            }
        };

        Game.HomeTeam.Players.CollectionChanged += TeamPlayersChanged;
        Game.AwayTeam.Players.CollectionChanged += TeamPlayersChanged;

        //TeamAInfo.Players.CollectionChanged += TeamPlayersChanged;
        //TeamBInfo.Players.CollectionChanged += TeamPlayersChanged;
        RegenerateTeamsFromInfo();
        //PlayerLayoutService.PopulateTeams(Players);
        RegenerateTeams();

        StatsVM = new StatsTabViewModel(TeamInfoVM.Game);
        _actionProcessor = new ActionProcessor(TeamInfoVM.Game);
        SubscribePlayerEvents();
        GameClockService.EndPeriodRequested += OnEndPeriodRequested;
        GameClockService.FinalizeGameRequested += OnFinalizeGameRequested;
        GameClockService.ClockStarted += OnClockStarted;
        // StatsVM = new StatsTabViewModel(Players);

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

    private void SetupGame()
    {
        Game.InitializePeriods(Game.DefaultPeriods);
        Game.CurrentPeriod = 0;
        var period = Game.GetCurrentPeriod();
        GameState.TeamATotalTimeouts = GameState.TeamATimeOutsLeft = 2;
        GameState.TeamBTotalTimeouts = GameState.TeamBTimeOutsLeft = 2;
        GameState.ResetPeriodFouls();
        GameClockService.Reset(period.Length, $"{period.Name} - {period.Status}", "Start Game");
        ClearMarkersRequested?.Invoke();
        IsGamePanelVisible = false;
        IsStartingFiveButtonEnabled = true;
        BeginStartingFive();
    }

    private void OnEndPeriodRequested()
    {
        var period = Game.GetCurrentPeriod();
        period.Status = PeriodStatus.Ended;

        var endActions = new List<PlayActionViewModel>
        {
            CreateGameAction(PlayType.PeriodEnd)
        };
        if (period.IsRegular && (period.PeriodNumber == 2 || period.PeriodNumber == Game.DefaultPeriods))
            endActions.Add(CreateGameAction(PlayType.HalfEnd));
        AddPlayCard(endActions);

        bool lastRegular = period.IsRegular && period.PeriodNumber == Game.DefaultPeriods;
        bool overtime = !period.IsRegular;

        if ((lastRegular || overtime) && Game.HomeTeam.Points != Game.AwayTeam.Points)
        {
            GameClockService.SetState("FINALIZE GAME", true);
            return;
        }

        if (period.PeriodNumber == Game.Periods.Count && Game.HomeTeam.Points == Game.AwayTeam.Points)
        {
            period = Game.AddOvertimePeriod();
            Game.CurrentPeriod = Game.Periods.Count - 1;
        }
        else if (period.PeriodNumber < Game.Periods.Count)
        {
            Game.CurrentPeriod++;
            period = Game.GetCurrentPeriod();
        }

        GameState.ResetPeriodFouls();
        period.Status = PeriodStatus.Setup;
        GameClockService.Reset(period.Length, $"{period.Name} - {period.Status}", "Start Period");
        ClearMarkersRequested?.Invoke();
    }

    private void OnFinalizeGameRequested()
    {
        AddPlayCard(new[] { CreateGameAction(PlayType.GameEnd) });
        GameClockService.SetState("FINALIZED", false);
        IsGameFinalized = true;
        SelectedTabIndex = 2; // switch to Stats tab
    }

    private void OnClockStarted()
    {
        var period = Game.GetCurrentPeriod();
        if (period.Status != PeriodStatus.Active)
        {
            if (period.PeriodNumber == 1)
            {
                GameState.TeamATotalTimeouts = GameState.TeamATimeOutsLeft = 2;
                GameState.TeamBTotalTimeouts = GameState.TeamBTimeOutsLeft = 2;
            }
            else if (period.PeriodNumber == 3)
            {
                GameState.TeamATotalTimeouts = GameState.TeamATimeOutsLeft = 3;
                GameState.TeamBTotalTimeouts = GameState.TeamBTimeOutsLeft = 3;
            }
            period.Status = PeriodStatus.Active;
            GameClockService.SetPeriodDisplay($"{period.Name} - {period.Status}");
            var startActions = new List<PlayActionViewModel>();
            if (period.PeriodNumber == 1)
            {
                startActions.Add(CreateGameAction(PlayType.GameStart));
                startActions.Add(CreateGameAction(PlayType.HalfStart));
            }
            else if (period.PeriodNumber == 3)
            {
                startActions.Add(CreateGameAction(PlayType.HalfStart));
            }
            startActions.Add(CreateGameAction(PlayType.PeriodStart));
            AddPlayCard(startActions);
        }
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

    private void BeginAssist()
    {
        ResetSelectionState();
        if (_pendingShooter == null)
        {
            IsAssistTeamSelectionActive = true;
        }
        else
        {
            IsAssistSelectionActive = true;
        }
    }

    private void SelectAssistTeam(bool teamA)
    {
        _assistTeamIsTeamA = teamA;
        IsAssistTeamSelectionActive = false;
        IsAssistSelectionActive = true;
    }

    private void SelectFreeThrowTeam(bool teamA)
    {
        _freeThrowTeamIsTeamA = teamA;
        IsFreeThrowTeamSelectionActive = false;
        BeginFreeThrowsAwardedSelection();
    }
    private void SelectStealTeam(bool teamA)
    {
        _stealTeamIsTeamA = teamA;
        IsStealTeamSelectionActive = false;
        IsStealSelectionActive = true;
    }

    private void BeginRebound()
    {
        var shooter = _pendingShooter;
        ResetSelectionState();

        if (_pendingFreeThrowRebound)
        {
            _pendingShooter = shooter;
        }

        IsReboundSelectionActive = true;
    }

    private void BeginSteal()
    {
        ResetSelectionState();
        if (_pendingShooter == null)
        {
            IsStealTeamSelectionActive = true;
        }
        else
        {
            _stealTeamIsTeamA = !_pendingShooter.IsTeamA;
            IsStealSelectionActive = true;
        }
    }

    private void BeginFreeThrows()
    {
        ResetSelectionState();
        if (_fouledPlayer == null)
        {
            IsFreeThrowTeamSelectionActive = true;
        }
        else
        {
            BeginFreeThrowsAwardedSelection();
        }
    }

    private void CompleteTimeoutSelection(string team)
    {
        Debug.WriteLine($"{GameClockService.TimeLeftString} Timeout called by {team}");
        IsTimeOutSelectionActive = false;
        var isTeamA = team == "Team A";
        UseTimeout(isTeamA);
        AddPlayCard(new[] { CreateTeamAction(isTeamA, "TIMEOUT") });
    }

    private void OnCoachTechnical(string team)
    {
        Debug.WriteLine($"Coach Technical on {team}");
        var isTeamA = team == "Team A";
        AddPlayCard(new[] { CreateTeamAction(isTeamA, "FOUL TECHNICAL") });
        GameState.AddFoul(isTeamA);
        var t = isTeamA ? Game.HomeTeam : Game.AwayTeam;
        _actionProcessor.ProcessTeam(ActionType.CoachFoul, t);
        StatsVM.Refresh();
        _defaultFreeThrows = 1;
        IsRebound = false;
        _freeThrowTeamIsTeamA = team != "Team A";
        BeginFreeThrowsAwardedSelection();
    }

    private void OnBenchTechnical(string team)
    {
        Debug.WriteLine($"Bench Technical on {team}");
        var isTeamA = team == "Team A";
        AddPlayCard(new[] { CreateTeamAction(isTeamA, "FOUL TECHNICAL") });
        GameState.AddFoul(isTeamA);
        var t = isTeamA ? Game.HomeTeam : Game.AwayTeam;
        _actionProcessor.ProcessTeam(ActionType.BenchFoul, t);
        StatsVM.Refresh();
        _defaultFreeThrows = 1;
        IsRebound = false;
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

    private void TeamPlayersChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RegenerateTeamsFromInfo();
    }

    private void PlayerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Player.IsPlaying))
        {
            RegenerateTeamsFromInfo();
        }
    }

    private void SubscribePlayerEvents()
    {
        foreach (var p in Game.HomeTeam.Players)
            p.PropertyChanged -= PlayerPropertyChanged;
        foreach (var p in Game.AwayTeam.Players)
            p.PropertyChanged -= PlayerPropertyChanged;
        foreach (var p in Game.HomeTeam.Players)
            p.PropertyChanged += PlayerPropertyChanged;
        foreach (var p in Game.AwayTeam.Players)
            p.PropertyChanged += PlayerPropertyChanged;
    }

    private void RegenerateTeams()
    {
        TeamAPlayers.Clear();
        TeamBPlayers.Clear();
        TeamACourtPlayers.Clear();
        TeamBCourtPlayers.Clear();
        TeamABenchPlayers.Clear();
        TeamBBenchPlayers.Clear();

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

        foreach (var p in teamA)
        {
            TeamAPlayers.Add(p);
            if (p.Player.IsActive)
                TeamACourtPlayers.Add(p.Player);
            else
                TeamABenchPlayers.Add(p.Player);
        }
        foreach (var p in teamB)
        {
            TeamBPlayers.Add(p);
            if (p.Player.IsActive)
                TeamBCourtPlayers.Add(p.Player);
            else
                TeamBBenchPlayers.Add(p.Player);
        }

        UpdateStartingFiveLists();
    }


    public void RegenerateTeamsFromInfo()
    {
        Players.Clear();

        var home = Game.HomeTeam.Players.Where(p => p.IsPlaying).ToList();
        var away = Game.AwayTeam.Players.Where(p => p.IsPlaying).ToList();

        foreach (var p in home)
        {
            p.IsTeamA = true;
            Players.Add(p);
        }
        foreach (var p in away)
        {
            p.IsTeamA = false;
            Players.Add(p);
        }
        /*
        int id = 1;
        foreach (var p in TeamAInfo.Players.Take(15))
        {
            p.IsTeamA = true;
            p.Id = id++;
            Players.Add(p);
        }

        id = 16;
        foreach (var p in TeamBInfo.Players.Take(15))
        {
            p.IsTeamA = false;
            p.Id = id++;
            Players.Add(p);
        }
        */
        RegenerateTeams();
        SubscribePlayerEvents();
        StatsVM?.Refresh();
        UpdateStartingFiveLists();
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
    private bool _isRebound = true;
    private bool _pendingFreeThrowRebound;
    private object? _pendingReboundSource;
    private bool? _pendingReboundOffensive;
    private int _selectedFreeThrowCount;
    private Player? _selectedFreeThrowShooter;
    private Player? _selectedFreeThrowAssist;
    private bool _assistTeamIsTeamA;
    private bool _stealTeamIsTeamA;
    private readonly List<PlayActionViewModel> _currentPlayActions = new();

    public bool IsActionInProgress =>
        _pendingShooter != null ||
        IsQuickShotSelectionActive ||
        IsBlockerSelectionActive ||
        IsSubstitutionPanelVisible ||
        IsStartingFivePanelVisible ||
        IsTimeOutSelectionActive ||
        IsJumpBallChoicePanelVisible ||
        IsJumpBallContestedPanelVisible ||
        IsJumpBallPanelVisible ||
        IsJumpWinnerPanelVisible ||
        IsTurnoverSelectionActive ||
        IsAssistSelectionActive ||
        IsAssistTeamSelectionActive ||
        IsReboundSelectionActive ||
        IsReboundTypeSelectionActive ||
        IsStealSelectionActive ||
        IsStealTeamSelectionActive ||
        IsFoulCommiterSelectionActive ||
        IsFoulTypeSelectionActive ||
        IsFouledPlayerSelectionActive ||
        IsFreeThrowTeamSelectionActive ||
        IsFreeThrowsAwardedSelectionActive ||
        IsFreeThrowsSelectionActive;

    private void NotifyActionInProgressChanged()
    {
        OnPropertyChanged(nameof(IsActionInProgress));
        CommandManager.InvalidateRequerySuggested();
    }

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

        if (_foulType?.ToLowerInvariant() == "double personal")
        {
            if (_fouledPlayer != null)
                _currentPlayActions.Add(CreateAction(_fouledPlayer, $"FOUL {_foulType.ToUpperInvariant()}"));

            if (_foulCommiter != null)
            {
                GameState.AddFoul(_foulCommiter.IsTeamA);
                _actionProcessor.Process(ActionType.Foul, _foulCommiter);
            }
            if (_fouledPlayer != null)
            {
                GameState.AddFoul(_fouledPlayer.IsTeamA);
                _actionProcessor.Process(ActionType.Foul, _fouledPlayer);
            }

            AddPlayCard(_currentPlayActions.ToList());
            _currentPlayActions.Clear();
            StatsVM.Refresh();
            ResetFoulState();
        }
        else
        {
            if (_fouledPlayer != null)
                _currentPlayActions.Add(CreateAction(_fouledPlayer, "FOULED"));

            if (_foulCommiter != null)
                GameState.AddFoul(_foulCommiter.IsTeamA);

            bool offensive = _foulType?.ToLowerInvariant() == "offensive";
            if (offensive && _foulCommiter != null)
            {
                _currentPlayActions.Add(CreateAction(_foulCommiter, "TURNOVER"));
            }

            AddPlayCard(_currentPlayActions.ToList());
            _currentPlayActions.Clear();

            if (offensive)
            {
                if (_foulCommiter != null)
                {
                    _actionProcessor.Process(ActionType.Turnover, _foulCommiter);
                    StatsVM.Refresh();
                }

                Debug.WriteLine($"{GameClockService.TimeLeftString} Offensive foul by {_foulCommiter?.Number}.{_foulCommiter?.Name} on {_fouledPlayer?.Number}.{_fouledPlayer?.Name} — no free throws");
                ResetFoulState();
            }
            else
            {
                BeginFreeThrowsAwardedSelection();
            }
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
        OnReboundTargetSelected(player);
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

    if (actionType == ActionButtonMode.Other)
    {
        MarkerRequested?.Invoke(position, Brushes.Transparent, false);
    }
    else
    {
        TempMarkerRemoved?.Invoke();
        Brush teamColor = GetTeamColorFromPlayer(player);
        bool isFilled = actionType == ActionButtonMode.Made;

        MarkerRequested?.Invoke(position, teamColor, isFilled);
    }

    Debug.WriteLine($"{GameClockService.TimeLeftString} Action '{SelectedAction}' by {player.Number}.{player.Name} at {position} ({actionType})");

    _pendingShooter = player;
    _pendingIsThreePoint = SelectedPoint.IsThreePoint;
    _wasBlocked = false;
    _blocker = null;
    _currentPlayActions.Clear();

    if (actionType == ActionButtonMode.Turnover)
    {
        _currentPlayActions.Add(CreateAction(player, "TURNOVER"));
        IsTurnoverSelectionActive = true;
    }
    else if (actionType == ActionButtonMode.Made)
    {
        IsAssistSelectionActive = true;
    }
    else if (actionType == ActionButtonMode.Missed)
    {
        IsReboundSelectionActive = true;
    }
    else
    {
        ResetSelectionState();
    }
}


    private void OnReboundTargetSelected(object reboundSource)
    {
        _pendingReboundSource = reboundSource;
        IsReboundSelectionActive = false;

        if (_pendingShooter == null && !_pendingFreeThrowRebound)
        {
            // Manual rebound flow – ask for offensive/defensive type
            IsReboundTypeSelectionActive = true;
        }
        else
        {
            // Rebound after a missed/blocked shot – use automatic logic
            CompleteReboundSelection(reboundSource);
            _pendingReboundSource = null;
        }
    }

    private void OnReboundTypeSelected(string? reboundType)
    {
        if (_pendingReboundSource == null || string.IsNullOrWhiteSpace(reboundType))
            return;

        _pendingReboundOffensive = reboundType.ToLowerInvariant() == "offensive";
        IsReboundTypeSelectionActive = false;

        CompleteReboundSelection(_pendingReboundSource);

        _pendingReboundSource = null;
        _pendingReboundOffensive = null;
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
        IsRebound = !(lowerType.Contains("technical") || lowerType.Contains("unsportsmanlike"));

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
                if (_foulCommiter != null)
                {
                    // Technical fouls award free throws to the opposing team.
                    _freeThrowTeamIsTeamA = !_foulCommiter.IsTeamA;
                }
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
        IsFreeThrowTeamSelectionActive = false;
        IsFreeThrowsAwardedSelectionActive = false;
        FreeThrowResultRows.Clear();
        IsFreeThrowsSelectionActive = false;
        SelectedFreeThrowCount = 0;
        SelectedFreeThrowShooter = null;
        SelectedFreeThrowAssist = null;
        _defaultFreeThrows = 0;
        _pendingFreeThrowRebound = false;
        IsRebound = false;
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
        IsAssistTeamSelectionActive = false;
        IsReboundSelectionActive = false;
        IsReboundTypeSelectionActive = false;
        _pendingReboundSource = null;
        _pendingReboundOffensive = null;
        IsTurnoverSelectionActive = false;
        IsStealSelectionActive = false;
        IsStealTeamSelectionActive = false;
        IsBlockerSelectionActive = false;
        IsQuickShotSelectionActive = false;
        IsJumpBallChoicePanelVisible = false;
        IsJumpBallContestedPanelVisible = false;
        IsTimeOutSelectionActive = false;
    }

    public void CancelCurrentAction()
    {
        if ((IsAssistSelectionActive && _pendingShooter != null) ||
            (IsReboundSelectionActive && _pendingShooter != null) ||
            (IsStealSelectionActive && _pendingShooter != null))
            return;

        TempMarkerRemoved?.Invoke();

        if (IsFoulCommiterSelectionActive || IsFoulTypeSelectionActive ||
            IsFouledPlayerSelectionActive || IsFreeThrowTeamSelectionActive ||
            IsFreeThrowsAwardedSelectionActive || IsFreeThrowsSelectionActive)
        {
            ResetFoulState();
        }
        else
        {
            ResetSelectionState();
        }
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
            RecordPoints(_pendingShooter, _pendingIsThreePoint ? 3 : 2, assistPlayer, _pendingIsThreePoint);
            AddPlayCard(_currentPlayActions.ToList());
            _currentPlayActions.Clear();
            GameClockService.SetPossession(!_pendingShooter.IsTeamA);
        }
        else if (assistPlayer != null)
        {
            Debug.WriteLine($"{GameClockService.TimeLeftString} Assist by {assistPlayer.Number}.{assistPlayer.Name}");
            AddPlayCard(new[] { CreateAction(assistPlayer, "ASSIST") });
            _actionProcessor.Process(ActionType.Assist, assistPlayer);
            StatsVM.Refresh();
        }

        _pendingShooter = null;
        SelectedAction = null;
        SelectedPoint = null;
        IsAssistSelectionActive = false;
        IsAssistTeamSelectionActive = false;
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
                null => "No rebound",
                _ => "Unknown rebound result"
            };

            Debug.WriteLine($"{GameClockService.TimeLeftString} {log} after miss by {_pendingShooter.Number}.{_pendingShooter.Name}");
            if (reboundSource is Player rp)
            {
                _currentPlayActions.Add(CreateAction(rp, "REBOUND"));
                bool offensive = _pendingReboundOffensive ?? (rp.IsTeamA == _pendingShooter.IsTeamA);
                _actionProcessor.Process(offensive ? ActionType.OffensiveRebound : ActionType.DefensiveRebound, rp);
                StatsVM.Refresh();
                GameClockService.SetPossession(rp.IsTeamA);
            }
            else if (reboundSource is string team && (team == "TeamA" || team == "TeamB"))
            {
                bool teamA = team == "TeamA";
                _currentPlayActions.Add(CreateTeamAction(teamA, "REBOUND"));
                bool offensive = _pendingReboundOffensive ?? (_pendingShooter != null && (_pendingShooter.IsTeamA == teamA));
                var t = teamA ? Game.HomeTeam : Game.AwayTeam;
                _actionProcessor.ProcessTeam(ActionType.TeamRebound, t, offensive);
                StatsVM.Refresh();
                GameClockService.SetPossession(teamA);
            }
            if (_pendingFreeThrowRebound)
            {
                AddPlayCard(_currentPlayActions.ToList());
                _currentPlayActions.Clear();
                _pendingFreeThrowRebound = false;
                ResetSelectionState();
                return;
            }

            var shot = FormatShotAction(_pendingIsThreePoint, _wasBlocked ? "BLOCKED" : "MISSED");
            _currentPlayActions.Insert(0, CreateAction(_pendingShooter, shot));
            _actionProcessor.Process(ActionType.ShotMissed, _pendingShooter, null, _pendingIsThreePoint);

            if (reboundSource is string r && r == "24")
            {
                bool teamA = _pendingShooter.IsTeamA;
                _currentPlayActions.Add(CreateTeamAction(teamA, "TURNOVER"));
                var team = teamA ? Game.HomeTeam : Game.AwayTeam;
                _actionProcessor.ProcessTeam(ActionType.TeamTurnover, team);
            }

            StatsVM.Refresh();
            AddPlayCard(_currentPlayActions.ToList());
            _currentPlayActions.Clear();
        }
        else
        {
            if (reboundSource is Player rp)
            {
                _currentPlayActions.Add(CreateAction(rp, "REBOUND"));
                bool offensive = _pendingReboundOffensive ?? false;
                _actionProcessor.Process(offensive ? ActionType.OffensiveRebound : ActionType.DefensiveRebound, rp);
                StatsVM.Refresh();
                GameClockService.SetPossession(rp.IsTeamA);
            }
            else if (reboundSource is string team && (team == "TeamA" || team == "TeamB"))
            {
                bool teamA = team == "TeamA";
                _currentPlayActions.Add(CreateTeamAction(teamA, "REBOUND"));
                bool offensive = _pendingReboundOffensive ?? false;
                var t = teamA ? Game.HomeTeam : Game.AwayTeam;
                _actionProcessor.ProcessTeam(ActionType.TeamRebound, t, offensive);
                StatsVM.Refresh();
                GameClockService.SetPossession(teamA);
            }
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
            var t = teamA ? Game.HomeTeam : Game.AwayTeam;
            if (t != null)
            {
                _actionProcessor.ProcessTeam(ActionType.TeamTurnover, t);
                StatsVM.Refresh();
            }
            GameClockService.SetPossession(!teamA);
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
        // Standalone steal flow when there is no pending turnover
        if (_pendingShooter == null)
        {
            if (stealer != null)
            {
                Debug.WriteLine($"{GameClockService.TimeLeftString} Steal by {stealer.Number}.{stealer.Name}");
                AddPlayCard(new[] { CreateAction(stealer, "STEAL") });
                _actionProcessor.Process(ActionType.Steal, stealer);
                StatsVM.Refresh();
            }
            ResetSelectionState();
            return;
        }

        if (stealer == null)
        {
            Debug.WriteLine($"{GameClockService.TimeLeftString} No steal awarded on turnover by {_pendingShooter.Number}.{_pendingShooter.Name}");
            _actionProcessor.Process(ActionType.Turnover, _pendingShooter);
            AddPlayCard(_currentPlayActions.ToList());
            _currentPlayActions.Clear();
            StatsVM.Refresh();
            GameClockService.SetPossession(!_pendingShooter.IsTeamA);
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
        _actionProcessor.Process(ActionType.Turnover, _pendingShooter);
        _actionProcessor.Process(ActionType.Steal, stealer);
        AddPlayCard(_currentPlayActions.ToList());
        _currentPlayActions.Clear();
        StatsVM.Refresh();
        GameClockService.SetPossession(stealer.IsTeamA);
        ResetSelectionState();
    }

    private Brush GetTeamColorFromPlayer(Player player)
    {
        return player.IsTeamA
            ? (Brush)_resources["PrimaryAColor"]
            : (Brush)_resources["PrimaryBColor"];
    }

    private ActionButtonMode GetActionType(string action) => action.ToUpperInvariant() switch
    {
        "MADE" => ActionButtonMode.Made,
        "MISSED" => ActionButtonMode.Missed,
        "TURNOVER" => ActionButtonMode.Turnover,
        _ => ActionButtonMode.Other
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

    private bool _isAssistTeamSelectionActive;
    public bool IsAssistTeamSelectionActive
    {
        get => _isAssistTeamSelectionActive;
        set
        {
            _isAssistTeamSelectionActive = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(AssistTeamPanelVisibility));
            NotifyActionInProgressChanged();
        }
    }

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
            NotifyActionInProgressChanged();
        }
    }

    public Visibility AssistTeamPanelVisibility =>
        IsAssistTeamSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    private bool _isFreeThrowTeamSelectionActive;
    public bool IsFreeThrowTeamSelectionActive
    {
        get => _isFreeThrowTeamSelectionActive;
        set
        {
            _isFreeThrowTeamSelectionActive = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FreeThrowTeamPanelVisibility));
            NotifyActionInProgressChanged();
        }
    }

    public Visibility FreeThrowTeamPanelVisibility =>
        IsFreeThrowTeamSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    private void UpdateAssistPlayerStyles()
    {
        EligibleCourtAssistPlayers.Clear();
        EligibleBenchAssistPlayers.Clear();

        if (_pendingShooter == null)
        {
            if (!IsAssistSelectionActive)
            {
                OnPropertyChanged(nameof(EligibleCourtAssistPlayers));
                OnPropertyChanged(nameof(EligibleBenchAssistPlayers));
                return;
            }

            var playerList = _assistTeamIsTeamA ? TeamAPlayers : TeamBPlayers;
            foreach (var vm in playerList)
            {
                vm.SetAssistSelectionMode(IsAssistSelectionActive);
                vm.IsSelectedAsAssist = false;
                if (vm.Player.IsActive)
                    EligibleCourtAssistPlayers.Add(vm);
                else
                    EligibleBenchAssistPlayers.Add(vm);
            }

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
            NotifyActionInProgressChanged();
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

    private bool _isReboundTypeSelectionActive;
    public bool IsReboundTypeSelectionActive
    {
        get => _isReboundTypeSelectionActive;
        set
        {
            _isReboundTypeSelectionActive = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ReboundTypePanelVisibility));
            NotifyActionInProgressChanged();
        }
    }

    public Visibility ReboundTypePanelVisibility =>
        IsReboundTypeSelectionActive ? Visibility.Visible : Visibility.Collapsed;

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
            NotifyActionInProgressChanged();
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
            NotifyActionInProgressChanged();
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
            NotifyActionInProgressChanged();
        }
    }

    public Visibility StealPanelVisibility =>
        IsStealSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    private bool _isStealTeamSelectionActive;
    public bool IsStealTeamSelectionActive
    {
        get => _isStealTeamSelectionActive;
        set
        {
            _isStealTeamSelectionActive = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StealTeamPanelVisibility));
            NotifyActionInProgressChanged();
        }
    }

    public Visibility StealTeamPanelVisibility =>
        IsStealTeamSelectionActive ? Visibility.Visible : Visibility.Collapsed;

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
            NotifyActionInProgressChanged();
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

            if (IsStealSelectionActive)
            {
                if (_pendingShooter != null)
                {
                    isSelectable = vm.Player.IsTeamA != _pendingShooter.IsTeamA;
                }
                else
                {
                    isSelectable = vm.Player.IsTeamA == _stealTeamIsTeamA;
                }
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
            NotifyActionInProgressChanged();
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
            NotifyActionInProgressChanged();
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
            NotifyActionInProgressChanged();
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
            NotifyActionInProgressChanged();
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
            NotifyActionInProgressChanged();
        }
    }

    public bool IsRebound
    {
        get => _isRebound;
        set
        {
            _isRebound = value;
            OnPropertyChanged();
        }
    }

    public Visibility FreeThrowsPanelVisibility =>
        IsFreeThrowsSelectionActive ? Visibility.Visible : Visibility.Collapsed;

    private void StartFreeThrows(int count)
    {
        IsFreeThrowsAwardedSelectionActive = false;
        SelectedFreeThrowAssist = null;
        _pendingFreeThrowRebound = false;

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

        IsFreeThrowsSelectionActive = true;

        UpdateAssistPlayerStyles();
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

            var actions = new List<PlayActionViewModel>();
            if (_pendingShooter != null)
            {
                Debug.WriteLine($"{GameClockService.TimeLeftString} Free throws by {_pendingShooter.Number}.{_pendingShooter.Name}: {made} made, {missed} missed");
                if (made > 0 && SelectedFreeThrowAssist != null)
                {
                    Debug.WriteLine($"{GameClockService.TimeLeftString} Assist by {SelectedFreeThrowAssist.Number}.{SelectedFreeThrowAssist.Name}");
                }
                foreach (var r in FreeThrowResultRows)
                {
                    actions.Add(CreateAction(_pendingShooter, $"FTA {r.Result}"));
                }
                if (made > 0 && SelectedFreeThrowAssist != null)
                {
                    actions.Add(CreateAction(SelectedFreeThrowAssist, "ASSIST"));
                }
                if (made > 0 && _pendingShooter != null)
                {
                    for (int i = 0; i < made; i++)
                        _actionProcessor.Process(ActionType.FreeThrowMade, _pendingShooter);
                    GameState.TeamAScore = Game.HomeTeam.Points;
                    GameState.TeamBScore = Game.AwayTeam.Points;
                    StatsVM.Refresh();
                }
            }

            var lastResult = FreeThrowResultRows.Last().Result;
            AddPlayCard(actions);

            if (IsRebound && lastResult == "MISSED")
            {
                var shooter = _pendingShooter;
                ResetFoulState();
                _pendingShooter = shooter;
                _pendingFreeThrowRebound = true;
                BeginRebound();
            }
            else
            {
                ResetFoulState();
            }
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
            ? (Brush)_resources["PrimaryAColor"]
            : (Brush)_resources["PrimaryBColor"];
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

    private void ToggleSubOutAll(bool isTeamA)
    {
        var targetList = isTeamA ? TeamASubOut : TeamBSubOut;
        var courtPlayers = isTeamA ? TeamACourtPlayers : TeamBCourtPlayers;

        if (targetList.Count == courtPlayers.Count && courtPlayers.All(p => targetList.Contains(p)))
        {
            targetList.Clear();
        }
        else
        {
            targetList.Clear();
            foreach (var p in courtPlayers)
                targetList.Add(p);
        }

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

    private void BeginStartingFive()
    {
        UpdateStartingFiveLists();
        IsStartingFivePanelVisible = true;
    }

    private void ToggleStartingFive(Player? player)
    {
        if (player == null) return;
        var list = player.IsTeamA ? TeamAStartingFivePlayers : TeamBStartingFivePlayers;

        if (player.S5)
        {
            player.S5 = false;
        }
        else if (list.Count < 5)
        {
            player.S5 = true;
        }

        UpdateStartingFiveLists();
        OnPropertyChanged(nameof(IsStartingFiveConfirmEnabled));
    }

    public bool IsStartingFiveConfirmEnabled =>
        TeamAStartingFivePlayers.Count == 5 && TeamBStartingFivePlayers.Count == 5;

    private void ConfirmStartingFive()
    {
        foreach (var p in Players)
            p.IsActive = p.S5;

        RegenerateTeams();
        IsStartingFivePanelVisible = false;
        OnPropertyChanged(nameof(IsStartingFiveConfirmEnabled));
        StatsVM.Refresh();
        BeginJumpBall();
    }

    private void UpdateStartingFiveLists()
    {
        TeamAStartingFivePlayers.Clear();
        TeamAStartingBenchPlayers.Clear();
        TeamBStartingFivePlayers.Clear();
        TeamBStartingBenchPlayers.Clear();

        foreach (var p in Players)
        {
            if (p.IsTeamA)
            {
                if (p.S5)
                    TeamAStartingFivePlayers.Add(p);
                else
                    TeamAStartingBenchPlayers.Add(p);
            }
            else
            {
                if (p.S5)
                    TeamBStartingFivePlayers.Add(p);
                else
                    TeamBStartingBenchPlayers.Add(p);
            }
        }

        OnPropertyChanged(nameof(TeamAStartingFivePlayers));
        OnPropertyChanged(nameof(TeamAStartingBenchPlayers));
        OnPropertyChanged(nameof(TeamBStartingFivePlayers));
        OnPropertyChanged(nameof(TeamBStartingBenchPlayers));
    }

    private void BeginJumpBallChoice()
    {
        ResetSelectionState();
        IsJumpBallChoicePanelVisible = true;
    }

    private void OnJumpBallSelected()
    {
        IsJumpBallChoicePanelVisible = false;
        BeginJumpBall(true);
    }

    private void OnContestSelected()
    {
        IsJumpBallChoicePanelVisible = false;
        IsJumpBallContestedPanelVisible = true;
    }

    private void CompleteContestedRebound()
    {
        IsJumpBallContestedPanelVisible = false;
        bool arrowA = GameClockService.TeamAArrow;
        bool possA = GameClockService.TeamAPossession;

        if (arrowA == possA)
        {
            GameClockService.SwapArrow();
        }
        else
        {
            GameClockService.SwapPossessionAndArrow();
            var teamA = arrowA;
            _actionProcessor.ProcessTeam(ActionType.TeamRebound, teamA ? Game.HomeTeam : Game.AwayTeam);
            AddPlayCard(new[] { CreateTeamAction(teamA, "REBOUND") });
            StatsVM.Refresh();
        }
    }

    private void CompleteContestedTurnover()
    {
        IsJumpBallContestedPanelVisible = false;
        bool arrowA = GameClockService.TeamAArrow;
        bool possA = GameClockService.TeamAPossession;

        if (arrowA == possA)
        {
            GameClockService.SwapArrow();
        }
        else
        {
            GameClockService.SwapPossessionAndArrow();
            var teamA = possA;
            _actionProcessor.ProcessTeam(ActionType.TeamTurnover, teamA ? Game.HomeTeam : Game.AwayTeam);
            AddPlayCard(new[] { CreateTeamAction(teamA, "TURNOVER") });
            StatsVM.Refresh();
        }
    }

    private bool _inGameJumpBall;
    private void BeginJumpBall(bool inGame = false)
    {
        _inGameJumpBall = inGame;
        TeamAJumpingPlayers.Clear();
        TeamBJumpingPlayers.Clear();
        IsJumpBallPanelVisible = true;
        OnPropertyChanged(nameof(IsJumpEnabled));
    }

    private void ToggleJumpPlayer(Player? player)
    {
        if (player == null) return;
        var list = player.IsTeamA ? TeamAJumpingPlayers : TeamBJumpingPlayers;

        if (list.Contains(player))
            list.Remove(player);
        else
        {
            list.Clear();
            list.Add(player);
        }

        OnPropertyChanged(nameof(IsJumpEnabled));
    }

    private void ConfirmJumpBall()
    {
        IsJumpBallPanelVisible = false;
        IsJumpWinnerPanelVisible = true;
        IsStartingFiveButtonEnabled = false;
        GameClockService.Start();
    }

    private void CompleteJumpBall(bool teamAWon)
    {
        var winner = teamAWon ? TeamAJumpingPlayers.FirstOrDefault() : TeamBJumpingPlayers.FirstOrDefault();
        var loser = teamAWon ? TeamBJumpingPlayers.FirstOrDefault() : TeamAJumpingPlayers.FirstOrDefault();

        if (_inGameJumpBall)
        {
            bool possessionTeamA = GameClockService.TeamAPossession;
            if (possessionTeamA == teamAWon)
            {
                if (winner != null)
                    AddPlayCard(new[] { CreateAction(winner, "JUMP BALL WON") });
                GameClockService.SetArrow(!teamAWon);
            }
            else
            {
                if (loser != null)
                {
                    AddPlayCard(new[]
                    {
                        CreateAction(loser, "TURNOVER"),
                        winner != null ? CreateAction(winner, "STEAL") : CreateTeamAction(teamAWon, "STEAL")
                    });
                    _actionProcessor.Process(ActionType.Turnover, loser);
                    if (winner != null)
                        _actionProcessor.Process(ActionType.Steal, winner);
                    StatsVM.Refresh();
                }
                GameClockService.SetPossession(teamAWon);
                GameClockService.SetArrow(!teamAWon);
            }
        }
        else
        {
            AddPlayCard(new[]
            {
                CreateTeamAction(teamAWon, "JUMP BALL WON"),
                CreateTeamAction(!teamAWon, "JUMP BALL LOST")
            });
            GameClockService.SetPossession(teamAWon);
            GameClockService.SetArrow(!teamAWon);
        }

        GameClockService.SetPossession(teamAWon);
        GameClockService.SetArrow(!teamAWon);

        TeamAJumpingPlayers.Clear();
        TeamBJumpingPlayers.Clear();
        IsJumpWinnerPanelVisible = false;
        _inGameJumpBall = false;
        OnPropertyChanged(nameof(IsJumpEnabled));
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

    private PlayActionViewModel CreateGameAction(PlayType action)
    {
        Debug.WriteLine($"CreateGameAction: {action}");
        string text = action switch
        {
            PlayType.GameStart => "GAME START",
            PlayType.GameEnd => "GAME END",
            PlayType.PeriodStart => "PERIOD START",
            PlayType.PeriodEnd => "PERIOD END",
            PlayType.HalfStart => "HALF START",
            PlayType.HalfEnd => "HALF END",
            _ => action.ToString().ToUpperInvariant()
        };
        return new PlayActionViewModel
        {
            TeamColor = Brushes.Gray,
            PlayerNumber = string.Empty,
            FirstName = string.Empty,
            LastName = string.Empty,
            Action = text
        };
    }

    private void RecordPoints(Player shooter, int points, Player? assist = null, bool isThreePoint = false)
    {
        _actionProcessor.Process(ActionType.ShotMade, shooter, assist, points == 3 || isThreePoint);
        GameState.TeamAScore = Game.HomeTeam.Points;
        GameState.TeamBScore = Game.AwayTeam.Points;
        StatsVM.Refresh();
        GameClockService.SetPossession(!shooter.IsTeamA);
    }

    private void UseTimeout(bool teamA)
    {
        var period = Game.GetCurrentPeriod();
        if (teamA)
        {
            if (GameState.TeamATimeOutsLeft == 0) return;
            Game.HomeTeam.AddTimeout(period);
            GameState.TeamATimeOutsLeft = Math.Max(0, GameState.TeamATimeOutsLeft - 1);
        }
        else
        {
            if (GameState.TeamBTimeOutsLeft == 0) return;
            Game.AwayTeam.AddTimeout(period);
            GameState.TeamBTimeOutsLeft = Math.Max(0, GameState.TeamBTimeOutsLeft - 1);
        }
    }

    private void AddPlayCard(IEnumerable<PlayActionViewModel> actions)
    {
        PlayLog.AddCard(
            Game.GetCurrentPeriod().Name,
            GameClockService.TimeLeftString,
            GameState.TeamAScore,
            GameState.TeamBScore,
            actions);
    }

    private void SwapSides()
    {
        var temp = Game.HomeTeam;
        Game.HomeTeam = Game.AwayTeam;
        Game.AwayTeam = temp;

        Game.HomeTeam.IsHomeTeam = true;
        Game.AwayTeam.IsHomeTeam = false;

        var nameTmp = TeamAName;
        TeamAName = TeamBName;
        TeamBName = nameTmp;

        var shortTmp = TeamAShortName;
        TeamAShortName = TeamBShortName;
        TeamBShortName = shortTmp;

        var colorTmp = TeamAColorOption;
        TeamAColorOption = TeamBColorOption;
        TeamBColorOption = colorTmp;

        // notify in case other view models cache the current brushes so
        // the updated colors propagate everywhere
        OnPropertyChanged(nameof(TeamAColorOption));
        OnPropertyChanged(nameof(TeamBColorOption));

        // ensure team-specific stats follow the teams after swapping
        (GameState.TeamAScore, GameState.TeamBScore) = (GameState.TeamBScore, GameState.TeamAScore);
        (GameState.TeamATimeOutsLeft, GameState.TeamBTimeOutsLeft) = (GameState.TeamBTimeOutsLeft, GameState.TeamATimeOutsLeft);
        (GameState.TeamATotalTimeouts, GameState.TeamBTotalTimeouts) = (GameState.TeamBTotalTimeouts, GameState.TeamATotalTimeouts);
        (GameState.TeamAFouls, GameState.TeamBFouls) = (GameState.TeamBFouls, GameState.TeamAFouls);

        RegenerateTeamsFromInfo();
        StatsVM.Refresh();
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
        RecordPoints(shooter, 2);
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

        UseTimeout(true);
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
