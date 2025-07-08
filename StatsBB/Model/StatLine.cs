namespace StatsBB.Model
{
    public class StatLine
    {
        public int Points { get; set; }
        public int Block { get; set; }
        public int BlockR { get; set; }
        public int ReboundD { get; set; }
        public int ReboundO { get; set; }
        public int PersonalFouls { get; set; }
        public int FoulsDrawn { get; set; }
        public int FieldGoal2Attemt { get; set; }
        public int FieldGoal2Made { get; set; }
        public float FieldGoal2Percentage { get; set; }
        public int FieldGoal3Attempt { get; set; }
        public int FieldGoal3Made { get; set; }
        public float FieldGoal3Percentage { get; set; }
        public int FieldGoalAttempt { get; set; }
        public int FieldGoalMade { get; set; }
        public float FieldGoalMadePercentage { get; set; }
        public int FreeThrowAttempt { get; set; }
        public int FreeThrowMade { get; set; }
        public float FreeThrowPercentage { get; set; }
        public int TimeOnField { get; set; }
        public int PlusMinus { get; set; }
        public int Efficiency { get; set; }
    }
}
