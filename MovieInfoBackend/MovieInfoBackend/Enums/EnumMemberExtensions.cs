using System.Reflection;
using System.Runtime.Serialization;
using Serilog;

public static class EnumMemberExtensions
{
    // (Mostly) taken from https://stackoverflow.com/questions/10418651/using-enummemberattribute-and-doing-automatic-string-conversions
    public static string? ToEnumString<T>(this T type)
        where T : Enum
    {
        Type? enumType = typeof(T);
        if (enumType == null)
        {
            Log.Warning("Enum type is invalid: '" + type + "'.");
            return null;
        }
        string? enumName = Enum.GetName(enumType, type);
        if (enumName == null)
        {
            Log.Warning("Enum type is invalid: '" + type + "'.");
            return null;
        }
        FieldInfo? enumFieldInfo = enumType.GetField(enumName);
        if (enumFieldInfo == null)
        {
            Log.Warning("Enum type is invalid: '" + type + "'.");
            return null;
        }
        EnumMemberAttribute enumMemberAttribute = ((EnumMemberAttribute[])enumFieldInfo.GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
        return enumMemberAttribute.Value;
    }

    public static T? ToEnum<T>(this string str)
        where T : Enum
    {
        Type enumType = typeof(T);
        foreach (string? enumName in Enum.GetNames(enumType))
        {
            FieldInfo? enumFieldInfo = enumType.GetField(enumName);
            if (enumFieldInfo == null)
            {
                Log.Warning("Enum value is invalid: '" + str + "'.");
                return default;
            }
            EnumMemberAttribute? enumMemberAttribute = ((EnumMemberAttribute[])enumFieldInfo.GetCustomAttributes(typeof(EnumMemberAttribute), true)).Single();
            if (enumMemberAttribute.Value == str) return (T)Enum.Parse(enumType, enumName);
        }

        Log.Warning("Enum value is invalid: '" + str + "'.");
        return default;
    }
}