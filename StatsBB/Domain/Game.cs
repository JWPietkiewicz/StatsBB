using StatsBB.Model;
using System;
using System.Collections.Generic;

namespace StatsBB.Domain;

public class Game
{
    public Guid GameId { get; set; } = Guid.NewGuid();
    public Team? HomeTeam { get; set; }
    public TeamInfo? HomeTeamInfo { get; set; }
    public Team? AwayTeam { get; set; }
    public TeamInfo? AwayTeamInfo { get; set; }
    public List<Period> Periods { get; set; } = new();
    public int CurrentPeriod { get; set; }
    public List<ActionLogEntry> ActionLog { get; set; } = new();

    public Period GetCurrentPeriod() =>
        CurrentPeriod >= 0 && CurrentPeriod < Periods.Count
            ? Periods[CurrentPeriod]
            : new Period { PeriodNumber = CurrentPeriod + 1 };
}
