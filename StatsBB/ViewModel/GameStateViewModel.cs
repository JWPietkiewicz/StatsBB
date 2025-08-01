using StatsBB.MVVM;

namespace StatsBB.ViewModel;

/// <summary>
/// Holds basic game state information such as scores and timeouts for both teams.
/// This logic was previously part of <see cref="MainWindowViewModel"/> but was
/// extracted to keep the main view model smaller and focused on UI coordination.
/// </summary>
public class GameStateViewModel : ViewModelBase
{
    private int _teamAScore;
    public int TeamAScore
    {
        get => _teamAScore;
        set
        {
            if (_teamAScore == value) return;
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
            if (_teamBScore == value) return;
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
            if (_teamATimeOutsLeft == value) return;
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
            if (_teamATotalTimeouts == value) return;
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
            if (_teamBTimeOutsLeft == value) return;
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
            if (_teamBTotalTimeouts == value) return;
            _teamBTotalTimeouts = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TeamBTimeoutsText));
        }
    }

    public string TeamBTimeoutsText => $"{TeamBTimeOutsLeft}/{TeamBTotalTimeouts}";

    private int _teamAPeriodFouls;
    /// <summary>
    /// Fouls committed by Team A in the current period.
    /// </summary>
    public int TeamAPeriodFouls
    {
        get => _teamAPeriodFouls;
        set
        {
            if (_teamAPeriodFouls == value) return;
            _teamAPeriodFouls = value;
            OnPropertyChanged();
        }
    }

    private int _teamAFouls;
    public int TeamAFouls
    {
        get => _teamAFouls;
        set
        {
            if (_teamAFouls == value) return;
            _teamAFouls = value;
            OnPropertyChanged();
        }
    }

    private int _teamBPeriodFouls;
    /// <summary>
    /// Fouls committed by Team B in the current period.
    /// </summary>
    public int TeamBPeriodFouls
    {
        get => _teamBPeriodFouls;
        set
        {
            if (_teamBPeriodFouls == value) return;
            _teamBPeriodFouls = value;
            OnPropertyChanged();
        }
    }

    private int _teamBFouls;
    public int TeamBFouls
    {
        get => _teamBFouls;
        set
        {
            if (_teamBFouls == value) return;
            _teamBFouls = value;
            OnPropertyChanged();
        }
    }

    public void AddFoul(bool teamA)
    {
        if (teamA)
        {
            TeamAFouls++;
            TeamAPeriodFouls++;
        }
        else
        {
            TeamBFouls++;
            TeamBPeriodFouls++;
        }
    }

    /// <summary>
    /// Reset the period foul counters to zero.
    /// Call this when a new period begins.
    /// </summary>
    public void ResetPeriodFouls()
    {
        TeamAPeriodFouls = 0;
        TeamBPeriodFouls = 0;
    }
}

