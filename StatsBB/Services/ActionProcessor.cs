using System;
using System.Linq;
using StatsBB.Domain;

namespace StatsBB.Services;

public class ActionProcessor
{
    private readonly Game _game;

    public ActionProcessor(Game game)
    {
        _game = game;
    }

    private Team? GetTeamOfPlayer(Player player)
    {
        if (_game.HomeTeam != null && _game.HomeTeam.Players.Contains(player))
            return _game.HomeTeam;
        if (_game.AwayTeam != null && _game.AwayTeam.Players.Contains(player))
            return _game.AwayTeam;
        return null;
    }

    private Team? GetTeam(bool isHome)
    {
        return isHome ? _game.HomeTeam : _game.AwayTeam;
    }

    public void Process(ActionType action, Player player, Player? assistingPlayer = null, bool isThreePoint = false)
    {
        var team = GetTeamOfPlayer(player);
        if (team == null)
            return;

        var period = _game.GetCurrentPeriod();
        int points = isThreePoint ? 3 : 2;

        switch (action)
        {
            case ActionType.ShotMade:
                player.AddPoints(points);
                player.AddShotMade(isThreePoint);
                if (assistingPlayer != null)
                    assistingPlayer.AddAssist();
                team.AddPoints(points);
                if (team.IsHomeTeam)
                    period.HomePeriodScore += points;
                else
                    period.AwayPeriodScore += points;
                break;
            case ActionType.ShotMissed:
                player.AddShotAttempt(isThreePoint);
                break;
            case ActionType.OffensiveRebound:
                player.AddRebound(true);
                break;
            case ActionType.DefensiveRebound:
                player.AddRebound(false);
                break;
            case ActionType.Rebound:
                player.AddRebound(true);
                break;
            case ActionType.Assist:
                player.AddAssist();
                break;
            case ActionType.Turnover:
                player.AddTurnover();
                break;
            case ActionType.Steal:
                player.AddSteal();
                break;
            case ActionType.Block:
                player.AddBlock();
                break;
            case ActionType.Foul:
                player.AddFoul();
                team.AddFoul(period);
                break;
            case ActionType.FreeThrowMade:
                player.AddFreeThrowMade();
                player.AddPoints(1);
                team.AddPoints(1);
                if (team.IsHomeTeam)
                    period.HomePeriodScore += 1;
                else
                    period.AwayPeriodScore += 1;
                break;
            case ActionType.FreeThrowMissed:
                player.AddFreeThrowAttempt();
                break;
            case ActionType.Timeout:
                team.AddTimeout(period);
                break;
        }

        _game.ActionLog.Add(new ActionLogEntry
        {
            Timestamp = DateTime.UtcNow,
            Description = $"{player.FirstName} {player.LastName} {action}",
            PlayerInvolved = player,
            AssistingPlayer = assistingPlayer,
            ActionType = action
        });
    }

    public void ProcessTeam(ActionType action, Team team, bool offensive = false)
    {
        var period = _game.GetCurrentPeriod();

        switch (action)
        {
            case ActionType.TeamRebound:
                team.AddTeamRebound(offensive);
                break;
            case ActionType.CoachFoul:
                team.AddCoachFoul(period);
                break;
            case ActionType.BenchFoul:
                team.AddBenchFoul(period);
                break;
            case ActionType.TeamTurnover:
                team.AddTeamTurnover();
                break;
            case ActionType.Timeout:
                team.AddTimeout(period);
                break;
        }

        _game.ActionLog.Add(new ActionLogEntry
        {
            Timestamp = DateTime.UtcNow,
            Description = $"{team.TeamName} {action}",
            ActionType = action
        });
    }
}
