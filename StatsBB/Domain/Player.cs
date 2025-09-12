using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;

namespace StatsBB.Domain;

public class Player : INotifyPropertyChanged, IDataErrorInfo
{
    public int Id { get; set; }
    
    private int _number;
    public int Number 
    { 
        get => _number;
        set
        {
            if (_number != value)
            {
                _number = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasValidationErrors));
            }
        }
    }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    private bool _s5;
    /// <summary>
    /// Indicates whether the player belongs to the starting five.
    /// </summary>
    public bool S5
    {
        get => _s5;
        set
        {
            if (_s5 != value)
            {
                _s5 = value;
                OnPropertyChanged();
            }
        }
    }
    /// <summary>
    /// Indicates whether the player is on the game roster. Only players with
    /// <c>IsPlaying</c> set to <c>true</c> should appear on the main view and in
    /// the stats tab.
    /// </summary>
    private bool _isPlaying = false;
    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            if (_isPlaying != value)
            {
                _isPlaying = value;
                OnPropertyChanged();
            }
        }
    }

    private bool _isCaptain;
    /// <summary>
    /// Indicates whether the player is the team's captain.
    /// </summary>
    public bool IsCaptain
    {
        get => _isCaptain;
        set
        {
            if (_isCaptain != value)
            {
                _isCaptain = value;
                OnPropertyChanged();
            }
        }
    }
    public bool IsTeamA { get; set; }
    public string DisplayName
    {
        get { return Number == 0 ? "" : Number.ToString("00"); }
    }
    public string Name
    {
        get { return $"{FirstName} {LastName}".Trim(); }
    }

    public int Points { get; set; }
    public int Assists { get; set; }
    public int Rebounds { get; set; }
    public int OffensiveRebounds { get; set; }
    public int DefensiveRebounds { get; set; }
    public int Blocks { get; set; }
    public int Steals { get; set; }
    public int Turnovers { get; set; }
    public int FoulsCommitted { get; set; }
    public int ShotAttempts2pt { get; set; }
    public int ShotsMade2pt { get; set; }
    public int ShotAttempts3pt { get; set; }
    public int ShotsMade3pt { get; set; }
    public int FieldGoalsMade => ShotsMade2pt + ShotsMade3pt;
    public int FieldGoalsAttempted => ShotAttempts2pt + ShotAttempts3pt;
    public int FreeThrowsAttempted { get; set; }
    public int FreeThrowsMade { get; set; }

    public void AddPoints(int points)
    {
        Points += points;
    }
    public void AddAssist() => Assists++;
    public void AddRebound(bool offensive)
    {
        Rebounds++;
        if (offensive)
            OffensiveRebounds++;
        else
            DefensiveRebounds++;
    }
    public void AddBlock() => Blocks++;
    public void AddSteal() => Steals++;
    public void AddTurnover() => Turnovers++;
    public void AddFoul() => FoulsCommitted++;

    public void AddShotAttempt(bool isThreePoint)
    {
        if (isThreePoint)
            ShotAttempts3pt++;
        else
            ShotAttempts2pt++;
    }

    public void AddShotMade(bool isThreePoint)
    {
        AddShotAttempt(isThreePoint);
        if (isThreePoint)
            ShotsMade3pt++;
        else
            ShotsMade2pt++;
    }

    public void AddFreeThrowAttempt() => FreeThrowsAttempted++;

    public void AddFreeThrowMade()
    {
        FreeThrowsAttempted++;
        FreeThrowsMade++;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Validation support
    public Team? ParentTeam { get; set; }
    
    public bool HasValidationErrors => !string.IsNullOrEmpty(this[nameof(Number)]);

    public string Error => string.Empty;

    public string this[string columnName]
    {
        get
        {
            switch (columnName)
            {
                case nameof(Number):
                    // No validation for players with no number (0) - they are allowed
                    if (Number == 0)
                        return string.Empty;
                        
                    if (Number < 0)
                        return "Player number cannot be negative";
                    if (Number > 99)
                        return "Player number must be 99 or less";
                    if (ParentTeam != null && !ParentTeam.IsPlayerNumberAvailable(Number, this))
                        return $"Number {Number:00} is already taken by another player";
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }
    }
}
