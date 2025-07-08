using System.Collections.Generic;

namespace StatsBB.Model
{
    public class Game
    {
        public Team TeamA { get; set; } = new();
        public Team TeamB { get; set; } = new();
        public int Period { get; set; }
        public int PeriodLength { get; set; }
        public bool Overtime { get; set; }
        public string Q1Score { get; set; } = string.Empty;
        public string Q2Score { get; set; } = string.Empty;
        public string Q3Score { get; set; } = string.Empty;
        public string Q4Score { get; set; } = string.Empty;
        public string QT1Score { get; set; } = string.Empty;
        public string OT2Score { get; set; } = string.Empty;
        public string OT3Score { get; set; } = string.Empty;
        public string OT4Score { get; set; } = string.Empty;
        public string OT5Score { get; set; } = string.Empty;
        public string OT6Score { get; set; } = string.Empty;
        public string OT7Score { get; set; } = string.Empty;
        public string OT8Score { get; set; } = string.Empty;
        public int Periods { get; set; } = 4;
        public int PeriodTime { get; set; } = 10;
        public int MaxPlayerFoul { get; set; } = 5;
        public int TeamFoulLimit { get; set; } = 4;
        public bool OT { get; set; } = true;
        public int OTTime { get; set; } = 5;
        public bool JumpBall { get; set; } = false;
        public int MaxTechnicalFouls { get; set; } = 2;
        public int PlayersOnCourt { get; set; } = 5;
        public bool TimeoutsPerPeriod { get; set; } = false;
        public List<int> TimeoutsPerPeriodList { get; set; } = new();
        public bool TimeoutsPerHalf { get; set; } = true;
        public int TieoutsFirstHalf { get; set; } = 2;
        public int TieoutsSecondHalf { get; set; } = 3;
        public int TimeoutsPerOvertime { get; set; } = 0;
        public List<PlayByPlay> PlayByPlay { get; set; } = new();
    }
}
