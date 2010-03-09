using System;
using System.Collections.Generic;

namespace SoftwareNinjas.Core.Text
{
    /// <summary>
    /// Represents a strategy for searching items of type <typeparamref name="T"/> by their <see cref="String"/>
    /// representations.
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type of <see cref="Items"/> whose <see cref="String"/> representation will be matched on for searching.
    /// </typeparam>
    public interface IMatcher<T>
    {
        /// <summary>
        /// The type of <see cref="String"/> comparison to use.
        /// </summary>
        StringComparison ComparisonType { get; set; }

        /// <summary>
        /// The items to search.
        /// </summary>
        IList<T> Items { get; }

        /// <summary>
        /// Returns a sequence of <typeparamref name="T"/> items whose <see cref="String"/> representations were matched
        /// by the provided <paramref name="characters"/> anywhere said representation.
        /// </summary>
        /// 
        /// <param name="characters">
        /// One or more characters to search for in the <see cref="String"/> representation of the <see cref="Items"/>.
        /// </param>
        /// 
        /// <returns>
        /// A sequence of the <see cref="Items"/> whose <see cref="String"/> representations matched
        /// <paramref name="characters"/> anywhere.
        /// </returns>
        IEnumerable<T> MatchAnywhere(string characters);

        /// <summary>
        /// Returns a sequence of <typeparamref name="T"/> items whose <see cref="String"/> representations were matched
        /// by the provided <paramref name="characters"/> at the beginning of said representation, then those items
        /// whose <see cref="String"/> representations were matched anywhere.
        /// </summary>
        /// 
        /// <param name="characters">
        /// One or more characters to search for in the <see cref="String"/> representation of the <see cref="Items"/>.
        /// </param>
        /// 
        /// <returns>
        /// A sequence of the <see cref="Items"/> whose <see cref="String"/> representations matched
        /// <paramref name="characters"/> at the start or anywhere.
        /// </returns>
        IEnumerable<T> MatchFromStartThenAnywhere(string characters);

        /// <summary>
        /// Returns a sequence of <typeparamref name="T"/> items whose <see cref="String"/> representations were matched
        /// by the provided <paramref name="characters"/> at the beginning of said representation.
        /// </summary>
        /// 
        /// <param name="characters">
        /// One or more characters to search for in the <see cref="String"/> representation of the <see cref="Items"/>.
        /// </param>
        /// 
        /// <returns>
        /// A sequence of the <see cref="Items"/> whose <see cref="String"/> representations matched
        /// <paramref name="characters"/> at the start.
        /// </returns>
        IEnumerable<T> MatchFromStart(string characters);

        /// <summary>
        /// Returns a sequence of <typeparamref name="T"/> items whose <see cref="String"/> representations were matched
        /// by the provided <paramref name="characters"/> such that uppercase letters are matched with uppercase
        /// letters in said representations and optionally disambiguated by lowercase letters.
        /// </summary>
        /// 
        /// <param name="characters">
        /// One or more characters to search for in the <see cref="String"/> representation of the <see cref="Items"/>.
        /// </param>
        /// 
        /// <returns>
        /// A sequence of the <see cref="Items"/> whose <see cref="String"/> representations matched
        /// <paramref name="characters"/> as camel-cased humps.
        /// </returns>
        IEnumerable<T> MatchCamelCasingHumps(string characters);
    }
}