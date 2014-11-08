namespace KerbalSpaceProgram.Api
{
    internal static class StringExtensions
    {
        [PublicAPI, StringFormatMethod("format")]
        public static string With([NotNull] this string format, object arg1)
        {
            return string.Format(format, arg1);
        }

        [PublicAPI, StringFormatMethod("format")]
        public static string With([NotNull] this string format, object arg1, object arg2)
        {
            return string.Format(format, arg1, arg2);
        }

        [PublicAPI, StringFormatMethod("format")]
        public static string With([NotNull] this string format, object arg1, object arg2, object arg3)
        {
            return string.Format(format, arg1, arg2, arg3);
        }

        [PublicAPI, StringFormatMethod("format")]
        public static string With([NotNull] this string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }
}
