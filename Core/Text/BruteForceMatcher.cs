using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SoftwareNinjas.Core.Text
{
    /// <summary>
    /// A naïve implementation of <see cref="IMatcher{T}"/> that checks everything, every time.
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type of <see cref="IMatcher{T}.Items"/> to search for by their <see cref="String"/> representation.
    /// </typeparam>
    public class BruteForceMatcher<T> : IMatcher<T>
    {
        // TODO: we could also keep a list of the string representations of the _items,
        // to not have to call ToString() on every search.  The two lists would be correlated.
        private readonly IList<T> _items;

        /// <summary>
        /// Initializes a new instance of the <see cref="BruteForceMatcher{T}"/> with an empty list of
        /// <see cref="IMatcher{T}.Items"/>.
        /// </summary>
        public BruteForceMatcher()
        {
            _items = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BruteForceMatcher{T}"/> with the provided
        /// <paramref name="items"/>.
        /// </summary>
        /// 
        /// <param name="items">
        /// A sequence of instances of <typeparamref name="T"/> that will be searched for by their string
        /// representation.
        /// </param>
        public BruteForceMatcher(IEnumerable<T> items)
        {
            _items = new List<T>(items);
        }

        /// <summary>
        /// The type of <see cref="String"/> comparison to use.
        /// </summary>
        public StringComparison ComparisonType { get; set; }

        IList<T> IMatcher<T>.Items
        {
            get
            {
                return _items;
            }
        }

        IEnumerable<T> IMatcher<T>.MatchAnywhere(string characters)
        {
            foreach (var item in _items)
            {
                if (item.ToString().Contains(characters, ComparisonType))
                {
                    yield return item;
                }
            }
        }

        IEnumerable<T> IMatcher<T>.MatchFromStartThenAnywhere(string characters)
        {
            var matchedAnywhereItems = new List<T>();
            foreach (var item in _items)
            {
                var s = item.ToString();
                if (s.StartsWith(characters, ComparisonType))
                {
                    yield return item;
                }
                else if (s.Contains(characters, ComparisonType))
                {
                    matchedAnywhereItems.Add(item);
                }
            }
            foreach (var item in matchedAnywhereItems)
            {
                yield return item;
            }
        }

        IEnumerable<T> IMatcher<T>.MatchFromStart(string characters)
        {
            foreach (var item in _items)
            {
                if (item.ToString().StartsWith(characters, ComparisonType))
                {
                    yield return item;
                }
            }
        }

        IEnumerable<T> IMatcher<T>.MatchCamelCasingHumps(string characters)
        {
            var pattern = ConvertSpecificationToPattern(characters);
            var options = ConvertToRegexOptions(ComparisonType);
            var re = new Regex(pattern, options | RegexOptions.Compiled);
            foreach (var item in _items)
            {
                if (re.IsMatch(item.ToString()))
                {
                    yield return item;
                }
            }
        }

        internal static RegexOptions ConvertToRegexOptions(StringComparison comparisonType)
        {
            RegexOptions result = RegexOptions.None;
            if (comparisonType.HasFlag(StringComparison.InvariantCulture))
            {
                result |= RegexOptions.CultureInvariant;
            }
            if(comparisonType.HasFlag(StringComparison.CurrentCultureIgnoreCase)
                || comparisonType.HasFlag(StringComparison.InvariantCultureIgnoreCase)
                || comparisonType.HasFlag(StringComparison.OrdinalIgnoreCase))
            {
                result |= RegexOptions.IgnoreCase;
            }
            return result;
        }

        internal static string ConvertSpecificationToPattern(string specification)
        {
            var sb = new StringBuilder();
            var isFirst = true;
            foreach (var c in specification)
            {
                if (Char.IsUpper(c))
                {
                    if (isFirst)
                    {
                        isFirst = false;
                    }
                    else
                    {
                        sb.Append(".*");
                    }
                }
                sb.Append(c);
            }
            if (!isFirst)
            {
                sb.Append(".*");
            }
            return sb.ToString();
        }
    }
}
