namespace StatsBB.Model;

/// <summary>
/// Basic information about a player used for lookups when mapping plays.
/// </summary>
public class PlayerInfo
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
}
