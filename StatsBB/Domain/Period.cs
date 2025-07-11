using System;

namespace StatsBB.Domain;

public class Period
{
    public int PeriodNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// True if the period is one of the regular quarters. Overtime
    /// periods are marked with <c>false</c>.
    /// </summary>
    public bool IsRegular { get; set; }
    public PeriodStatus Status { get; set; } = PeriodStatus.Setup;
    public TimeSpan Length { get; set; }

    public int HomeFouls { get; set; }
    public int AwayFouls { get; set; }
    public int HomeTimeoutsTaken { get; set; }
    public int AwayTimeoutsTaken { get; set; }

    public int HomePeriodScore { get; set; }
    public int AwayPeriodScore { get; set; }
}
