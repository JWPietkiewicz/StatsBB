using System.Collections.ObjectModel;
using StatsBB.Model;
using StatsBB.MVVM;

namespace StatsBB.ViewModel;

public class StatsTabViewModel : ViewModelBase
{
    public ObservableCollection<PlayerStats> TeamAStats { get; } = new();
    public ObservableCollection<PlayerStats> TeamBStats { get; } = new();

    public StatsTabViewModel(ObservableCollection<Player> players)
    {
        foreach (var p in players)
        {
            var ps = new PlayerStats
            {
                No = p.Number,
                Player = p.Name,
                S5 = p.IsActive,
                OnCourt = p.IsActive
            };
            if (p.IsTeamA)
                TeamAStats.Add(ps);
            else
                TeamBStats.Add(ps);
        }
    }
}
