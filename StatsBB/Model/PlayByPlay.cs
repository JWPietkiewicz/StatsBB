namespace StatsBB.Model;

/// <summary>
/// Represents a raw play-by-play event coming from the live game feed.
/// </summary>
public class PlayByPlay
{
    public int Id { get; set; }
    public int OrderNumber { get; set; }
    public string Clock { get; set; } = string.Empty;
    public Period Period { get; set; }
    public TeamSelect Team { get; set; }
    public int Player { get; set; }
    public PlayType PlayType { get; set; }
    public List<string> Flags { get; set; } = new();
    public List<string> SubFlags { get; set; } = new();
    public Point Point { get; set; } = new();
    public bool PossessionSwitch { get; set; }
    public bool ArrowSwitch { get; set; }
    public bool ScoreChange { get; set; }
    public StatsSnapshot TeamA { get; set; } = new();
    public StatsSnapshot TeamB { get; set; } = new();
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Identifies which team performed the action.
/// </summary>
public enum TeamSelect
{
    TeamA,
    TeamB
}

/// <summary>
/// Enumerates the different periods in a game.
/// </summary>
public enum Period
{
    Q1 = 1,
    Q2 = 2,
    Q3 = 3,
    Q4 = 4,
    OT1 = 5,
    OT2 = 6
}

/// <summary>
/// Type of play that occurred.
/// </summary>
public enum PlayType
{
    FieldGoal,
    FreeThrow,
    Rebound,
    Turnover,
    Foul,
    Substitution,
    Timeout,
    JumpBall,
    Violation,
    Other
}

/// <summary>
/// Simple point wrapper used in the feed.
/// </summary>
public class Point
{
    public int Value { get; set; }
}
