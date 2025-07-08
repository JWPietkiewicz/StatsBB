using System.Collections.Generic;
using System.Windows;

namespace StatsBB.Model
{
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
        public List<string> Subflags { get; set; } = new();
        public Point? Point { get; set; }
        public bool PossesionSwitch { get; set; }
        public bool ArrowSwitch { get; set; }
        public bool ScoreChange { get; set; }
        public List<string> TeamA { get; set; } = new();
        public List<string> TeamB { get; set; } = new();
        public string Description { get; set; } = string.Empty;
    }

    public enum PlayType
    {
        GameStart = 0,
        GameEnd = 1,
        PeriodStart = 2,
        PeriodEnd = 3,
        JumpBallWon = 4,
        JumpBallLost = 5,
        Shot2Made = 6,
        Shot2Missed = 7,
        Shot3Made = 8,
        Shot3Missed = 9,
        FreeThrowMade = 10,
        FreeThrowMissed = 11,
        OffensiveRebound = 12,
        DeffensiveRebound = 13,
        TeamRebound = 14,
        Assist = 15,
        Turnover = 16,
        Steal = 17,
        Block = 18,
        Foul = 19,
        FoulDrawn = 20,
        UnsportsmanlikeFoul = 21,
        OffensiveFoul = 22,
        Ejection = 23,
        Timeout = 24,
        SubstitutionIn = 25,
        SubstitutionOut = 26,
        Dunk = 27,
        Putback = 28,
        Unknown = 29
    }
}
