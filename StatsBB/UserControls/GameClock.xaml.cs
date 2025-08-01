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

    private void AddMinute_Click(object sender, RoutedEventArgs e) => GameClockService.AddMinute();
    private void SubtractMinute_Click(object sender, RoutedEventArgs e) => GameClockService.SubtractMinute();
    private void AddSecond_Click(object sender, RoutedEventArgs e) => GameClockService.AddSecond();
    private void SubtractSecond_Click(object sender, RoutedEventArgs e) => GameClockService.SubtractSecond();

    private void UpdateDisplay()
    {
        TimeText.Text = GameClockService.TimeLeftString;
        PeriodText.Text = GameClockService.Period;
        StartStopButton.Content = GameClockService.StartStopLabel;
        StartStopButton.IsEnabled = GameClockService.StartStopEnabled;
        StartStopButton.Background = GameClockService.IsRunning
            ? Brushes.Green
            : GameClockService.StartStopEnabled ? Brushes.Red : Brushes.LightGray;

        ArrowAText.Visibility = GameClockService.TeamAArrow ? Visibility.Visible : Visibility.Collapsed;
        ArrowBText.Visibility = GameClockService.TeamAArrow ? Visibility.Collapsed : Visibility.Visible;
        PossessionAText.Visibility = GameClockService.TeamAPossession ? Visibility.Visible : Visibility.Collapsed;
        PossessionBText.Visibility = GameClockService.TeamAPossession ? Visibility.Collapsed : Visibility.Visible;
    }

    public void Toggle() => StartStop_Click(this, new RoutedEventArgs());
}
