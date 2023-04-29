namespace SimLoad.Common;

public static class DateTimeExtensions
{

    public static DateTime TruncateToMinute(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, 0, DateTimeKind.Utc);
    }
    
}