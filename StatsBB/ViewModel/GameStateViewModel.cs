using StatsBB.MVVM;
using System.Windows.Input;

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
            CommandManager.InvalidateRequerySuggested();
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
            CommandManager.InvalidateRequerySuggested();
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
            CommandManager.InvalidateRequerySuggested();
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
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public string TeamBTimeoutsText => $"{TeamBTimeOutsLeft}/{TeamBTotalTimeouts}";

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
            TeamAFouls++;
        else
            TeamBFouls++;
    }
}

