using System.Collections.ObjectModel;
using System.Windows.Media;

namespace StatsBB.ViewModel;

public class PlayActionViewModel
{
    public Brush TeamColor { get; set; } = Brushes.Gray;
    public string PlayerNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}

public class PlayCardViewModel
{
    /// <summary>
    /// Name of the period when the actions occurred (e.g. "Q1").
    /// </summary>
    public string PeriodName { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public int TeamAScore { get; set; }
    public int TeamBScore { get; set; }
    public ObservableCollection<PlayActionViewModel> Actions { get; } = new();

    /// <summary>
    /// Header displayed above play actions.
    /// Formatted as "PERIOD TIME SCORE" (e.g. "Q1 09:58 10:8").
    /// </summary>
    public string Header => $"{PeriodName} {Time} {TeamAScore}:{TeamBScore}";
}
