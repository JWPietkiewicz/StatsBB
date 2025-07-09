using System.Collections.ObjectModel;
using System.Net.Sockets;
using StatsBB.Domain;
using StatsBB.MVVM;

namespace StatsBB.Model;

public class TeamInfo : ViewModelBase
{
    public Team Team { get; set; }
    public Guid TeamId
    {
        get => Team.TeamId;
        set { Team.TeamId = value; OnPropertyChanged(); }
    }
    public int Points
    {
        get => Team.Points;
        set { Team.Points = value; OnPropertyChanged(); }
    }

    public bool IsHomeTeam
    {
        get => Team.IsHomeTeam;
        set { Team.IsHomeTeam = value; OnPropertyChanged(); }
    }

    public void AddPoints(int points)
    {
        Team.AddPoints(points);
    }

    public void AddFoul(Period currentPeriod)
    {
        Team.AddFoul(currentPeriod);
    }

    public string Name
    {
        get => Team.TeamName;
        set { Team.TeamName = value; OnPropertyChanged(); }
    }

    public string ShortName
    {
        get => Team.TeamShortName;
        set { Team.TeamShortName = value; OnPropertyChanged(); }
    }

    public TeamColorOption? Color
    {
        get => Team.TeamColor;
        set { Team.TeamColor = value; OnPropertyChanged(); }
    }

    public ObservableCollection<Player> Players { get => Team.Players; }
}
