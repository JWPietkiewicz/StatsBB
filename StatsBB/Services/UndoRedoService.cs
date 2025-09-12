using System;
using System.Collections.Generic;
using System.Linq;
using StatsBB.Domain;

namespace StatsBB.Services;

/// <summary>
/// Service for managing undo/redo operations for game actions
/// </summary>
public class UndoRedoService
{
    private readonly Stack<IUndoableAction> _undoStack = new();
    private readonly Stack<IUndoableAction> _redoStack = new();
    private readonly int _maxUndoSteps;

    /// <summary>
    /// Event raised when undo/redo state changes
    /// </summary>
    public event EventHandler? StateChanged;

    /// <summary>
    /// Gets whether undo is available
    /// </summary>
    public bool CanUndo => _undoStack.Count > 0;

    /// <summary>
    /// Gets whether redo is available
    /// </summary>
    public bool CanRedo => _redoStack.Count > 0;

    /// <summary>
    /// Gets the number of actions that can be undone
    /// </summary>
    public int UndoCount => _undoStack.Count;

    /// <summary>
    /// Gets the number of actions that can be redone
    /// </summary>
    public int RedoCount => _redoStack.Count;

    /// <summary>
    /// Initializes a new instance of UndoRedoService
    /// </summary>
    /// <param name="maxUndoSteps">Maximum number of undo steps to keep</param>
    public UndoRedoService(int maxUndoSteps = 50)
    {
        _maxUndoSteps = maxUndoSteps;
    }

    /// <summary>
    /// Executes an action and adds it to the undo stack
    /// </summary>
    /// <param name="action">Action to execute</param>
    public void ExecuteAction(IUndoableAction action)
    {
        if (action == null) return;

        try
        {
            action.Execute();
            
            // Clear redo stack when new action is executed
            _redoStack.Clear();
            
            // Add to undo stack
            _undoStack.Push(action);
            
            // Limit stack size
            while (_undoStack.Count > _maxUndoSteps)
            {
                var oldest = _undoStack.ToArray().Last();
                var tempStack = new Stack<IUndoableAction>();
                
                // Rebuild stack without the oldest item
                while (_undoStack.Count > 1)
                {
                    tempStack.Push(_undoStack.Pop());
                }
                _undoStack.Pop(); // Remove the oldest
                
                while (tempStack.Count > 0)
                {
                    _undoStack.Push(tempStack.Pop());
                }
            }
            
            OnStateChanged();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to execute action: {ex.Message}");
        }
    }

