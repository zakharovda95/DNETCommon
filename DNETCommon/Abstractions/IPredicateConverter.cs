using System.Linq.Expressions;

namespace DNETCommon.Abstractions;

/// <summary>
/// Трансформирует предикат
/// </summary>
/// <typeparam name="TCurrent">Текущий тип параметра предиката</typeparam>
/// <typeparam name="TTarget">Целевой тип параметра предиката предикат</typeparam>
public interface IPredicateConverter<TCurrent, TTarget> 
    where TCurrent : class
    where TTarget : class
{
    /// <summary>
    /// Трансформировать предикат
    /// </summary>
    /// <param name="predicate">Исходный предикат</param>
    /// <returns>Трансформированный предикат</returns>
    public Expression<Func<TTarget, bool>> TransformPredicate(Expression<Func<TCurrent, bool>> predicate);
}