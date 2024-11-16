using System.Linq.Expressions;

namespace DNETCommon.Utils;

/// <summary>
/// Преобразовать параметр выражения (используется для мапинга предикатов)
/// </summary>
public class ExpressionParameterReplacer : ExpressionVisitor
{
    private ParameterExpression _oldParameter;
    private ParameterExpression _newParameter;
    
    public ExpressionParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        ArgumentNullException.ThrowIfNull(oldParameter, nameof(oldParameter));
        ArgumentNullException.ThrowIfNull(newParameter, nameof(newParameter));

        _oldParameter = oldParameter;
        _newParameter = newParameter;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        ArgumentNullException.ThrowIfNull(node, nameof(node));
        return node == _oldParameter ? _newParameter : base.VisitParameter(node);
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression == _oldParameter)
        {
            var targetProperty = _newParameter.Type.GetProperty(node.Member.Name);
            if (targetProperty is not null)
            {
                return Expression.MakeMemberAccess(_newParameter, targetProperty);
            }
            
            throw new Exception($"Member {node.Member.Name} is not definer on type {_newParameter.Type}");
        }
        
        return base.VisitMember(node);
    }
    
}