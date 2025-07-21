using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using StatsBB.MVVM;

namespace StatsBB.ViewModel;

/// <summary>
/// ViewModel managing the play-by-play log entries.
/// </summary>
public class PlayByPlayLogViewModel : ViewModelBase
{
    public ObservableCollection<PlayCardViewModel> Cards { get; } = new();
    public ObservableCollection<PlayActionEntryViewModel> Entries { get; } = new();

    public ICollectionView EntryView { get; }

    public ObservableCollection<string> PeriodOptions { get; } = new() { "All" };
    public ObservableCollection<string> PlayerOptions { get; } = new() { "All" };
    public ObservableCollection<string> ActionOptions { get; } = new() { "All" };

    private string _selectedPeriod = "All";
    public string SelectedPeriod
    {
        get => _selectedPeriod;
        set
        {
            if (_selectedPeriod != value)
            {
                _selectedPeriod = value;
                OnPropertyChanged();
                EntryView.Refresh();
            }
        }
    }

    private string _selectedPlayer = "All";
    public string SelectedPlayer
    {
        get => _selectedPlayer;
        set
        {
            if (_selectedPlayer != value)
            {
                _selectedPlayer = value;
                OnPropertyChanged();
                EntryView.Refresh();
            }
        }
    }

    private string _selectedAction = "All";
    public string SelectedAction
    {
        get => _selectedAction;
        set
        {
            if (_selectedAction != value)
            {
                _selectedAction = value;
                OnPropertyChanged();
                EntryView.Refresh();
            }
        }
    }

    public PlayByPlayLogViewModel()
    {
        EntryView = CollectionViewSource.GetDefaultView(Entries);
        EntryView.Filter = obj => FilterEntry(obj as PlayActionEntryViewModel);
    }

    private bool FilterEntry(PlayActionEntryViewModel? entry)
    {
        if (entry == null)
            return false;

        if (SelectedPeriod != "All" && entry.Period != SelectedPeriod)
            return false;

        if (SelectedPlayer != "All" && entry.LastName != SelectedPlayer)
            return false;

        if (SelectedAction != "All" && entry.Action != SelectedAction)
            return false;

        return true;
    }

    /// <summary>
    /// Adds a new play card to the log.
    /// </summary>
    /// <param name="period">Name of the period when the play happened.</param>
    /// <param name="time">Timestamp of the play.</param>
    /// <param name="teamAScore">Current home score.</param>
    /// <param name="teamBScore">Current away score.</param>
    /// <param name="actions">Actions to display on the card.</param>
    public void AddCard(string period, string time, int teamAScore, int teamBScore,
        IEnumerable<PlayActionViewModel> actions)
    {
        var card = new PlayCardViewModel
        {
            PeriodName = period,
            Time = time,
            TeamAScore = teamAScore,
            TeamBScore = teamBScore
        };
        foreach (var a in actions)
        {
            card.Actions.Add(a);
            var entry = new PlayActionEntryViewModel
            {
                Period = period,
                Time = time,
                TeamColor = a.TeamColor,
                PlayerNumber = a.PlayerNumber,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Action = a.Action
            };
            Entries.Insert(0, entry);

            if (!PeriodOptions.Contains(period))
                PeriodOptions.Add(period);

            if (!PlayerOptions.Contains(a.LastName))
                PlayerOptions.Add(a.LastName);

            if (!ActionOptions.Contains(a.Action))
                ActionOptions.Add(a.Action);
        }
        EntryView.Refresh();
        Cards.Insert(0, card);
    }
}
