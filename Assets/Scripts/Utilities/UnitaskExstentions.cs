using System;

public static class UnitaskExstentions
{
    public static void LogAsync(this Exception exception)
    {
        if (exception is not OperationCanceledException) throw exception;
    }
}