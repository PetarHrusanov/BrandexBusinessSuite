namespace BrandexBusinessSuite.Methods;

using System.Reflection;

public static class FieldsValuesMethods
{
    public static string ReturnValueByClassAndName(Type type, string propertyName)
    {
        var field = type.GetField(propertyName, BindingFlags.Public | BindingFlags.Static);
        return (string)field!.GetValue(null)!;
    }
}