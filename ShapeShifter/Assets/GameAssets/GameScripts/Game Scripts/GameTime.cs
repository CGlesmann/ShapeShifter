using System;
using UnityEngine;

public static class GameTime
{
    // Declaring Time Constants
    public const int DAY_IN_SECOND = 86400, HOUR_IN_SECOND = 3600, MINUTE_IN_SECOND = 60;

    public static string GetGameTimeFormat(float s)
    {
        // Declaring the store variable
        string time = "";
        int seconds = Mathf.RoundToInt(s);

        // Checking days
        int dayCount = Mathf.FloorToInt(seconds / DAY_IN_SECOND);

        // Checking hours
        seconds -= (dayCount * DAY_IN_SECOND);
        int hourCount = Mathf.FloorToInt(seconds / HOUR_IN_SECOND);

        // Checking Minutes
        seconds -= (hourCount * HOUR_IN_SECOND);
        int minuteCount = Mathf.FloorToInt(seconds / MINUTE_IN_SECOND);

        // Removing minutes from seconds timer
        seconds -= (minuteCount * 60);

        // Formatting the time string
        if (dayCount > 0)
            time += dayCount < 10 ? "0" + dayCount + ":" : dayCount + ":";
        if (hourCount > 0)
            time += hourCount < 10 ? "0" + hourCount + ":" : hourCount + ":";

        time += minuteCount < 10 ? "0" + minuteCount + ":" : minuteCount + ":";
        time += seconds < 10 ? "0" + seconds : seconds.ToString();

        // Returning the resulting time
        return time;
    }

    public static int GetMinuteCount(float s) { return Mathf.FloorToInt(s / MINUTE_IN_SECOND); }
}
