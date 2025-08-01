using System;
using System.Collections.ObjectModel;
using System.Linq;
using StatsBB.Domain;
using StatsBB.MVVM;

namespace StatsBB.ViewModel;

public class StatsTabViewModel : ViewModelBase
{
    public Game Game { get; }

    public StatsTabViewModel(Game game)
    {
        Game = game;
    }

    public string HomeTeamName => Game.HomeTeam?.TeamName ?? string.Empty;
    public string AwayTeamName => Game.AwayTeam?.TeamName ?? string.Empty;

    public ObservableCollection<Player> HomePlayers =>
        new(Game.HomeTeam?.Players
                .Where(p => p.IsPlaying)
                .Select(p => { p.IsTeamA = true; return p; })
            ?? Enumerable.Empty<Player>());

    public ObservableCollection<Player> AwayPlayers =>
        new(Game.AwayTeam?.Players
                .Where(p => p.IsPlaying)
                .Select(p => { p.IsTeamA = false; return p; })
            ?? Enumerable.Empty<Player>());

    public int HomeScore => Game.HomeTeam?.Points ?? 0;
    public int AwayScore => Game.AwayTeam?.Points ?? 0;

    private int GetHomePeriod(int index) => Game.Periods.ElementAtOrDefault(index)?.HomePeriodScore ?? 0;
    private int GetAwayPeriod(int index) => Game.Periods.ElementAtOrDefault(index)?.AwayPeriodScore ?? 0;

    public int HomeP1 => GetHomePeriod(0);
    public int HomeP2 => GetHomePeriod(1);
    public int HomeP3 => GetHomePeriod(2);
    public int HomeP4 => GetHomePeriod(3);

    public int AwayP1 => GetAwayPeriod(0);
    public int AwayP2 => GetAwayPeriod(1);
    public int AwayP3 => GetAwayPeriod(2);
    public int AwayP4 => GetAwayPeriod(3);

    public ObservableCollection<Period> PeriodResults => new(Game.Periods);

    private Player CalculateTotals(bool home)
    {
        var players = home
            ? Game.HomeTeam?.Players?.Where(p => p.IsPlaying)
            : Game.AwayTeam?.Players?.Where(p => p.IsPlaying);
        var result = new Player { LastName = "TOTAL" };
        Team? team = home ? Game.HomeTeam : Game.AwayTeam;
        if (players != null)
        {
            foreach (var p in players)
            {
                result.Points += p.Points;
                result.Assists += p.Assists;
                result.Rebounds += p.Rebounds;
                result.OffensiveRebounds += p.OffensiveRebounds;
                result.DefensiveRebounds += p.DefensiveRebounds;
                result.Blocks += p.Blocks;
                result.Steals += p.Steals;
                result.Turnovers += p.Turnovers;
                result.FoulsCommitted += p.FoulsCommitted;
                result.ShotsMade2pt += p.ShotsMade2pt;
                result.ShotAttempts2pt += p.ShotAttempts2pt;
                result.ShotsMade3pt += p.ShotsMade3pt;
                result.ShotAttempts3pt += p.ShotAttempts3pt;
                result.FreeThrowsMade += p.FreeThrowsMade;
                result.FreeThrowsAttempted += p.FreeThrowsAttempted;
            }
        }

        if (team != null)
        {
            result.Rebounds += team.TeamRebounds;
            result.OffensiveRebounds += team.OffensiveTeamRebounds;
            result.DefensiveRebounds += team.DefensiveTeamRebounds;
            result.Turnovers += team.TeamTurnovers;
            result.FoulsCommitted += team.CoachFouls + team.BenchFouls;
        }

        return result;
    }

    private Player CalculateTeamStats(bool home)
    {
        Team? team = home ? Game.HomeTeam : Game.AwayTeam;
        if (team == null)
            return new Player { LastName = "TEAM" };

        return new Player
        {
            LastName = "TEAM",
            Rebounds = team.TeamRebounds,
            OffensiveRebounds = team.OffensiveTeamRebounds,
            DefensiveRebounds = team.DefensiveTeamRebounds,
            Turnovers = team.TeamTurnovers,
            FoulsCommitted = team.CoachFouls + team.BenchFouls
        };
    }

    public ObservableCollection<Player> HomeTotalsCollection =>
        new() { CalculateTotals(true) };

    public ObservableCollection<Player> AwayTotalsCollection =>
        new() { CalculateTotals(false) };

    public ObservableCollection<Player> HomeTeamStatsCollection =>
        new() { CalculateTeamStats(true) };

    public ObservableCollection<Player> AwayTeamStatsCollection =>
        new() { CalculateTeamStats(false) };

    public void Refresh()
    {
        OnPropertyChanged(nameof(HomePlayers));
        OnPropertyChanged(nameof(AwayPlayers));
        OnPropertyChanged(nameof(HomeTeamName));
        OnPropertyChanged(nameof(AwayTeamName));
        OnPropertyChanged(nameof(HomeScore));
        OnPropertyChanged(nameof(AwayScore));
        OnPropertyChanged(nameof(HomeP1));
        OnPropertyChanged(nameof(HomeP2));
        OnPropertyChanged(nameof(HomeP3));
        OnPropertyChanged(nameof(HomeP4));
        OnPropertyChanged(nameof(AwayP1));
        OnPropertyChanged(nameof(AwayP2));
        OnPropertyChanged(nameof(AwayP3));
        OnPropertyChanged(nameof(AwayP4));
        OnPropertyChanged(nameof(HomeTotalsCollection));
        OnPropertyChanged(nameof(AwayTotalsCollection));
        OnPropertyChanged(nameof(HomeTeamStatsCollection));
        OnPropertyChanged(nameof(AwayTeamStatsCollection));
        OnPropertyChanged(nameof(PeriodResults));
    }
}
