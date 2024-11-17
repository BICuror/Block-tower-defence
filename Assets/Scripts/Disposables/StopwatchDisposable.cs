using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public sealed class StopwatchDisposable : IDisposable
{
    private Stopwatch _stopwatch;
    private string _stopwatchName;

    public StopwatchDisposable(string stopwatchName)
    {
        _stopwatchName = stopwatchName;
        _stopwatch = new();
        _stopwatch.Start();
    }

    public void Dispose()
    {   
        _stopwatch.Stop();
        Debug.Log($"[{_stopwatchName}] Elapsed time: {_stopwatch.Elapsed}");
    }
}