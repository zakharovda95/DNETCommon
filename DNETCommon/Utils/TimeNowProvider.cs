using DNETCommon.Abstractions;
using DNETCommon.Infrastructure.Enums;

namespace DNETCommon.Utils;

public class TimeNowProvider : ITimeNowProvider
{
    public DateTime Now => DateTime.Now;
    public DateTime NowUtc => DateTime.UtcNow;
    
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
}