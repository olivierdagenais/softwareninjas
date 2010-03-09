using System;
using System.Collections.Generic;

using NUnit.Framework;
using Parent = SoftwareNinjas.Core.Text;
using System.Text.RegularExpressions;

namespace SoftwareNinjas.Core.Text.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.BruteForceMatcher{T}"/>.
    /// </summary>
    [TestFixture]
    public class BruteForceMatcher : AbstractMatcher
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BruteForceMatcher{T}"/> with the provided
        /// <paramref name="items"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of <see cref="IMatcher{T}.Items"/> to search for by their <see cref="String"/> representation.
        /// </typeparam>
        /// 
        /// <param name="items">
        /// A sequence of instances of <typeparamref name="T"/> that will be searched for by their string
        /// representation.
        /// </param>
        /// 
        /// <returns>
        /// A new instance on each call.
        /// </returns>
        protected override IMatcher<T> CreateImplementation<T>(IEnumerable<T> items)
        {
            return new Parent.BruteForceMatcher<T>(items);
        }

        /// <summary>
        /// Tests the <see cref="Parent.BruteForceMatcher{T}.ConvertToRegexOptions(StringComparison)" /> method with
        /// all possible values.
        /// </summary>
        [Test]
        public void ConvertToRegexOptions_AllPossibilities()
        {
            var test = new Action<RegexOptions, StringComparison>(
                (expected, input)
                => Assert.AreEqual(expected, Parent.BruteForceMatcher<object>.ConvertToRegexOptions(input))
            );

            test(RegexOptions.None, StringComparison.CurrentCulture);
            test(RegexOptions.IgnoreCase, StringComparison.CurrentCultureIgnoreCase);
            test(RegexOptions.CultureInvariant, StringComparison.InvariantCulture);
            test(RegexOptions.CultureInvariant | RegexOptions.IgnoreCase, StringComparison.InvariantCultureIgnoreCase);
            test(RegexOptions.None, StringComparison.Ordinal);
            test(RegexOptions.IgnoreCase, StringComparison.OrdinalIgnoreCase);
        }


        /// <summary>
        /// Tests the <see cref="Parent.BruteForceMatcher{T}.ConvertSpecificationToPattern(String)" /> method with
        /// the typical cases.
        /// </summary>
        [Test]
        public void ConvertSpecificationToPattern_Typical()
        {
            var test = new Action<string, string>(
                (expected, input) 
                => Assert.AreEqual(expected, Parent.BruteForceMatcher<object>.ConvertSpecificationToPattern(input))
            );

            test("Com.*Ha.*F.*", "ComHaF");
            test("Com.*Ha.*Fa.*", "ComHaFa");
            test("Com.*", "Com");
            test("C.*", "C");
            test("C.*H.*F.*", "CHF");
        }
    }
}
