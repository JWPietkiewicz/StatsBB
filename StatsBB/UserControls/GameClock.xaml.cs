using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using StatsBB.Services;

namespace StatsBB.UserControls;

public partial class GameClock : UserControl
{
    public GameClock()
    {
        InitializeComponent();
        GameClockService.TimeUpdated += UpdateDisplay;
        UpdateDisplay();
    }

    private void StartStop_Click(object sender, RoutedEventArgs e)
    {
        GameClockService.Toggle();
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        TimeText.Text = GameClockService.TimeLeftString;
        PeriodText.Text = GameClockService.Period;
        StartStopButton.Content = GameClockService.IsRunning ? "STOP" : "START";
        StartStopButton.Background = GameClockService.IsRunning ? Brushes.Green : Brushes.Red;
    }

    public void Toggle() => StartStop_Click(this, new RoutedEventArgs());
}
