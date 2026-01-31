using System;

public static class Util
{
    private static readonly Random Rng = new();

    public static T GetRandomEnumValue<T>()
    {
        var enumValues = Enum.GetValues(typeof(T));
        var random = Rng.Next(0, enumValues.Length);
        return (T)enumValues.GetValue(random);
    }
}