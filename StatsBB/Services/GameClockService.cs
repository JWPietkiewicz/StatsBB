using System;
using System.Windows.Threading;

namespace StatsBB.Services;

public static class GameClockService
{
    private static readonly DispatcherTimer _timer;
    private static TimeSpan _maxTime = TimeSpan.FromMinutes(10);

    static GameClockService()
    {
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (s, e) => Tick();
    }

    public static TimeSpan TimeLeft { get; private set; } = TimeSpan.FromMinutes(10);
    public static string Period { get; private set; } = "Q1";
    public static bool IsRunning => _timer.IsEnabled;

    public static bool TeamAPossession { get; private set; } = true;
    public static bool TeamAArrow { get; private set; } = true;

    public static string StartStopLabel { get; private set; } = "START";
    public static bool StartStopEnabled { get; private set; } = true;

    public static event Action? EndPeriodRequested;
    public static event Action? FinalizeGameRequested;
    public static event Action? ClockStarted;
    public static event Action? ClockStopped;

    public static void SetState(string label, bool enabled)
    {
        StartStopLabel = label;
        StartStopEnabled = enabled;
        TimeUpdated?.Invoke();
    }

    public static event Action? TimeUpdated;

    public static void SetPeriodDisplay(string text)
    {
        Period = text;
        TimeUpdated?.Invoke();
    }

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
        ClockStarted?.Invoke();
        StartStopLabel = "STOP";
        StartStopEnabled = true;
        TimeUpdated?.Invoke();
    }

    public static void Stop()
    {
        if (IsRunning)
            _timer.Stop();
        ClockStopped?.Invoke();

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

    private static void AddTime(TimeSpan delta)
    {
        TimeLeft += delta;
        if (TimeLeft < TimeSpan.Zero)
            TimeLeft = TimeSpan.Zero;
        if (TimeLeft > _maxTime)
            TimeLeft = _maxTime;
        if (TimeLeft > TimeSpan.Zero && (StartStopLabel == "END PERIOD" || StartStopLabel == "FINALIZE GAME"))
        {
            StartStopLabel = "START";
            StartStopEnabled = true;
        }
        TimeUpdated?.Invoke();
    }

    public static void AddMinute() => AddTime(TimeSpan.FromMinutes(1));
    public static void SubtractMinute() => AddTime(TimeSpan.FromMinutes(-1));
    public static void AddSecond() => AddTime(TimeSpan.FromSeconds(1));
    public static void SubtractSecond() => AddTime(TimeSpan.FromSeconds(-1));

    public static void Reset(TimeSpan? periodLength = null, string? periodName = null, string? label = "START")
    {
        SetPeriodDisplay(periodName ?? "Q1");
        _maxTime = periodLength ?? TimeSpan.FromMinutes(10);
        TimeLeft = _maxTime;
        StartStopLabel = label ?? "START";
        StartStopEnabled = true;
        TimeUpdated?.Invoke();
    }

    public static void SetPossession(bool teamA)
    {
        TeamAPossession = teamA;
        TimeUpdated?.Invoke();
    }

    public static void SwapPossession()
    {
        TeamAPossession = !TeamAPossession;
        TimeUpdated?.Invoke();
    }

    public static void SetArrow(bool teamA)
    {
        TeamAArrow = teamA;
        TimeUpdated?.Invoke();
    }

    public static void SwapArrow()
    {
        TeamAArrow = !TeamAArrow;
        TimeUpdated?.Invoke();
    }

    public static void SwapPossessionAndArrow()
    {
        TeamAPossession = !TeamAPossession;
        TeamAArrow = !TeamAArrow;
        TimeUpdated?.Invoke();
    }

    public static string TimeLeftString => $"{(int)TimeLeft.TotalMinutes:D2}:{TimeLeft.Seconds:D2}";
}
