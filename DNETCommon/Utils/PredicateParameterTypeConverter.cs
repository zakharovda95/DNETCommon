using System.Linq.Expressions;
using DNETCommon.Abstractions;

namespace DNETCommon.Utils;

/// <summary>
/// Преобразует тип параметра предикатов
/// </summary>
/// <typeparam name="TCurrent">Текущий тип параметра</typeparam>
/// <typeparam name="TTarget">Целевой тип параметра</typeparam>
public class PredicateParameterTypeConverter<TCurrent, TTarget> : IPredicateConverter<TCurrent, TTarget> 
    where TCurrent : class 
    where TTarget : class
{
    /// <summary>
    /// Заменить тип параметра предиката
    /// </summary>
    /// <param name="predicate">Исходный предикат</param>
    /// <returns>Предикат с новым типом</returns>
    public virtual Expression<Func<TTarget, bool>> TransformPredicate(Expression<Func<TCurrent, bool>> predicate)
    {
        var oldParameter = predicate.Parameters[0];
        var newParameter = Expression.Parameter(typeof(TTarget), oldParameter.Name);
        
        var parameterReplacer = new ExpressionParameterReplacer(oldParameter, newParameter);
        var newBody = parameterReplacer.Visit(predicate.Body);
        
        return Expression.Lambda<Func<TTarget, bool>>(newBody, newParameter);
    }
}