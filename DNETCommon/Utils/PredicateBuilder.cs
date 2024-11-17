using System.Linq.Expressions;
using DNETCommon.Abstractions;
using DNETCommon.Enums;

namespace DNETCommon.Utils;

/// <summary>
/// Строитель предикатов для EntityFramework на основе [OutputSettings]
/// </summary>
public class PredicateBuilder<T> : IPredicateBuilder<T>
    where T : class
{
    private ParameterExpression _parameter;
    private Expression? _body;

    public PredicateBuilder(string parameterName = "item")
    {
        _parameter = Expression.Parameter(typeof(T), parameterName);
    }


    public IPredicateBuilder<T> AddSearch(string term, ConditionEnum condition = ConditionEnum.And, params string[] properties)
    {
        // Пример:
        // Нужно составить выражение для поиска по подстроке user => user.PersonalData.FirstName.Contains(term);
        // Вызов метода будет: AddTerm("Дмитрий", ConditionEnum.None, "PersonalData", "FirstName");

        if (string.IsNullOrEmpty(term))
        {
            return this;
        }
        
        // получаем цепочку доступа к параметру (например user.PersonalData.FirstName)
        var parameterAccessChain = _getPropertyAccessChain(_parameter, properties);
        
        // получаем выражение с проверкой цепочки на null parameterAccessChain != null
        var parameterAccessChainNotANull = Expression.NotEqual(parameterAccessChain, Expression.Constant(null));
        
        // получаем метод Contains типа string используя рефлексию
        var methodContains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        ArgumentNullException.ThrowIfNull(methodContains, nameof(methodContains));
        
        // создаем выражение user.PersonalData.FirstName.Contains(term)
        var parameterContainsTerm = Expression.Call(parameterAccessChain, methodContains, Expression.Constant(term));
        
        // получаем выражение с проверкой на null user.PersonalData.FirstName != null && user.PersonalData.FirstName.Contains(term)
        var resultExpression = Expression.AndAlso(parameterAccessChainNotANull, parameterContainsTerm);

        // если в теле выражения нет членов - добавляем текущее выражение
        if (_body is null)
        {
            _body = resultExpression;
        }
        // если есть
        else
        {
            // смотрим условие && или || и добавляем к телу текущее выражение в зависимости от условия
            switch (condition)
            {
                case ConditionEnum.And:
                    _body = Expression.AndAlso(_body, resultExpression);
                    break;
                case ConditionEnum.Or:
                    _body = Expression.OrElse(_body, resultExpression);
                    break;
            }
        }

        return this;
    }

    public IPredicateBuilder<T> AddDate(
        DateTime? dateFrom, 
        DateTime? dateTo, 
        ConditionEnum condition = ConditionEnum.And,
        params string[] properties)
    {
        // Пример:
        // Нужно получить выражение для выборки пользователей родившихся в определенный промежуток времени:
        // user => user.PersonalData.BirthDate >= даты-с && user.PersonalData.BirthDate <= даты-по
        // Вызов метода будет: AddDate(dateFrom, dateTo, ConditionEnum.None, "PersonalData", "BirthDate");
        
        if ((dateFrom is null && dateTo is null) || (dateFrom <= DateTime.MinValue && dateTo <= DateTime.MinValue))
        {
            return this;
        }

        // создаем цепочку доступа (user.PersonalData.BirthDate)
        var propertyAccessChain = _getPropertyAccessChain(_parameter, properties);
        if (propertyAccessChain.Type != typeof(DateTime))
        {
            return this;
        }
        
        // создаем выражение с проверкой на null user.PersonalData.BirthDate != null
        var propertyAccessChainNotANull = Expression.NotEqual(propertyAccessChain, Expression.Constant(null));

        // если дата с больше чем дата-по - меняем местами
        if (dateFrom > dateTo)
        {
            (dateTo, dateFrom) = (dateFrom, dateTo);
        }
        
        // создаем выражение для сравнения даты user.PersonalData.BirthDate >= dateFrom
        Expression? dateExpression = null;
        if (dateFrom is not null)
        {
            // если есть dateTo добавляем выражение user.PersonalData.BirthDate <= dateTo
            dateExpression = Expression.GreaterThanOrEqual(propertyAccessChain, Expression.Constant(dateFrom));
        }
        // если есть dateFrom трансформируем выражение в
        // user.PersonalData.BirthDate >= dateFrom && user.PersonalData.BirthDate <= dateTo
        if (dateTo is not null)
        {
            var dateToExpression = Expression.LessThanOrEqual(propertyAccessChain, Expression.Constant(dateTo));
            dateExpression = dateExpression != null ? Expression.AndAlso(dateExpression, dateToExpression) : dateToExpression;
        }

        // если по какой то причине не удалось собрать выражение - выходим
        if (dateExpression is null)
        {
            return this;
        }

        var resultExpression = Expression.AndAlso(propertyAccessChainNotANull, dateExpression);

        // если еще нет выражения в теле - просто присваиваем созданное выражение
        if (_body is null)
        {
            _body = resultExpression;
        }
        // если есть - добавляем с условием
        else
        {
            switch (condition)
            {
                case ConditionEnum.And:
                    _body = Expression.AndAlso(_body, resultExpression);
                    break;
                case ConditionEnum.Or:
                    _body = Expression.OrElse(_body, resultExpression);
                    break;
            }
        }

        return this;
    }
    
    public Expression<Func<T, bool>> Build()
    {
        if (_parameter is not null && _body is not null)
        {
            return Expression.Lambda<Func<T, bool>>(_body, _parameter);
        }

        return _ => false;
    }

    /// <summary>
    /// Формирует цепочку доступа к свойству (Например User.PersonalInfo.FirstName)
    /// </summary>
    /// <param name="parameter">Параметр выражения (В примере: user)</param>
    /// <param name="props">Цепочка свойств [В примере: PersonalInfo, FirstName]</param>
    /// <returns>Цепочка доступов (Например: user.PersonalInfo.FirstName)</returns>
    private Expression _getPropertyAccessChain(ParameterExpression parameter, params string[] props)
    {
        // если нет вложенных свойств - возвращаем сам параметр, будет обращение к нему (user)
        if (props is null || props.Length <= 0)
        {
            return parameter;
        }
        
        // первый шаг доступа (в примере user.PersonalData)
        var propertyAccessChain = Expression.Property(parameter, props[0]);

        // если больше нет свойств - возвращаем (в примере: user.PersonalData)
        if (props.Length == 1) return propertyAccessChain;

        // если есть - добавляем цепочки доступа user.PersonalData.FirstName и тд
        for (var i = 1; i < props.Length; i++)
        {
            propertyAccessChain = Expression.Property(propertyAccessChain, props[i]);
        }

        return propertyAccessChain;
    }
}