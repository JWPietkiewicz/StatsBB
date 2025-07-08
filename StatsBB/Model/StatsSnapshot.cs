namespace StatsBB.Model;

/// <summary>
/// Represents a snapshot of team statistics after a play.
/// Only a few fields are included for demo purposes.
/// </summary>
public class StatsSnapshot
{
    /// <summary>Team score after the play.</summary>
    public int Score { get; set; }

    /// <summary>Total number of fouls committed.</summary>
    public int Fouls { get; set; }

    /// <summary>Remaining timeouts.</summary>
    public int TimeoutsLeft { get; set; }
}
