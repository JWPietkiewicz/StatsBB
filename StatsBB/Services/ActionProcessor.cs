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
                    period.HomeTeamPoints += points;
                else
                    period.AwayTeamPoints += points;
                break;
            case ActionType.ShotMissed:
                player.AddShotAttempt(isThreePoint);
                break;
            case ActionType.Rebound:
                player.AddRebound();
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
                    period.HomeTeamPoints += 1;
                else
                    period.AwayTeamPoints += 1;
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
}
