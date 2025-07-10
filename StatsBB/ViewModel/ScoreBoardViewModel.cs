using System.ComponentModel;
using StatsBB.MVVM;

namespace StatsBB.ViewModel;

/// <summary>
/// View model used by the scoreboard view. It simply exposes
/// team names and the current <see cref="GameStateViewModel"/>.
/// </summary>
public class ScoreBoardViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;

    public ScoreBoardViewModel(MainWindowViewModel main)
    {
        _main = main;
        _main.PropertyChanged += MainOnPropertyChanged;
    }

    private void MainOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainWindowViewModel.TeamAName))
            OnPropertyChanged(nameof(TeamAName));
        else if (e.PropertyName == nameof(MainWindowViewModel.TeamBName))
            OnPropertyChanged(nameof(TeamBName));
    }

    public GameStateViewModel GameState => _main.GameState;

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
