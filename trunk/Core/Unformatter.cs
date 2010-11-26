using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace SoftwareNinjas.Core
{
    /// <summary>
    /// A class to parse format strings and decode formatted versions back into component strings.
    /// </summary>
    public class Unformatter
    {
        private const RegexOptions StandardOptions = RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant;
        private static readonly Regex PlaceholderRegex =
            new Regex (@"{(?<index>\d+)}", StandardOptions | RegexOptions.Compiled);

        private readonly string _format;
        private readonly int _maximumFormatIndex;
        private readonly Regex _unformattingRegex;

        /// <summary>
        /// Initializes a new instance of the <see cref="Unformatter"/> class with the specified
        /// <paramref name="format"/>, indicating whether to compile the resulting <see cref="Regex"/> or not.
        /// </summary>
        /// 
        /// <param name="format">
        /// A composite format string.
        /// </param>
        /// 
        /// <param name="compileRegex">
        /// <see langword="true" /> if the resulting <see cref="Regex"/> should include the option
        /// <see cref="RegexOptions.Compiled"/>; <see langword="false"/> otherwise. There is a slight performance
        /// penalty for compiling a <see cref="Regex"/>, but then subsequent matching is much more performant.
        /// In other words, set to <see langword="true"/> if lots of formatted strings will be unformatted with this
        /// instance.
        /// </param>
        public Unformatter(string format, bool compileRegex)
        {
            _format = format;
            var placeholders = PlaceholderRegex.Matches (format);
            if (placeholders.Count > 0)
            {
                _maximumFormatIndex = 0;
                for (var i = 0; i < placeholders.Count; i++)
                {
                    var index = Convert.ToInt32 (placeholders[i].Groups[1].Value, 10);
                    _maximumFormatIndex = Math.Max (_maximumFormatIndex, index);
                }
                // replace a substring like "{0}" with "(?<c0>.+)"
                var pattern = PlaceholderRegex.Replace (format, @"(?<c${index}>.+)");

                var options = StandardOptions;
                if (compileRegex)
                {
                    options |= RegexOptions.Compiled;
                }
                _unformattingRegex = new Regex (pattern, options);
            }
        }

        /// <summary>
        /// The composite format string represented by this instance of <see cref="Unformatter"/>.
        /// </summary>
        public string Format
        {
            get
            {
                return _format;
            }
        }

        /// <summary>
        /// Performs the reverse of <see cref="StringExtensions.FormatInvariant(String, Object[])"/> by trying to
        /// determine what the string representations of the objects were when they were formatted into
        /// <see cref="Format"/> to yield <paramref name="formatted"/>.
        /// </summary>
        /// 
        /// <param name="formatted">
        /// The result of formatting <see cref="Format"/> with zero or more objects.
        /// </param>
        /// 
        /// <returns>
        /// An <see cref="IList{String}"/> representing the string representations of the objects formatted into
        /// <see cref="Format"/>.
        /// </returns>
        public IList<string> Unformat(string formatted)
        {
            var result = new List<string> ();
            var matches = _unformattingRegex.Match (formatted);
            if (matches.Success)
            {
                for (var i = 0; i <= _maximumFormatIndex; i++)
                {
                    var groupName = "c{0}".FormatInvariant (i);
                    result.Add (matches.Groups[groupName].Value);
                }
            }
            return new ReadOnlyCollection<string> (result);
        }

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
            var unformatter = new Unformatter (format, false);
            return unformatter.Unformat (formatted);
        }
    }
}
