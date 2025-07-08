namespace StatsBB.Model
{
    public class Team
    {
        public string TeamName { get; set; } = string.Empty;
        public string TeamShortName { get; set; } = string.Empty;
        public TeamColor TeamColor { get; set; }
        public TeamStats Stats { get; set; } = new();
    }
}
