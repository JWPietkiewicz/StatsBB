using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StatsBB.Domain;

public class Player : INotifyPropertyChanged
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
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
    public bool IsTeamA { get; set; }
    public string DisplayName
    {
        get { return Number.ToString(); }
    }
    public string Name
    {
        get { return FirstName + "" + LastName; }
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
}
