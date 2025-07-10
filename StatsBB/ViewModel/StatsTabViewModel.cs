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

    public ObservableCollection<Player> HomePlayers =>
        new(Game.HomeTeam?.Players.Where(p => p.IsPlaying).Select(p => { p.IsTeamA = true; return p; }) ?? new());

    public ObservableCollection<Player> AwayPlayers =>
        new(Game.AwayTeam?.Players.Where(p => p.IsPlaying).Select(p => { p.IsTeamA = false; return p; }) ?? new());

    public int HomeScore => Game.HomeTeam?.Points ?? 0;
    public int AwayScore => Game.AwayTeam?.Points ?? 0;

    private int GetHomePeriod(int index) => Game.Periods.ElementAtOrDefault(index)?.HomeTeamPoints ?? 0;
    private int GetAwayPeriod(int index) => Game.Periods.ElementAtOrDefault(index)?.AwayTeamPoints ?? 0;

    public int HomeP1 => GetHomePeriod(0);
    public int HomeP2 => GetHomePeriod(1);
    public int HomeP3 => GetHomePeriod(2);
    public int HomeP4 => GetHomePeriod(3);

    public int AwayP1 => GetAwayPeriod(0);
    public int AwayP2 => GetAwayPeriod(1);
    public int AwayP3 => GetAwayPeriod(2);
    public int AwayP4 => GetAwayPeriod(3);

    public void Refresh()
    {
        OnPropertyChanged(nameof(HomePlayers));
        OnPropertyChanged(nameof(AwayPlayers));
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
    }
}
