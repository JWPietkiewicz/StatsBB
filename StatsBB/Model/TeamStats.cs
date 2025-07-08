using System.Collections.Generic;

namespace StatsBB.Model
{
    public class TeamStats
    {
        public List<PlayerStatLine> Players { get; set; } = new();
        public TeamStatline Team { get; set; } = new();
    }
}
