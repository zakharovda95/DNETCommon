using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DNETCommon.Extensions;

public static class EnumExtension
{
    public static string GetName(this Enum value)
    {
        string result = string.Empty;
        Type type = value.GetType();
        FieldInfo? fieldInfo = type.GetField(value.ToString());
        if (fieldInfo is null) return result;
        DisplayAttribute? displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
        if (displayAttribute is null) return result;
        string? name = displayAttribute.GetName();
        result = string.IsNullOrEmpty(name) ? fieldInfo.Name : name;
        return result;
    }
    
    public static string GetDescription(this Enum value)
    {
        string result = string.Empty;
        Type type = value.GetType();
        FieldInfo? fieldInfo = type.GetField(value.ToString());
        if (fieldInfo is null) return result;
        DisplayAttribute? displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
        if (displayAttribute is null) return result;
        string? description = displayAttribute.GetDescription();
        result = string.IsNullOrEmpty(description) ? fieldInfo.Name : description;
        return result;
    }
}