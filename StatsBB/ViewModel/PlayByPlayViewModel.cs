using StatsBB.Model;

namespace StatsBB.ViewModel;

/// <summary>
/// View model used to render play-by-play information in the UI.
/// </summary>
public class PlayByPlayViewModel
{
    public int Id { get; set; }
    public int Sequence { get; set; }
    public string Time { get; set; } = string.Empty;
    public string PeriodLabel { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public int? PlayerNumber { get; set; }
    public string? PlayerName { get; set; }
    public string ActionDescription { get; set; } = string.Empty;
    public bool IsMadeShot { get; set; }
    public bool IsFreeThrow { get; set; }
    public bool IsRebound { get; set; }
    public bool IsTurnover { get; set; }
    public bool PossessionChange { get; set; }
    public bool ArrowSwitch { get; set; }
    public bool IsScoreChange { get; set; }
    public int Points { get; set; }
    public string ScoreAfter { get; set; } = string.Empty;
    public StatsSnapshot TeamAStats { get; set; } = new();
    public StatsSnapshot TeamBStats { get; set; } = new();

    /// <summary>
    /// Maps a raw <see cref="PlayByPlay"/> into a view model instance.
    /// </summary>
    public static PlayByPlayViewModel Map(
        PlayByPlay play,
        IDictionary<int, PlayerInfo> players,
        StatsSnapshot[]? scoreData = null)
    {
        var vm = new PlayByPlayViewModel
        {
            Id = play.Id,
            Sequence = play.OrderNumber,
            Time = play.Clock,
            PeriodLabel = play.Period.ToLabel(),
            TeamName = play.Team == TeamSelect.TeamA ? "Team A" : "Team B",
            PossessionChange = play.PossessionSwitch,
            ArrowSwitch = play.ArrowSwitch,
            IsScoreChange = play.ScoreChange,
            Points = play.Point.Value,
            ScoreAfter = $"{play.TeamA.Score}\u2013{play.TeamB.Score}",
            TeamAStats = play.TeamA,
            TeamBStats = play.TeamB
        };

        if (players != null && players.TryGetValue(play.Player, out var info))
        {
            vm.PlayerNumber = info.Number;
            vm.PlayerName = info.Name;
        }

        vm.IsFreeThrow = play.PlayType == PlayType.FreeThrow;
        vm.IsMadeShot =
            (play.PlayType == PlayType.FieldGoal || vm.IsFreeThrow) &&
            play.Flags.Contains("made");
        vm.IsRebound = play.PlayType == PlayType.Rebound;
        vm.IsTurnover = play.PlayType == PlayType.Turnover;

        vm.ActionDescription = string.IsNullOrWhiteSpace(play.Description)
            ? BuildDescription(play, vm.PlayerName, vm.PlayerNumber)
            : play.Description;

        return vm;
    }

    private static string BuildDescription(PlayByPlay play, string? name, int? number)
    {
        var prefix = name != null && number.HasValue ? $"{name} ({number}) " : string.Empty;
        return prefix + play.PlayType switch
        {
            PlayType.FieldGoal when play.Flags.Contains("made") && play.Flags.Contains("3pt")
                => "makes 3-pt shot",
            PlayType.FieldGoal when play.Flags.Contains("made") => "makes shot",
            PlayType.FieldGoal when play.Flags.Contains("missed") => "misses shot",
            PlayType.FreeThrow when play.Flags.Contains("made") => "makes free throw",
            PlayType.FreeThrow when play.Flags.Contains("missed") => "misses free throw",
            PlayType.Rebound => (play.Flags.Contains("offensive") ? "offensive" : "defensive") + " rebound",
            PlayType.Turnover => "turnover",
            PlayType.Foul => "foul",
            PlayType.Substitution => "substitution",
            _ => play.Description
        };
    }
}

internal static class PeriodExtensions
{
    public static string ToLabel(this Period period)
    {
        return period switch
        {
            Period.Q1 => "Q1",
            Period.Q2 => "Q2",
            Period.Q3 => "Q3",
            Period.Q4 => "Q4",
            Period.OT1 => "OT",
            Period.OT2 => "OT2",
            _ => period.ToString()
        };
    }
}
