using System;
using System.Collections.Generic;

namespace System.Linq
{
	/// <summary>
	/// Re-implements parts of System.Core.dll for use with .NET 2.0
	/// </summary>
	public static class Enumerable
	{
		/// <summary>
		/// Returns the last element of a sequence.
		/// </summary>
		/// 
		/// <typeparam name="TSource">
		/// The type of the elements of <paramref name="source"/>.
		/// </typeparam>
		/// 
		/// <param name="source">
		/// An <see cref="IEnumerable{T}"/> to return the last element of.
		/// </param>
		/// 
		/// <returns>
		/// The value at the last position in the source sequence.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="source"/> is <see langword="null"/>.
		/// </exception>
		/// 
		/// <exception cref="InvalidOperationException">
		/// The source sequence is empty.
		/// </exception>
		public static TSource Last<TSource>(this IEnumerable<TSource> source)
		{
			if (null == source)
			{
				throw new ArgumentNullException("source");
			}
			var e = source.GetEnumerator();
			if(!e.MoveNext())
			{
				throw new InvalidOperationException();
			}
			TSource item = e.Current;
			while (e.MoveNext())
			{
				item = e.Current;
			}
			return item;
		}

		/// <summary>
		/// Determines whether a sequence contains any elements.
		/// </summary>
		/// 
		/// <typeparam name="TSource">
		/// The type of the elements of <paramref name="source"/>.
		/// </typeparam>
		/// 
		/// <param name="source">
		/// An <see cref="IEnumerable{T}"/> to check for emptiness.
		/// </param>
		/// 
		/// <returns>
		/// <see langword="true"/> if the source sequence contains any elements; otherwise <see langword="false"/>.
		/// </returns>
		/// 
		/// <exception cref="ArgumentNullException">
		/// <paramref name="source"/> is <see langword="null"/>.
		/// </exception>
		public static bool Any<TSource>(this IEnumerable<TSource> source)
		{
			if (null == source)
			{
				throw new ArgumentNullException("source");
			}
			var e = source.GetEnumerator();
			return e.MoveNext();
		}
	}
}
