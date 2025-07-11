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
    public static string Period { get; private set; } = "Q1";
    public static bool IsRunning => _timer.IsEnabled;

    public static string StartStopLabel { get; private set; } = "START";
    public static bool StartStopEnabled { get; private set; } = true;

    public static event Action? EndPeriodRequested;
    public static event Action? FinalizeGameRequested;

    public static void SetState(string label, bool enabled)
    {
        StartStopLabel = label;
        StartStopEnabled = enabled;
        TimeUpdated?.Invoke();
    }

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
        StartStopLabel = "STOP";
        StartStopEnabled = true;
        TimeUpdated?.Invoke();
    }

    public static void Stop()
    {
        if (IsRunning)
            _timer.Stop();

        if (TimeLeft == TimeSpan.Zero)
        {
            StartStopLabel = "END PERIOD";
            StartStopEnabled = true;
        }
        else
        {
            StartStopLabel = "START";
            StartStopEnabled = true;
        }

        TimeUpdated?.Invoke();
    }

    public static void Toggle()
    {
        if (!StartStopEnabled)
            return;

        if (StartStopLabel == "END PERIOD")
        {
            EndPeriodRequested?.Invoke();
            return;
        }
        if (StartStopLabel == "FINALIZE GAME")
        {
            FinalizeGameRequested?.Invoke();
            return;
        }

        if (IsRunning)
            Stop();
        else
            Start();
    }

    public static void Reset(TimeSpan? periodLength = null, string? periodName = null, string? label = "START")
    {
        Period = periodName ?? "Q1";
        TimeLeft = periodLength ?? TimeSpan.FromMinutes(10);
        StartStopLabel = label ?? "START";
        StartStopEnabled = true;
        TimeUpdated?.Invoke();
    }

    public static string TimeLeftString => $"{Period} {(int)TimeLeft.TotalMinutes:D2}:{TimeLeft.Seconds:D2}";
}
