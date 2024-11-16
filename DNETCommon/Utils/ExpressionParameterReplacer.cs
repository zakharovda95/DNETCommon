using System.Linq.Expressions;

namespace DNETCommon.Utils;

/// <summary>
/// Преобразовать параметр выражения (используется для мапинга предикатов)
/// </summary>
public class ExpressionParameterReplacer : ExpressionVisitor
{
    /// <summary>
    /// Текущий параметр
    /// </summary>
    private ParameterExpression _oldParameter;
    
    /// <summary>
    /// Целевой параметр
    /// </summary>
    private ParameterExpression _newParameter;
    
    public ExpressionParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        ArgumentNullException.ThrowIfNull(oldParameter, nameof(oldParameter));
        ArgumentNullException.ThrowIfNull(newParameter, nameof(newParameter));

        _oldParameter = oldParameter;
        _newParameter = newParameter;
    }

    /// <summary>
    /// Заменить типа параметра в выражении
    /// </summary>
    /// <param name="node">Узел выражения</param>
    /// <returns>Узел / измененный узел</returns>
    /// <exception cref="Exception">Если в новом типе нет соответсвующего свойства - ошибка</exception>
    protected override Expression VisitMember(MemberExpression node)
    {
        // проходит по каждому узлу дерева выражения
        
        // если параметр узла дерева выражения равен текущему параметру -
        // нужно заменить на параметр с тем же именем но новым типом
        if (node.Expression == _oldParameter)
        {
            // пытаемся получить в новом параметре то же свойство
            // (например, если есть свойство Id в текущем параметре, оно должно быть и в целевом)
            var targetProperty = _newParameter.Type.GetProperty(node.Member.Name);
            if (targetProperty is not null)
            {
                // если найдено - создаем доступ к свойству у параметра (например выражение parameter.Id)
                return Expression.MakeMemberAccess(_newParameter, targetProperty);
            }
            // если не найден - выдаем ошибку, тк параметры не могут быть приведены
            throw new Exception($"Member {node.Member.Name} is not defined on type {_newParameter.Type}");
        }
        
        return base.VisitMember(node);
    }
    
}