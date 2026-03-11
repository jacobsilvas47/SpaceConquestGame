using System;

public static class TimeFormatUtility
{
    public static string FormatDuration(double seconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(seconds);

        if (t.TotalHours >= 1)
            return $"{(int)t.TotalHours}h {t.Minutes}m {t.Seconds}s";

        if (t.TotalMinutes >= 1)
            return $"{t.Minutes}m {t.Seconds}s";

        return $"{t.Seconds}s";
    }
}