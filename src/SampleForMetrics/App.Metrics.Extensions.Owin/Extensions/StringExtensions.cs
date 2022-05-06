namespace App.Metrics.Extensions.Owin.Extensions
{
    using System.Diagnostics;

    internal static class StringExtensions
    {

        [DebuggerStepThrough]
        internal static string EnsureLeadingSlash(this string url)
        {
            if (!url.StartsWith("/"))
            {
                return "/" + url;
            }

            return url;
        }

        [DebuggerStepThrough]
        internal static bool IsPresent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        [DebuggerStepThrough]
        internal static string RemoveLeadingSlash(this string url)
        {
            if (url != null && url.StartsWith("/"))
            {
                url = url.Substring(1);
            }

            return url;
        }
    }
}