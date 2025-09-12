using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

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
    
    private ObservableCollection<Player> _players = new();
    public ObservableCollection<Player> Players 
    { 
        get => _players;
        set
        {
            _players = value;
            // Set ParentTeam reference for all players
            foreach (var player in _players)
            {
                player.ParentTeam = this;
            }
            // Subscribe to collection changes to set ParentTeam for new players
            _players.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (Player player in e.NewItems)
                    {
                        player.ParentTeam = this;
                    }
                }
            };
        }
    }
    public int Points { get; set; }

    public int TeamRebounds { get; set; }
    public int OffensiveTeamRebounds { get; set; }
    public int DefensiveTeamRebounds { get; set; }
    public int CoachFouls { get; set; }
    public int BenchFouls { get; set; }
    public int TeamTurnovers { get; set; }

    public bool IsHomeTeam { get; set; }

    public void AddPoints(int points) => Points += points;

    public void AddTeamRebound(bool offensive)
    {
        TeamRebounds++;
        if (offensive)
            OffensiveTeamRebounds++;
        else
            DefensiveTeamRebounds++;
    }

    public void AddCoachFoul(Period currentPeriod)
    {
        CoachFouls++;
        AddFoul(currentPeriod);
    }

    public void AddBenchFoul(Period currentPeriod)
    {
        BenchFouls++;
        AddFoul(currentPeriod);
    }

    public void AddTeamTurnover()
    {
        TeamTurnovers++;
    }

    public void AddFoul(Period currentPeriod)
    {
        if (IsHomeTeam)
            currentPeriod.HomeFouls++;
        else
            currentPeriod.AwayFouls++;
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
        
        // First, get active players sorted by number, then by last name
        var activePlayers = Players.Where(p => p.IsPlaying || (!string.IsNullOrWhiteSpace(p.FirstName) || !string.IsNullOrWhiteSpace(p.LastName)))
                                  .OrderBy(p => p.Number)
                                  .ThenBy(p => p.LastName)
                                  .ToList();
        
        // Then, get inactive empty players (no sorting needed)
        var inactivePlayers = Players.Where(p => !p.IsPlaying && string.IsNullOrWhiteSpace(p.FirstName) && string.IsNullOrWhiteSpace(p.LastName))
                                   .ToList();
        
        // Add active players first
        foreach (Player player in activePlayers)
        {
            player.IsTeamA = IsHomeTeam;
            player.ParentTeam = this; // Set parent team reference for validation
            result.Add(player);
        }
        
        // Add inactive players at the end
        foreach (Player player in inactivePlayers)
        {
            player.IsTeamA = IsHomeTeam;
            player.ParentTeam = this; // Set parent team reference for validation
            result.Add(player);
        }
        
        return result;
    }

    /// <summary>
    /// Gets active players (IsPlaying = true) sorted by number
    /// </summary>
    public ObservableCollection<Player> GetActivePlayers()
    {
        ObservableCollection<Player> result = new ObservableCollection<Player>();
        var activePlayers = Players.Where(p => p.IsPlaying)
                                  .OrderBy(p => p.Number)
                                  .ThenBy(p => p.LastName)
                                  .ToList();
        foreach (Player player in activePlayers)
        {
            player.IsTeamA = IsHomeTeam;
            player.ParentTeam = this; // Set parent team reference for validation
            result.Add(player);
        }
        return result;
    }

    /// <summary>
    /// Validates if a player number is available (not used by another player)
    /// </summary>
    public bool IsPlayerNumberAvailable(int number, Player? excludePlayer = null)
    {
        return !Players.Any(p => p.Number == number && p != excludePlayer && p.Number > 0);
    }

    /// <summary>
    /// Gets the next available player number
    /// </summary>
    public int GetNextAvailableNumber()
    {
        var usedNumbers = Players.Where(p => p.Number > 0).Select(p => p.Number).ToHashSet();
        
        // Start from 1 and find the first available number
        for (int i = 1; i <= 99; i++)
        {
            if (!usedNumbers.Contains(i))
                return i;
        }
        
        return 0; // No available numbers (shouldn't happen in normal use)
    }

    /// <summary>
    /// Assigns a unique number to a player if their current number is taken
    /// </summary>
    public void EnsureUniquePlayerNumber(Player player)
    {
        if (!IsPlayerNumberAvailable(player.Number, player))
        {
            player.Number = GetNextAvailableNumber();
        }
    }
}
