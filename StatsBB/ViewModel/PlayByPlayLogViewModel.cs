using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using StatsBB.MVVM;
using StatsBB.Windows;

namespace StatsBB.ViewModel;

/// <summary>
/// ViewModel managing the play-by-play log entries.
/// </summary>
public class PlayByPlayLogViewModel : ViewModelBase
{
    /// <summary>
    /// Play cards collection - read-only display with no reordering capabilities.
    /// Items are always added at the beginning (index 0) to show most recent plays first.
    /// </summary>
    public ObservableCollection<PlayCardViewModel> Cards { get; } = new();
    public ObservableCollection<PlayActionEntryViewModel> Entries { get; } = new();

    public ICollectionView EntryView { get; }

    public ObservableCollection<string> PeriodOptions { get; } = new() { "All" };
    public ObservableCollection<string> PlayerOptions { get; } = new() { "All" };
    public ObservableCollection<string> ActionOptions { get; } = new() { "All" };
    
    public ICommand EditPlayCommand { get; }

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
        EditPlayCommand = new RelayCommand(param => EditPlay(param as PlayActionViewModel));
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
    
    /// <summary>
    /// Handles editing a play action.
    /// </summary>
    /// <param name="action">The play action to edit.</param>
    private void EditPlay(PlayActionViewModel? action)
    {
        if (action == null) return;
        
        try
        {
            var dialog = new EditPlayDialog(action);
            dialog.Owner = System.Windows.Application.Current.MainWindow;
            
            if (dialog.ShowDialog() == true && dialog.WasEdited && dialog.PlayAction != null)
            {
                // Find and update the corresponding entry in the Entries collection first
                var correspondingEntry = Entries.FirstOrDefault(e => 
                    e.PlayerNumber == action.PlayerNumber &&
                    e.FirstName == action.FirstName &&
                    e.LastName == action.LastName &&
                    e.Action == action.Action);
                
                if (correspondingEntry != null)
                {
                    correspondingEntry.PlayerNumber = dialog.PlayAction.PlayerNumber;
                    correspondingEntry.FirstName = dialog.PlayAction.FirstName;
                    correspondingEntry.LastName = dialog.PlayAction.LastName;
                    correspondingEntry.Action = dialog.PlayAction.Action;
                }
                
                // Now update the original action with the edited values
                action.PlayerNumber = dialog.PlayAction.PlayerNumber;
                action.FirstName = dialog.PlayAction.FirstName;
                action.LastName = dialog.PlayAction.LastName;
                action.Action = dialog.PlayAction.Action;
                
                // Refresh the view to show changes
                EntryView.Refresh();
                
                System.Windows.MessageBox.Show(
                    "Play action has been updated successfully.",
                    "Edit Successful",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
        }
        catch (System.Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"An error occurred while editing the play action:\n{ex.Message}",
                "Edit Error",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }
}
