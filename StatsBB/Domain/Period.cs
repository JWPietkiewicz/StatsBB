using System;

namespace StatsBB.Domain;

public class Period
{
    public int PeriodNumber { get; set; }
    public TimeSpan Duration { get; set; }
    public int HomeTeamFouls { get; set; }
    public int AwayTeamFouls { get; set; }
    public int HomeTimeoutsTaken { get; set; }
    public int AwayTimeoutsTaken { get; set; }
}
