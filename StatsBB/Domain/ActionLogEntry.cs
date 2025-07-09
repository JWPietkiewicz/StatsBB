using System;

namespace StatsBB.Domain;

public class ActionLogEntry
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
    public Player? PlayerInvolved { get; set; }
    public Player? AssistingPlayer { get; set; }
    public ActionType ActionType { get; set; }
}
