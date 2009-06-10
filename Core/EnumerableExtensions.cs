using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareNinjas.Core
{
    /// <summary>
    /// Extension methods to augment implementations of <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Simulates extending an <see cref="IEnumerable{T}"/> with an extra item at the beginning.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of elements to enumerate.
        /// </typeparam>
        /// 
        /// <param name="firstItem">
        /// The item to enumerate first.
        /// </param>
        /// 
        /// <param name="items">
        /// The items to enumerate after <paramref name="firstItem"/>.
        /// </param>
        /// 
        /// <returns>
        /// An enumeration of <paramref name="firstItem"/> and then all the items in <paramref name="items"/>.
        /// </returns>
        public static IEnumerable<T> Compose<T>(T firstItem, IEnumerable<T> items)
        {
            yield return firstItem;
            foreach (T item in items)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Simulates extending an <see cref="IEnumerable{T}"/> with an extra item at the end.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of elements to enumerate.
        /// </typeparam>
        /// 
        /// <param name="items">
        /// The items to enumerate before <paramref name="lastItem"/>.
        /// </param>
        /// 
        /// <param name="lastItem">
        /// The item to enumerate last.
        /// </param>
        /// 
        /// <returns>
        /// An enumeration of all the items in <paramref name="items"/> and then <paramref name="lastItem"/>.
        /// </returns>
        public static IEnumerable<T> Compose<T>(IEnumerable<T> items, T lastItem)
        {
            foreach (T item in items)
            {
                yield return item;
            }
            yield return lastItem;
        }

        /// <summary>
        /// Simulates extending an <see cref="IEnumerable{T}"/> with another so that a single loop may be used over two
        /// sources of <typeparamref name="T"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of elements to enumerate.
        /// </typeparam>
        /// 
        /// <param name="firstItems">
        /// The items to enumerate first.
        /// </param>
        /// 
        /// <param name="lastItems">
        /// The items to enumerate last.
        /// </param>
        /// 
        /// <returns>
        /// An enumeration of all the items in <paramref name="firstItems"/> and then all those in
        /// <paramref name="lastItems"/>.
        /// </returns>
        public static IEnumerable<T> Compose<T>(IEnumerable<T> firstItems, IEnumerable<T> lastItems)
        {
            foreach (T item in firstItems)
            {
                yield return item;
            }
            foreach (T item in lastItems)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Concatenates a specified separator <see cref="String"/> between each element of a specified
        /// <see cref="IEnumerable{T}"/> of <typeparamref name="T"/>, yielding a single concatenated string. 
        /// </summary>
        /// 
        /// <param name="values">
        /// Zero or more <typeparamref name="T"/> items, such as an array of <see cref="String"/>.
        /// </param>
        /// 
        /// <param name="separator">
        /// A <see cref="String"/> to insert in between all the <see cref="String"/> representations of the instances in
        /// <paramref name="values"/>.
        /// </param>
        /// 
        /// <typeparam name="T">
        /// The type of items in <paramref name="values"/>.
        /// </typeparam>
        /// 
        /// <returns>
        /// A <see cref="String"/> consisting of the elements of <paramref name="values"/> interspersed with the
        /// <paramref name="separator"/> string.
        /// </returns>
        /// 
        /// <seealso cref="String.Join(String, String[])"/>
        public static string Join<T>(this IEnumerable<T> values, string separator)
        {
            return Join(values, separator, o => o.ToString());
        }

        /// <summary>
        /// Concatenates a specified separator <see cref="String"/> between each element of a specified
        /// <see cref="IEnumerable{T}"/> of <typeparamref name="T"/> - which are transformed to <see cref="String"/>
        /// instances by the specified <paramref name="stringifier"/>, yielding a single concatenated string. 
        /// </summary>
        /// 
        /// <param name="values">
        /// Zero or more <typeparamref name="T"/> items, such as an array of <see cref="String"/>.
        /// </param>
        /// 
        /// <param name="separator">
        /// A <see cref="String"/> to insert in between all the <see cref="String"/> representations of the instances in
        /// <paramref name="values"/>.
        /// </param>
        /// 
        /// <param name="stringifier">
        /// The functor to use to convert <typeparamref name="T"/> instances to <see cref="String"/> instances.  Useful
        /// for using an <see cref="IFormatProvider"/> or for applying extra processing to the strings before they are
        /// joined.
        /// </param>
        /// 
        /// <typeparam name="T">
        /// The type of items in <paramref name="values"/>.
        /// </typeparam>
        /// 
        /// <returns>
        /// A <see cref="String"/> consisting of the elements of <paramref name="values"/> converted to strings by
        /// <paramref name="stringifier"/> and interspersed with the <paramref name="separator"/> string.
        /// </returns>
        /// 
        /// <seealso cref="String.Join(String, String[])"/>
        public static string Join<T>(this IEnumerable<T> values, string separator, Func<T, string> stringifier)
        {
            StringBuilder sb = new StringBuilder();
            var e = values.GetEnumerator();
            if (e.MoveNext())
            {
                sb.Append(stringifier(e.Current));
                while (e.MoveNext())
                {
                    sb.Append(separator);
                    sb.Append(stringifier(e.Current));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Prepares a single <see cref="String"/> representing all the <paramref name="values"/> quoted as necessary
        /// for use when invoking a sub-process.
        /// </summary>
        /// 
        /// <param name="values">
        /// Zero or more values to assemble into a single string.
        /// </param>
        /// 
        /// <typeparam name="T">
        /// The type of items in <paramref name="values"/>.
        /// </typeparam>
        /// 
        /// <returns>
        /// All the <paramref name="values"/> converted to <see cref="String"/>, quoted if they contained a space and
        /// separated by spaces.
        /// </returns>
        public static string QuoteForShell<T>(this IEnumerable<T> values)
        {
            // TODO: *nix shells might use single quotes or different rules for quoting?
            Func<T, string> stringifier = o =>
            {
                string s = o.ToString();
                if (s.Contains(" "))
                {
                    return '"' + s + '"';
                }
                else
                {
                    return s;
                }
            };
            return Join(values, " ", stringifier);
        }
    }
}
