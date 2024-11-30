using System;

public static class TaskUtility
{
    public static void LogAsync(Exception e)
    {
        if (e is not OperationCanceledException) throw e;
    }
}