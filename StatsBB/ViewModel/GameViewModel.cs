using StatsBB.Model;

namespace StatsBB.ViewModel
{
    public class GameViewModel
    {
        public Game CurrentGame { get; set; } = new();
        public Period CurrentPeriod { get; set; }
        public int CurrentPeriodTime { get; set; }
    }
}
