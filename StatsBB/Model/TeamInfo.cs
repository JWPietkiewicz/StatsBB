using System.Collections.ObjectModel;
using StatsBB.Domain;
using StatsBB.MVVM;

namespace StatsBB.Model;

public class TeamInfo : ViewModelBase
{
    public TeamInfo()
    {
        Team = new Team();
    }

    public TeamInfo(Team team)
    {
        Team = team;
    }

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

    private TeamColorOption? _color;
    public TeamColorOption? Color
    {
        get => _color;
        set
        {
            if (_color == value) return;
            _color = value;
            OnPropertyChanged();
            Team.TeamColorName = value?.Name ?? string.Empty;
        }
    }

    public ObservableCollection<Player> Players { get => Team.Players; }
}
