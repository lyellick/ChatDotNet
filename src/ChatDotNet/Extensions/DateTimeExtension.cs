namespace ChatDotNet.Extensions
{
    public static class DateTimeExtension
    {
        public static string AsTimeAgo(this DateTime dateTime)
        {
            TimeSpan timeSpan = DateTimeOffset.UtcNow.Subtract(dateTime);

            return timeSpan.TotalSeconds switch
            {
                <= 60 => $"{timeSpan.Seconds} seconds",

                _ => timeSpan.TotalMinutes switch
                {
                    <= 1 => "a minute",
                    < 60 => $"{timeSpan.Minutes} minutes",
                    _ => timeSpan.TotalHours switch
                    {
                        <= 1 => "an hour",
                        < 24 => $"{timeSpan.Hours} hours",
                        _ => timeSpan.TotalDays switch
                        {
                            <= 1 => "yesterday",
                            <= 30 => $"{timeSpan.Days} days",

                            <= 60 => "a month",
                            < 365 => $"{timeSpan.Days / 30} months",

                            <= 365 * 2 => "a year",
                            _ => $"{timeSpan.Days / 365} years"
                        }
                    }
                }
            };
        }

        public static string AsTimeAgo(this DateTimeOffset dateTime)
        {
            TimeSpan timeSpan = DateTimeOffset.Now.Subtract(dateTime);

            return timeSpan.TotalSeconds switch
            {
                <= 60 => $"{timeSpan.Seconds} seconds",

                _ => timeSpan.TotalMinutes switch
                {
                    <= 1 => "a minute",
                    < 60 => $"{timeSpan.Minutes} minutes",
                    _ => timeSpan.TotalHours switch
                    {
                        <= 1 => "an hour",
                        < 24 => $"{timeSpan.Hours} hours",
                        _ => timeSpan.TotalDays switch
                        {
                            <= 1 => "yesterday",
                            <= 30 => $"{timeSpan.Days} days",

                            <= 60 => "a month",
                            < 365 => $"{timeSpan.Days / 30} months",

                            <= 365 * 2 => "a year",
                            _ => $"{timeSpan.Days / 365} years"
                        }
                    }
                }
            };
        }

        public static string AsTimeRemaining(this TimeSpan t)
        {
            if (t.TotalSeconds <= 1)
            {
                return $@"{t:s\.ff} seconds";
            }
            if (t.TotalMinutes <= 1)
            {
                return $@"{t:%s} seconds";
            }
            if (t.TotalHours <= 1)
            {
                return $@"{t:%m} minutes";
            }
            if (t.TotalDays <= 1)
            {
                return $@"{t:%h} hours";
            }

            return $@"{t:%d} days";
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
