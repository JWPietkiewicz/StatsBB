using System.Collections.Generic;
using System.Collections.ObjectModel;
using StatsBB.MVVM;

namespace StatsBB.ViewModel;

/// <summary>
/// ViewModel managing the play-by-play log entries.
/// </summary>
public class PlayByPlayLogViewModel : ViewModelBase
{
    public ObservableCollection<PlayCardViewModel> Cards { get; } = new();

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
            card.Actions.Add(a);
        Cards.Insert(0, card);
    }
}
