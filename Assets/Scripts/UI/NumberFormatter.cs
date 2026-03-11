using System;

public static class NumberFormatter
{
    public static string Format(double value)
    {
        if (value >= 1_000_000_000)
            return (value / 1_000_000_000d).ToString("0.#") + "b";

        if (value >= 1_000_000)
            return (value / 1_000_000d).ToString("0.#") + "m";

        if (value >= 1_000)
            return (value / 1_000d).ToString("0.#") + "k";

        return Math.Floor(value).ToString();
    }
}