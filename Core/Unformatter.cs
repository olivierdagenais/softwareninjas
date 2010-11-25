using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SoftwareNinjas.Core
{
    /// <summary>
    /// A class to parse format strings and decode formatted versions back into component strings.
    /// </summary>
    public class Unformatter
    {
        private const RegexOptions StandardOptions = RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant;

        /// <summary>
        /// Performs the reverse of <see cref="StringExtensions.FormatInvariant(String, Object[])"/> by trying to
        /// determine what the string representations of the objects were when they were formatted into
        /// <paramref name="format"/> to yield <paramref name="formatted"/>.
        /// </summary>
        /// 
        /// <param name="formatted">
        /// The result of formatting <paramref name="format"/> with zero or more objects.
        /// </param>
        /// 
        /// <param name="format">
        /// A composite format string.
        /// </param>
        /// 
        /// <returns>
        /// An <see cref="IList{String}"/> representing the string representations of the objects formatted into
        /// <paramref name="format"/>.
        /// </returns>
        public static IList<string> UnformatInvariant(string formatted, string format)
        {
            var result = new List<string> ();
            var placeholderPattern = new Regex (@"{(?<index>\d+)}", StandardOptions);
            var placeholders = placeholderPattern.Matches (format);
            if (placeholders.Count > 0)
            {
                var maximumFormatIndex = 0;
                for (var i = 0; i < placeholders.Count; i++)
                {
                    var index = Convert.ToInt32 (placeholders[i].Groups[1].Value, 10);
                    maximumFormatIndex = Math.Max (maximumFormatIndex, index);
                }
                // replace a substring like "{0}" with "(?<c0>.+)"
                var pattern = placeholderPattern.Replace (format, @"(?<c${index}>.+)");
                var matches = Regex.Match (formatted, pattern, StandardOptions);
                if (matches.Success)
                {
                    for (var i = 0; i <= maximumFormatIndex; i++)
                    {
                        var groupName = "c{0}".FormatInvariant (i);
                        result.Add (matches.Groups[groupName].Value);
                    }
                }
            }
            // TODO: should probably wrap result in a ReadOnlyCollection
            return result;
        }
    }
}