    /// <summary>
    /// Undoes the last action
    /// </summary>
    /// <returns>True if undo was successful</returns>
    public bool Undo()
    {
        if (!CanUndo) return false;

        try
        {
            var action = _undoStack.Pop();
            action.Undo();
            _redoStack.Push(action);
            
            OnStateChanged();
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to undo action: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Redoes the last undone action
    /// </summary>
    /// <returns>True if redo was successful</returns>
    public bool Redo()
    {
        if (!CanRedo) return false;

        try
        {
            var action = _redoStack.Pop();
            action.Execute();
            _undoStack.Push(action);
            
            OnStateChanged();
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to redo action: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Clears all undo/redo history
    /// </summary>
    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
        OnStateChanged();
    }

    /// <summary>
    /// Gets a description of the next action that can be undone
    /// </summary>
    /// <returns>Description or empty string if no undo available</returns>
    public string GetUndoDescription()
    {
        return CanUndo ? _undoStack.Peek().Description : string.Empty;
    }

    /// <summary>
    /// Gets a description of the next action that can be redone
    /// </summary>
    /// <returns>Description or empty string if no redo available</returns>
    public string GetRedoDescription()
    {
        return CanRedo ? _redoStack.Peek().Description : string.Empty;
    }

    private void OnStateChanged()
    {
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>
/// Interface for actions that can be undone and redone
/// </summary>
public interface IUndoableAction
{
    /// <summary>
    /// Description of the action for UI display
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Executes the action
    /// </summary>
    void Execute();

    /// <summary>
    /// Undoes the action
    /// </summary>
    void Undo();
}

/// <summary>
/// Undoable action for player statistics changes
/// </summary>
public class PlayerStatAction : IUndoableAction
{
    private readonly Player _player;
    private readonly Action _executeAction;
    private readonly Action _undoAction;

    public string Description { get; }

    public PlayerStatAction(Player player, string description, Action executeAction, Action undoAction)
    {
        _player = player ?? throw new ArgumentNullException(nameof(player));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        _executeAction = executeAction ?? throw new ArgumentNullException(nameof(executeAction));
        _undoAction = undoAction ?? throw new ArgumentNullException(nameof(undoAction));
    }

    public void Execute()
    {
        _executeAction();
    }

    public void Undo()
    {
        _undoAction();
    }
}

/// <summary>
/// Undoable action for team score changes
/// </summary>
public class TeamScoreAction : IUndoableAction
{
    private readonly Team _team;
    private readonly int _oldScore;
    private readonly int _newScore;

    public string Description { get; }

    public TeamScoreAction(Team team, int oldScore, int newScore, string description)
    {
        _team = team ?? throw new ArgumentNullException(nameof(team));
        _oldScore = oldScore;
        _newScore = newScore;
        Description = description ?? $"Change {team.TeamName} score from {oldScore} to {newScore}";
    }

    public void Execute()
    {
        _team.Points = _newScore;
    }

    public void Undo()
    {
        _team.Points = _oldScore;
    }
}

/// <summary>
/// Undoable action for player substitutions
/// </summary>
public class SubstitutionAction : IUndoableAction
{
    private readonly Player _playerIn;
    private readonly Player _playerOut;

    public string Description { get; }

    public SubstitutionAction(Player playerIn, Player playerOut)
    {
        _playerIn = playerIn ?? throw new ArgumentNullException(nameof(playerIn));
        _playerOut = playerOut ?? throw new ArgumentNullException(nameof(playerOut));
        Description = $"Substitute {playerOut.FirstName} {playerOut.LastName} with {playerIn.FirstName} {playerIn.LastName}";
    }

    public void Execute()
    {
        _playerOut.IsPlaying = false;
        _playerIn.IsPlaying = true;
    }

    public void Undo()
    {
        _playerIn.IsPlaying = false;
        _playerOut.IsPlaying = true;
    }
}

/// <summary>
/// Composite action that groups multiple actions together
/// </summary>
public class CompositeAction : IUndoableAction
{
    private readonly List<IUndoableAction> _actions;

    public string Description { get; }

    public CompositeAction(string description, params IUndoableAction[] actions)
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));
        _actions = actions?.ToList() ?? throw new ArgumentNullException(nameof(actions));
    }

    public void Execute()
    {
        foreach (var action in _actions)
        {
            action.Execute();
        }
    }

    public void Undo()
    {
        // Undo in reverse order
        for (int i = _actions.Count - 1; i >= 0; i--)
        {
            _actions[i].Undo();
        }
    }
}

/// <summary>
/// Factory for creating common undoable actions
/// </summary>
public static class UndoableActionFactory
{
    /// <summary>
    /// Creates an action for adding points to a player
    /// </summary>
    public static PlayerStatAction CreateAddPointsAction(Player player, int points, bool isThreePoint = false)
    {
        return new PlayerStatAction(
            player,
            $"Add {points} points to {player.FirstName} {player.LastName}",
            () => 
            {
                player.AddPoints(points);
                player.AddShotMade(isThreePoint);
            },
            () => 
            {
                player.Points = Math.Max(0, player.Points - points);
                if (isThreePoint)
                {
                    player.ShotsMade3pt = Math.Max(0, player.ShotsMade3pt - 1);
                    player.ShotAttempts3pt = Math.Max(0, player.ShotAttempts3pt - 1);
                }
                else
                {
                    if (player.ShotsMade2pt > 0)
                    {
                        player.ShotsMade2pt = Math.Max(0, player.ShotsMade2pt - 1);
                        player.ShotAttempts2pt = Math.Max(0, player.ShotAttempts2pt - 1);
                    }
                }
            });
    }

    /// <summary>
    /// Creates an action for recording a missed shot
    /// </summary>
    public static PlayerStatAction CreateMissedShotAction(Player player, bool isThreePoint = false)
    {
        return new PlayerStatAction(
            player,
            $"Missed {(isThreePoint ? "3-point" : "2-point")} shot by {player.FirstName} {player.LastName}",
            () => player.AddShotAttempt(isThreePoint),
            () => 
            {
                if (isThreePoint)
                {
                    player.ShotAttempts3pt = Math.Max(0, player.ShotAttempts3pt - 1);
                }
                else
                {
                    player.ShotAttempts2pt = Math.Max(0, player.ShotAttempts2pt - 1);
                }
            });
    }

    /// <summary>
    /// Creates an action for adding a rebound to a player
    /// </summary>
    public static PlayerStatAction CreateReboundAction(Player player, bool isOffensive)
    {
        return new PlayerStatAction(
            player,
            $"{(isOffensive ? "Offensive" : "Defensive")} rebound by {player.FirstName} {player.LastName}",
            () => player.AddRebound(isOffensive),
            () =>
            {
                player.Rebounds = Math.Max(0, player.Rebounds - 1);
                if (isOffensive)
                {
                    player.OffensiveRebounds = Math.Max(0, player.OffensiveRebounds - 1);
                }
                else
                {
                    player.DefensiveRebounds = Math.Max(0, player.DefensiveRebounds - 1);
                }
            });
    }

    /// <summary>
    /// Creates an action for adding an assist to a player
    /// </summary>
    public static PlayerStatAction CreateAssistAction(Player player)
    {
        return new PlayerStatAction(
            player,
            $"Assist by {player.FirstName} {player.LastName}",
            () => player.AddAssist(),
            () => player.Assists = Math.Max(0, player.Assists - 1));
    }
}