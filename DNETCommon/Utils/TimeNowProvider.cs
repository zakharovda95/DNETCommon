using DNETCommon.Abstractions;
using DNETCommon.Infrastructure.Enums;

namespace DNETCommon.Utils;

public class TimeNowProvider : ITimeNowProvider
{
    public DateTime Now => DateTime.Now;
    public DateTime NowUtc => DateTime.UtcNow;
    
    public static DateTime NowStatic => DateTime.Now;
    public static DateTime NowUtcStatic => DateTime.UtcNow;
    
    public long ToUnix(DateTimeFormatEnum format = DateTimeFormatEnum.Local)
    {
        return format switch
        {
            DateTimeFormatEnum.Local => new DateTimeOffset(Now).ToUnixTimeMilliseconds(),
            DateTimeFormatEnum.Utc => new DateTimeOffset(NowUtc).ToUnixTimeMilliseconds(),
            _ => -1
        };
    }

    public string ToIso(DateTimeFormatEnum format = DateTimeFormatEnum.Local)
    {
        return format switch
        {
            DateTimeFormatEnum.Local => Now.ToString("o"),
            DateTimeFormatEnum.Utc => NowUtc.ToString("o"),
            _ => string.Empty
        };
    }
    
    public static long ToUnixStatic(DateTimeFormatEnum format = DateTimeFormatEnum.Local)
    {
        return format switch
        {
            DateTimeFormatEnum.Local => new DateTimeOffset(NowStatic).ToUnixTimeMilliseconds(),
            DateTimeFormatEnum.Utc => new DateTimeOffset(NowUtcStatic).ToUnixTimeMilliseconds(),
            _ => -1
        };
    }

    public static string ToIsoStatic(DateTimeFormatEnum format = DateTimeFormatEnum.Local)
    {
        return format switch
        {
            DateTimeFormatEnum.Local => NowStatic.ToString("o"),
            DateTimeFormatEnum.Utc => NowUtcStatic.ToString("o"),
            _ => string.Empty
        };
    }
}