namespace DNETCommon.Utils;

public class ObjectHelper
{
    /// <summary>
    /// Определить являются ли все свойства объект в значении null
    /// </summary>
    /// <param name="obj">Объект для проверки</param>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <returns>Да / нет</returns>
    public static bool AllPropertiesIsNull<T>(T obj) where T: class
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        var properties = typeof(T).GetProperties();
        foreach (var prop in properties)
        {
            var val = prop.GetValue(obj);
            if (val != null)
            {
                return false;
            }
        }

        return true;
    }
}