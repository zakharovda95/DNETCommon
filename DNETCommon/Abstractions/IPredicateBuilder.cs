using System.Linq.Expressions;
using DNETCommon.Enums;

namespace DNETCommon.Abstractions;

/// <summary>
/// Строитель предикатов
/// </summary>
public interface IPredicateBuilder<T> where T : class
{
    /// <summary>
    /// Добавить в выражение поиск по названию
    /// </summary>
    /// <param name="term">Подстрока для поиска</param>
    /// <param name="condition">&& или || для случаев когда уже есть выражение и нужно добавить еще условие, если выражения нет - игнорируется</param>
    /// <param name="properties">Цепочка доступов к свойству (Например user.FirstName)</param>
    IPredicateBuilder<T> AddSearch(string term, ConditionEnum condition = ConditionEnum.And, params string[] properties);
    
    /// <summary>
    /// Добавит в выражение поиск по дате С
    /// </summary>
    /// <param name="dateFrom">Дата поиска с</param>
    /// <param name="dateTo">Дата поиска по</param>
    /// <param name="condition">&& или || для случаев когда уже есть выражение и нужно добавить еще условие, если выражения нет - игнорируется</param>
    /// <param name="properties">Цепочка доступов к свойству (Например user.BirthDate)</param>
    IPredicateBuilder<T> AddDate(DateTime? dateFrom, DateTime? dateTo, ConditionEnum condition = ConditionEnum.And, params string[] properties);
    
    /// <summary>
    /// Собрать выражение в лямбду
    /// </summary>
    /// <returns>Лямбда выражение (Например: x => x + 1)</returns>
    Expression<Func<T, bool>> Build();
}