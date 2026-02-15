namespace Audio.Util
{
    public static class Extensions
    {
        public static string Capitalize(this string str)
        {
            if (str.IsNullOrWhitespace()) return str;
            if (str.Length == 1) return str.ToUpper();
            return char.ToUpper(str[0]) + str[1..];
        }

        public static bool IsNullOrWhitespace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
    }
}