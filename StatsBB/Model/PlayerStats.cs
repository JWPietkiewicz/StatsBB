using StatsBB.Model;

namespace StatsBB.Model;

public class PlayerStats
{
    public int No { get; set; }
    public string Player { get; set; } = string.Empty;
    public bool S5 { get; set; }
    public bool OnCourt { get; set; }
    public string Mins { get; set; } = "0";
    public int Pts { get; set; }
    public string FG { get; set; } = "0/0";
    public double FGPercent { get; set; }
    public string TwoP { get; set; } = "0/0";
    public double TwoPPercent { get; set; }
    public string ThreeP { get; set; } = "0/0";
    public double ThreePPercent { get; set; }
    public string FT { get; set; } = "0/0";
    public double FTPercent { get; set; }
    public int OFF { get; set; }
    public int DEF { get; set; }
    public int REB { get; set; }
    public int AST { get; set; }
    public int TO { get; set; }
    public int STL { get; set; }
    public int BLK { get; set; }
    public int BLKR { get; set; }
    public int PF { get; set; }
    public int FoulsOn { get; set; }
    public int PlusMinus { get; set; }
    public int Index { get; set; }
}
