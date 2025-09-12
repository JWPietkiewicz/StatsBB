using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using StatsBB.MVVM;

namespace StatsBB.ViewModel;

public class PlayActionViewModel : ViewModelBase
{
    private Brush _teamColor = Brushes.Gray;
    private string _playerNumber = string.Empty;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _action = string.Empty;
    
    public Brush TeamColor
    {
        get => _teamColor;
        set
        {
            _teamColor = value;
            OnPropertyChanged();
        }
    }
    
    public string PlayerNumber
    {
        get => _playerNumber;
        set
        {
            _playerNumber = value;
            OnPropertyChanged();
        }
    }
    
    public string FirstName
    {
        get => _firstName;
        set
        {
            _firstName = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FullName));
        }
    }
    
    public string LastName
    {
        get => _lastName;
        set
        {
            _lastName = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FullName));
        }
    }
    
    public string Action
    {
        get => _action;
        set
        {
            _action = value;
            OnPropertyChanged();
        }
    }
    
    /// <summary>
    /// Gets the properly formatted full name for display.
    /// </summary>
    public string FullName
    {
        get
        {
            if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
                return $"{FirstName} {LastName}";
            if (!string.IsNullOrEmpty(FirstName))
                return FirstName;
            if (!string.IsNullOrEmpty(LastName))
                return LastName;
            return string.Empty;
        }
    }
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

/// <summary>
/// Flat representation of a single play action used by the sortable log.
/// </summary>
public class PlayActionEntryViewModel
{
    public string Period { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public Brush TeamColor { get; set; } = Brushes.Gray;
    public string PlayerNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets the properly formatted full name for display.
    /// </summary>
    public string FullName
    {
        get
        {
            if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
                return $"{FirstName} {LastName}";
            if (!string.IsNullOrEmpty(FirstName))
                return FirstName;
            if (!string.IsNullOrEmpty(LastName))
                return LastName;
            return string.Empty;
        }
    }
}
