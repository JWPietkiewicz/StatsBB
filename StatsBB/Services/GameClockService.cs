using System;
using System.Windows.Threading;

namespace StatsBB.Services;

public static class GameClockService
{
    private static readonly DispatcherTimer _timer;

    static GameClockService()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (s, e) => Tick();
    }

    public static TimeSpan TimeLeft { get; private set; } = TimeSpan.FromMinutes(10);

    public static bool IsRunning => _timer.IsEnabled;

    public static event Action? TimeUpdated;

    private static void Tick()
    {
        if (TimeLeft > TimeSpan.Zero)
        {
            TimeLeft = TimeLeft - TimeSpan.FromSeconds(1);
            TimeUpdated?.Invoke();
        }
        else
        {
            Stop();
        }
    }

    public static void Start()
    {
        if (!IsRunning)
            _timer.Start();
    }

    public static void Stop()
    {
        if (IsRunning)
            _timer.Stop();
    }

    public static void Toggle()
    {
        if (IsRunning)
            Stop();
        else
            Start();
    }

    public static void Reset(TimeSpan? periodLength = null)
    {
        TimeLeft = periodLength ?? TimeSpan.FromMinutes(10);
        TimeUpdated?.Invoke();
    }

    public static string TimeLeftString => $"{(int)TimeLeft.TotalMinutes:D2}:{TimeLeft.Seconds:D2}";
}
