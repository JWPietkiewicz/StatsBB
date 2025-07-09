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
    public string Time { get; set; } = string.Empty;
    public int TeamAScore { get; set; }
    public int TeamBScore { get; set; }
    public ObservableCollection<PlayActionViewModel> Actions { get; } = new();

    public string Header => $"{Time} {TeamAScore}:{TeamBScore}";
}
