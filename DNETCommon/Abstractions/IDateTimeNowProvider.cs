using DNETCommon.Enums;

namespace DNETCommon.Abstractions;

/// <summary>
/// Предоставляет доступ к текущим дате и времени
/// </summary>
public interface IDateTimeNowProvider
{
    /// <summary>
    /// Текущее дата и время в локальном формате
    /// </summary>
    DateTime Now { get; }
    
    /// <summary>
    /// Текущие дата и время в формате UTC (00 по Гринвичу)
    /// </summary>
    DateTime NowUtc { get; }

    /// <summary>
    /// Привести к Unix Timestamp (например: 123456789121)
    /// </summary>
    /// <param name="format">Формат времени локальное или UTC</param>
    /// <returns></returns>
    long ToUnix(DateTimeFormatEnum format = DateTimeFormatEnum.Local);

    /// <summary>
    /// Привести к ISO (например: Local = 2024-11-14T15:00:00+03:00 или Utc = 2024-11-14T12:00:00Z)
    /// </summary>
    /// <param name="format">Формат времени локальное или UTC</param>
    /// <returns></returns>
    string ToIso(DateTimeFormatEnum format = DateTimeFormatEnum.Local);
}