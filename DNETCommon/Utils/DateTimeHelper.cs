namespace DNETCommon.Utils;

public interface IDateTimeHelper
{
    DateTime GetTimeNow();
}

public class DateTimeHelper : IDateTimeHelper
{
    public DateTime GetTimeNow()
    {
        return DateTime.Now;
    }
}