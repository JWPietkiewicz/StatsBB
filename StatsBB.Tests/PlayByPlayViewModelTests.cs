using System.Collections.Generic;
using StatsBB.Model;
using StatsBB.ViewModel;
using Xunit;

public class PlayByPlayViewModelTests
{
    private readonly Dictionary<int, PlayerInfo> _players = new()
    {
        { 23, new PlayerInfo { Id = 23, Name = "Jones", Number = 23 } },
        { 5, new PlayerInfo { Id = 5, Name = "Smith", Number = 5 } }
    };

    [Fact]
    public void Map_MadeThreePointShot()
    {
        var play = new PlayByPlay
        {
            Id = 1,
            OrderNumber = 27,
            Clock = "06:12",
            Period = Period.Q2,
            Team = TeamSelect.TeamA,
            Player = 23,
            PlayType = PlayType.FieldGoal,
            Flags = new() { "made", "3pt" },
            Point = new Point { Value = 3 },
            ScoreChange = true,
            TeamA = new StatsSnapshot { Score = 58 },
            TeamB = new StatsSnapshot { Score = 54 }
        };

        var vm = PlayByPlayViewModel.Map(play, _players);
        Assert.Equal(27, vm.Sequence);
        Assert.True(vm.IsMadeShot);
        Assert.Equal(3, vm.Points);
        Assert.Equal("58\u201354", vm.ScoreAfter);
        Assert.Equal("Jones", vm.PlayerName);
    }

    [Fact]
    public void Map_MissedFreeThrow()
    {
        var play = new PlayByPlay
        {
            Id = 2,
            OrderNumber = 30,
            Clock = "02:10",
            Period = Period.Q1,
            Team = TeamSelect.TeamB,
            Player = 5,
            PlayType = PlayType.FreeThrow,
            Flags = new() { "missed" },
            Point = new Point { Value = 0 },
            ScoreChange = false,
            TeamA = new StatsSnapshot { Score = 10 },
            TeamB = new StatsSnapshot { Score = 12 }
        };

        var vm = PlayByPlayViewModel.Map(play, _players);
        Assert.True(vm.IsFreeThrow);
        Assert.False(vm.IsMadeShot);
        Assert.Equal(0, vm.Points);
    }

    [Fact]
    public void Map_Rebound_Turnover()
    {
        var rebound = new PlayByPlay
        {
            Id = 3,
            OrderNumber = 40,
            Clock = "01:15",
            Period = Period.Q3,
            Team = TeamSelect.TeamA,
            Player = 23,
            PlayType = PlayType.Rebound,
            Flags = new() { "offensive" },
            Point = new Point { Value = 0 },
            ScoreChange = false,
            TeamA = new StatsSnapshot { Score = 70 },
            TeamB = new StatsSnapshot { Score = 65 }
        };
        var reboundVm = PlayByPlayViewModel.Map(rebound, _players);
        Assert.True(reboundVm.IsRebound);

        var turnover = new PlayByPlay
        {
            Id = 4,
            OrderNumber = 41,
            Clock = "01:10",
            Period = Period.Q3,
            Team = TeamSelect.TeamA,
            Player = 23,
            PlayType = PlayType.Turnover,
            Flags = new(),
            PossessionSwitch = true,
            Point = new Point { Value = 0 },
            ScoreChange = false,
            TeamA = new StatsSnapshot { Score = 70 },
            TeamB = new StatsSnapshot { Score = 65 }
        };
        var turnoverVm = PlayByPlayViewModel.Map(turnover, _players);
        Assert.True(turnoverVm.IsTurnover);
        Assert.True(turnoverVm.PossessionChange);
    }

    [Fact]
    public void Map_Foul_Substitution()
    {
        var foul = new PlayByPlay
        {
            Id = 5,
            OrderNumber = 50,
            Clock = "05:00",
            Period = Period.Q4,
            Team = TeamSelect.TeamB,
            Player = 5,
            PlayType = PlayType.Foul,
            Flags = new(),
            Point = new Point { Value = 0 },
            ScoreChange = false,
            TeamA = new StatsSnapshot { Score = 80 },
            TeamB = new StatsSnapshot { Score = 82 }
        };
        var foulVm = PlayByPlayViewModel.Map(foul, _players);
        Assert.Equal("Smith", foulVm.PlayerName);
        Assert.Equal(PlayType.Foul, foul.PlayType);

        var sub = new PlayByPlay
        {
            Id = 6,
            OrderNumber = 51,
            Clock = "04:50",
            Period = Period.Q4,
            Team = TeamSelect.TeamB,
            Player = 5,
            PlayType = PlayType.Substitution,
            Flags = new(),
            Point = new Point { Value = 0 },
            ScoreChange = false,
            TeamA = new StatsSnapshot { Score = 80 },
            TeamB = new StatsSnapshot { Score = 82 }
        };
        var subVm = PlayByPlayViewModel.Map(sub, _players);
        Assert.Equal("Smith", subVm.PlayerName);
        Assert.Equal(PlayType.Substitution, sub.PlayType);
    }

    [Fact]
    public void Map_ScoreChange_PossessionArrow()
    {
        var play = new PlayByPlay
        {
            Id = 7,
            OrderNumber = 60,
            Clock = "03:00",
            Period = Period.OT1,
            Team = TeamSelect.TeamA,
            Player = 23,
            PlayType = PlayType.FieldGoal,
            Flags = new() { "made" },
            Point = new Point { Value = 2 },
            ScoreChange = true,
            PossessionSwitch = true,
            ArrowSwitch = true,
            TeamA = new StatsSnapshot { Score = 90 },
            TeamB = new StatsSnapshot { Score = 88 }
        };
        var vm = PlayByPlayViewModel.Map(play, _players);
        Assert.True(vm.IsScoreChange);
        Assert.True(vm.PossessionChange);
        Assert.True(vm.ArrowSwitch);
    }
}
