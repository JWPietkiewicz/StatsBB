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
        
        // Subscribe to game clock events
        GameClockService.TimeUpdated += UpdateDisplay;
        
        // Ensure the display is updated when the control is loaded
        Loaded += (s, e) => UpdateDisplay();
        
        // Clean up when unloaded
        Unloaded += (s, e) => GameClockService.TimeUpdated -= UpdateDisplay;
        
        // Initial display update
        UpdateDisplay();
    }

    private void StartStop_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Only toggle if the button is enabled
            if (GameClockService.StartStopEnabled)
            {
                GameClockService.Toggle();
                // Force update display after toggle
                System.Windows.Threading.Dispatcher.CurrentDispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                    new System.Action(() => UpdateDisplay())
                );
            }
        }
        catch (System.Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GameClock StartStop_Click error: {ex.Message}");
        }
    }

    private void AddMinute_Click(object sender, RoutedEventArgs e) => GameClockService.AddMinute();
    private void SubtractMinute_Click(object sender, RoutedEventArgs e) => GameClockService.SubtractMinute();
    private void AddSecond_Click(object sender, RoutedEventArgs e) => GameClockService.AddSecond();
    private void SubtractSecond_Click(object sender, RoutedEventArgs e) => GameClockService.SubtractSecond();

    private void UpdateDisplay()
    {
        try
        {
            TimeText.Text = GameClockService.TimeLeftString;
            PeriodText.Text = GameClockService.Period;
            StartStopButton.Content = GameClockService.StartStopLabel;
            StartStopButton.IsEnabled = GameClockService.StartStopEnabled;
            
            // Update button colors based on state - safer resource lookup
            if (GameClockService.IsRunning)
            {
                // Clock is running - show green (success) color
                StartStopButton.Background = TryFindResource("SuccessColor") as SolidColorBrush ?? 
                    new SolidColorBrush(System.Windows.Media.Color.FromRgb(16, 124, 16)); // fallback green
            }
            else if (GameClockService.StartStopEnabled)
            {
                // Clock is stopped but can be started - show red (error) color
                StartStopButton.Background = TryFindResource("ErrorColor") as SolidColorBrush ?? 
                    new SolidColorBrush(System.Windows.Media.Color.FromRgb(164, 38, 44)); // fallback red
            }
            else
            {
                // Button is disabled - show disabled color
                StartStopButton.Background = TryFindResource("DisabledColor") as SolidColorBrush ?? 
                    new SolidColorBrush(System.Windows.Media.Color.FromRgb(243, 242, 241)); // fallback gray
            }

            // Update possession arrows - fix the logic
            ArrowAText.Visibility = GameClockService.TeamAArrow ? Visibility.Visible : Visibility.Collapsed;
            ArrowBText.Visibility = !GameClockService.TeamAArrow ? Visibility.Visible : Visibility.Collapsed;
            
            // Show possession indicators
            PossessionAText.Visibility = GameClockService.TeamAPossession ? Visibility.Visible : Visibility.Collapsed;
            PossessionBText.Visibility = !GameClockService.TeamAPossession ? Visibility.Visible : Visibility.Collapsed;
        }
        catch (System.Exception ex)
        {
            // Fallback if resource lookup fails
            System.Diagnostics.Debug.WriteLine($"GameClock UpdateDisplay error: {ex.Message}");
            StartStopButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(243, 242, 241));
        }
    }

    public void Toggle() => StartStop_Click(this, new RoutedEventArgs());
}
