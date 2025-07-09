using System;
using System.Collections.ObjectModel;
using StatsBB.Domain;
using StatsBB.Model;
using StatsBB.MVVM;

namespace StatsBB.ViewModel;

public class TeamInfoViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _main;
    public Game Game { get; } = new();

    public ObservableCollection<TeamColor> TeamColors { get; } = new((TeamColor[])Enum.GetValues(typeof(TeamColor)));

    public TeamInfoViewModel(MainWindowViewModel main)
    {
        _main = main;
        Game.HomeTeam = new Team { IsHomeTeam = true };
        Game.AwayTeam = new Team { IsHomeTeam = false };
        EnsurePlayers(Game.HomeTeam);
        EnsurePlayers(Game.AwayTeam);
        HomeTeamName = _main.TeamAName;
        AwayTeamName = _main.TeamBName;
    }

    private static void EnsurePlayers(Team team)
    {
        while (team.Players.Count < 15)
            team.Players.Add(new Player());
    }

    public string HomeTeamName
    {
        get => Game.HomeTeam.TeamName;
        set
        {
            Game.HomeTeam.TeamName = value;
            OnPropertyChanged();
            _main.TeamAName = value;
        }
    }

    public string AwayTeamName
    {
        get => Game.AwayTeam.TeamName;
        set
        {
            Game.AwayTeam.TeamName = value;
            OnPropertyChanged();
            _main.TeamBName = value;
        }
    }

    public TeamColor HomeTeamColor
    {
        get => Game.HomeTeam.TeamColor;
        set { Game.HomeTeam.TeamColor = value; OnPropertyChanged(); }
    }

    public TeamColor AwayTeamColor
    {
        get => Game.AwayTeam.TeamColor;
        set { Game.AwayTeam.TeamColor = value; OnPropertyChanged(); }
    }
    
    public ObservableCollection<Player> HomePlayers => Game.HomeTeam.GetPlayers();
    public ObservableCollection<Player> AwayPlayers => Game.AwayTeam.GetPlayers();
}
