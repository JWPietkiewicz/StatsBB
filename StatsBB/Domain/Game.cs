using System;
using System.Collections.Generic;

namespace StatsBB.Domain;

public class Game
{
    public const int RegularPeriodLength = 10;
    public const int OvertimeLength = 5;
    public const int DefaultPeriods = 4;
    public const int PeriodFoulLimit = 5;

    public Guid GameId { get; set; } = Guid.NewGuid();
    public Team? HomeTeam { get; set; }
    public Team? AwayTeam { get; set; }
    public List<Period> Periods { get; set; } = new();
    public int CurrentPeriod { get; set; }
    public List<ActionLogEntry> ActionLog { get; set; } = new();

    public Game()
    {
        InitializePeriods(DefaultPeriods);
    }

    public void InitializePeriods(int count)
    {
        Periods.Clear();
        for (int i = 0; i < count; i++)
        {
            Periods.Add(new Period
            {
                PeriodNumber = i + 1,
                Name = $"Q{i + 1}",
                IsRegular = true,
                Status = PeriodStatus.Setup,
                Length = TimeSpan.FromMinutes(RegularPeriodLength)
            });
        }
        CurrentPeriod = 0;
    }

    public Period AddOvertimePeriod()
    {
        int otCount = Periods.FindAll(p => !p.IsRegular).Count + 1;
        var period = new Period
        {
            PeriodNumber = Periods.Count + 1,
            Name = $"OT{otCount}",
            IsRegular = false,
            Status = PeriodStatus.Setup,
            Length = TimeSpan.FromMinutes(OvertimeLength)
        };
        Periods.Add(period);
        return period;
    }

    public Period GetCurrentPeriod() =>
        CurrentPeriod >= 0 && CurrentPeriod < Periods.Count
            ? Periods[CurrentPeriod]
            : new Period { PeriodNumber = CurrentPeriod + 1 };
}
