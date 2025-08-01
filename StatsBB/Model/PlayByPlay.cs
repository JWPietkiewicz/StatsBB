namespace StatsBB.Model
{
    public class PlayByPlay
    {
        public int Id { get; set; }
        public int Period { get; set; }
        public string Time { get; set; }
        public bool TeamA {  get; set; }
        public int PlayerNumer { get; set; }
        public string PlayerName { get; set; }
        public bool IsTeamAction { get; set; }
        public bool IsGameAction { get; set; }
        public PlayType Play { get; set; }
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
        Unknown = 29,
        HalfStart = 30,
        HalfEnd = 31
    }
}
