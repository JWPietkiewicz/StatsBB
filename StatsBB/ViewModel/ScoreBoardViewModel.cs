using System.ComponentModel;
using System.Windows.Media;
using StatsBB.MVVM;

namespace StatsBB.ViewModel;

/// <summary>
/// View model used by the scoreboard view. It exposes
/// team names along with score and timeout information
/// mirrored from <see cref="GameStateViewModel"/>.
/// </summary>
public class ScoreBoardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;

    public ScoreBoardViewModel(MainWindowViewModel main)
    {
        _main = main;
        _main.PropertyChanged += MainOnPropertyChanged;
        _main.GameState.PropertyChanged += GameStateOnPropertyChanged;
    }

    private void MainOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainWindowViewModel.TeamAName))
            OnPropertyChanged(nameof(TeamAName));
        else if (e.PropertyName == nameof(MainWindowViewModel.TeamBName))
            OnPropertyChanged(nameof(TeamBName));
        else if (e.PropertyName == nameof(MainWindowViewModel.TeamAColorOption))
            OnPropertyChanged(nameof(TeamAColor));
        else if (e.PropertyName == nameof(MainWindowViewModel.TeamBColorOption))
            OnPropertyChanged(nameof(TeamBColor));
    }

    private void GameStateOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(GameStateViewModel.TeamAScore):
                OnPropertyChanged(nameof(TeamAScore));
                break;
            case nameof(GameStateViewModel.TeamBScore):
                OnPropertyChanged(nameof(TeamBScore));
                break;
            case nameof(GameStateViewModel.TeamATimeoutsText):
                OnPropertyChanged(nameof(TeamATimeoutsText));
                break;
            case nameof(GameStateViewModel.TeamBTimeoutsText):
                OnPropertyChanged(nameof(TeamBTimeoutsText));
                break;
            case nameof(GameStateViewModel.TeamAFouls):
                OnPropertyChanged(nameof(TeamAFouls));
                break;
            case nameof(GameStateViewModel.TeamBFouls):
                OnPropertyChanged(nameof(TeamBFouls));
                break;
            case nameof(GameStateViewModel.TeamAPeriodFouls):
                OnPropertyChanged(nameof(TeamAPeriodFouls));
                break;
            case nameof(GameStateViewModel.TeamBPeriodFouls):
                OnPropertyChanged(nameof(TeamBPeriodFouls));
                break;
        }
    }

    public int TeamAScore => _main.GameState.TeamAScore;
    public int TeamBScore => _main.GameState.TeamBScore;
    public string TeamATimeoutsText => _main.GameState.TeamATimeoutsText;
    public string TeamBTimeoutsText => _main.GameState.TeamBTimeoutsText;
    public int TeamAFouls => _main.GameState.TeamAFouls;
    public int TeamBFouls => _main.GameState.TeamBFouls;
    public int TeamAPeriodFouls => _main.GameState.TeamAPeriodFouls;
    public int TeamBPeriodFouls => _main.GameState.TeamBPeriodFouls;

    public Brush TeamAColor => _main.TeamAColorOption?.ColorBrush ?? Brushes.Orange;
    public Brush TeamBColor => _main.TeamBColorOption?.ColorBrush ?? Brushes.Green;

    public string TeamAName
    {
        get => _main.TeamAName;
        set => _main.TeamAName = value;
    }

    public string TeamBName
    {
        get => _main.TeamBName;
        set => _main.TeamBName = value;
    }
}

