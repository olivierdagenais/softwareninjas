using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public static IEnumerable<T> Compose<T>(this T firstItem, IEnumerable<T> items)
        {
            yield return firstItem;
            if (items != null)
            {
                foreach (T item in items)
                {
                    yield return item;
                }
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
        public static IEnumerable<T> Compose<T>(this IEnumerable<T> items, T lastItem)
        {
            if (items != null)
            {
                foreach (T item in items)
                {
                    yield return item;
                }
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
        public static IEnumerable<T> Compose<T>(this IEnumerable<T> firstItems, IEnumerable<T> lastItems)
        {
            if (firstItems != null)
            {
                foreach (T item in firstItems)
                {
                    yield return item;
                }
            }
            if (lastItems != null)
            {
                foreach (T item in lastItems)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Filters a set of <paramref name="items"/> of type <typeparamref name="T"/> based on the provided
        /// <paramref name="predicate"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of elements to enumerate.
        /// </typeparam>
        /// 
        /// <param name="items">
        /// The items to filter.
        /// </param>
        /// 
        /// <param name="predicate">
        /// A function that determines whether to keep each item (<see langword="true"/>)
        /// or not (<see langword="false"/>).
        /// </param>
        /// 
        /// <returns>
        /// The <paramref name="items"/> for which <paramref name="predicate"/> returned <see langword="true"/>.
        /// </returns>
        public static IEnumerable<T> Filter<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            foreach (T item in items)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Returns the first element of a sequence, or a default value if the sequence contains no elements.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// 
        /// <param name="source">
        /// The <see cref="IEnumerable{T}"/> to return the first element of.
        /// </param>
        /// 
        /// <returns>
        /// <b>default</b>(<typeparamref name="T"/>) if <paramref name="source"/> is empty; otherwise, the first element
        /// in <paramref name="source"/>.
        /// </returns>
        public static T FirstOrDefault<T>(this IEnumerable<T> source)
        {
            var e = source.GetEnumerator();
            if (e.MoveNext())
            {
                return e.Current;
            }
            return default(T);
        }

        /// <summary>
        /// Applies <paramref name="each"/> to items of a sequence or calls <paramref name="else"/> if the sequence
        /// was empty.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// 
        /// <param name="source">
        /// An <see cref="IEnumerable{T}"/> that contains the elements to enumerate.
        /// </param>
        /// 
        /// <param name="each">
        /// A function that will be called for each of the items in the sequence.  Return <see langword="false"/> to
        /// stop the enumeration.
        /// </param>
        /// 
        /// <param name="else">
        /// An action that will be called if the sequence did not yield any items.
        /// </param>
        public static void ForElse<T> ( this IEnumerable<T> source, Func<T, bool> each, Action @else )
        {
            var e = source.GetEnumerator();
            if (e.MoveNext())
            {
                if (!each(e.Current))
                {
                    return;
                }
                while (e.MoveNext())
                {
                    if (!each(e.Current))
                    {
                        return;
                    }
                }
            }
            else
            {
                @else();
            }
        }

        /// <summary>
        /// Applies <paramref name="each"/> to items of a sequence or calls <paramref name="else"/> if the sequence
        /// was empty.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of the elements of <paramref name="source"/>.
        /// </typeparam>
        /// 
        /// <param name="source">
        /// An <see cref="IEnumerable{T}"/> that contains the elements to enumerate.
        /// </param>
        /// 
        /// <param name="each">
        /// An action that will be called for each of the items in the sequence.
        /// </param>
        /// 
        /// <param name="else">
        /// An action that will be called if the sequence did not yield any items.
        /// </param>
        public static void ForElse<T>(this IEnumerable<T> source, Action<T> each, Action @else)
        {
            ForElse ( source, item => { each(item); return true; }, @else );
        }

        /// <summary>
        /// Combines two sequences such that the <paramref name="insertedItems"/> will appear before the
        /// <paramref name="items"/> at the provided <paramref name="insertionIndex"/>, followed by the rest of the
        /// <paramref name="items"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of <paramref name="items"/> and <paramref name="insertedItems"/>.
        /// </typeparam>
        /// 
        /// <param name="items">
        /// The sequence in which to insert <paramref name="insertedItems"/>.
        /// </param>
        /// 
        /// <param name="insertedItems">
        /// The sequence to insert into <paramref name="items"/>.
        /// </param>
        /// 
        /// <param name="insertionIndex">
        /// The index of the item in <paramref name="items"/> before which <paramref name="insertedItems"/> will be
        /// inserted.
        /// </param>
        /// 
        /// <returns>
        /// A sequence representing the insertion of <paramref name="insertedItems"/> into <paramref name="items"/>
        /// at the specified <paramref name="insertionIndex"/>.
        /// </returns>
        public static IEnumerable<T> Insert<T>(this IEnumerable<T> items, IEnumerable<T> insertedItems,
            int insertionIndex)
        {
            var index = 0;
            foreach (var item in items)
            {
                if (index == insertionIndex)
                {
                    foreach (var insertedItem in insertedItems)
                    {
                        yield return insertedItem;
                    }
                }
                yield return item;
                index++;
            }
            if (index == insertionIndex)
            {
                foreach (var insertedItem in insertedItems)
                {
                    yield return insertedItem;
                }
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
            // TODO: Join() is really a special case of Map() and Reduce(); consider extracting Reduce() and re-writing
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
        /// Applies a transformation on every item of type <typeparamref name="T"/> in <paramref name="items"/> to
        /// yield as many items of type <typeparamref name="TResult"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of items to convert from.
        /// </typeparam>
        /// 
        /// <typeparam name="TResult">
        /// The type of items to convert to.
        /// </typeparam>
        /// 
        /// <param name="items">
        /// One or more <typeparamref name="T"/> instances to convert.
        /// </param>
        /// 
        /// <param name="transformer">
        /// A method that accepts an instance of <typeparamref name="T"/> and produces an instance of
        /// <typeparamref name="TResult"/>.
        /// </param>
        /// 
        /// <returns>
        /// A number of <typeparamref name="TResult"/> instances that were created from <typeparamref name="T"/>
        /// instances.
        /// </returns>
        public static IEnumerable<TResult> Map<T, TResult>(this IEnumerable<T> items, Func<T, TResult> transformer)
        {
            foreach (T item in items)
            {
                yield return transformer(item);
            }
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
                return s;
            };
            return Join(values, " ", stringifier);
        }

        /// <summary>
        /// Converts a sequence of <typeparamref name="T"/> to a <see cref="IList{T}"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of elements in the <paramref name="sequence"/>.
        /// </typeparam>
        /// 
        /// <param name="sequence">
        /// The sequence to convert to a <see cref="IList{T}"/>.
        /// </param>
        /// 
        /// <returns>
        /// A read-only <see cref="IList{T}"/> representing the elements in the <paramref name="sequence"/>.
        /// </returns>
        public static IList<T> ToList<T>(this IEnumerable<T> sequence)
        {
            var list = new List<T>(sequence);
            var readOnlyCollection = new ReadOnlyCollection<T>(list);
            return readOnlyCollection;
        }
    }
}
