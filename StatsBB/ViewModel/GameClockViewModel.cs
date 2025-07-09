using System.Windows.Input;
using System.Windows.Media;
using StatsBB.MVVM;
using StatsBB.Services;

namespace StatsBB.ViewModel;

public class GameClockViewModel : ViewModelBase
{
    public GameClockViewModel()
    {
        ToggleCommand = new RelayCommand(_ => Toggle());
        GameClockService.TimeUpdated += OnTimeUpdated;
    }

    public string Time => GameClockService.TimeLeftString;
    public string Period => GameClockService.Period;

    public string StartStopText => GameClockService.IsRunning ? "STOP" : "START";
    public Brush StartStopBrush => GameClockService.IsRunning ? Brushes.Green : Brushes.Red;

    public ICommand ToggleCommand { get; }

    private void Toggle()
    {
        GameClockService.Toggle();
        OnPropertyChanged(nameof(StartStopText));
        OnPropertyChanged(nameof(StartStopBrush));
    }

    private void OnTimeUpdated()
    {
        OnPropertyChanged(nameof(Time));
        OnPropertyChanged(nameof(Period));
    }
}
