using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StatsBB.Domain;

public class Team
{
    public Guid TeamId { get; set; } = Guid.NewGuid();
    public string TeamName { get; set; } = string.Empty;
    public string TeamShortName {  get; set; } = string.Empty;
    /// <summary>
    /// Name of the team's color. This is kept as a simple string so the domain
    /// layer has no dependency on UI-specific color types.
    /// </summary>
    public string TeamColorName { get; set; } = "White";
    public ObservableCollection<Player> Players { get; set; } = new();
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


    public ObservableCollection<Player> GetPlayers()
    {
        ObservableCollection<Player> result = new ObservableCollection<Player>();
        foreach (Player player in Players)
        {
            player.IsTeamA = IsHomeTeam;
            result.Add(player);
        }
        return result;
    }
}
