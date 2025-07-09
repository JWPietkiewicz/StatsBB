using System;
using System.Collections.Generic;

namespace StatsBB.Domain;

public class Team
{
    public Guid TeamId { get; set; } = Guid.NewGuid();
    public string TeamName { get; set; } = string.Empty;
    public TeamColor TeamColor { get; set; }
    public List<Player> Players { get; set; } = new();
    public int Points { get; set; }

    public bool IsHomeTeam { get; set; }

    public void AddPoints(int points) => Points += points;

    public void AddFoul(Period currentPeriod)
    {
        if (IsHomeTeam)
            currentPeriod.HomeTeamFouls++;
        else
            currentPeriod.AwayTeamFouls++;
    }

    public void AddTimeout(Period currentPeriod)
    {
        if (IsHomeTeam)
            currentPeriod.HomeTimeoutsTaken++;
        else
            currentPeriod.AwayTimeoutsTaken++;
    }
}
