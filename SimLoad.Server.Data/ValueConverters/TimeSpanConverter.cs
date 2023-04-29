using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace SimLoad.Server.Data.ValueConverters;

public class TimeSpanConverter : ValueConverter<TimeSpan, double>
{
    public TimeSpanConverter()
        : base(
            v => (int)v.TotalSeconds,
            v => TimeSpan.FromSeconds(v)
        )
    {
    }
}