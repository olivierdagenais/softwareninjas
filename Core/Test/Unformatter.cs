using System;

using Parent = SoftwareNinjas.Core;
using NUnit.Framework;

namespace SoftwareNinjas.Core.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.Unformatter"/>.
    /// </summary>
    [TestFixture]
    public class Unformatter
    {
        /// <summary>
        /// Tests the <see cref="Parent.Unformatter.UnformatInvariant(String, String)" /> method with
        /// a typical use case.
        /// </summary>
        [Test]
        public void UnformatInvariant_Typical ()
        {
            var actual = Parent.Unformatter.UnformatInvariant ("'red' alert", "'{0}' alert");
            EnumerableExtensions.EnumerateSame (new[] {"red"}, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.Unformatter.UnformatInvariant(String, String)" /> method with
        /// a format string where the format placeholders are not in order.
        /// </summary>
        [Test]
        public void UnformatInvariant_ReversedSpecifiers ()
        {
            var actual = Parent.Unformatter.UnformatInvariant ("one zero", "{1} {0}");
            EnumerableExtensions.EnumerateSame (new[] {"zero", "one"}, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.Unformatter.UnformatInvariant(String, String)" /> method with
        /// a format string where one of the format placeholders was not in the string.
        /// </summary>
        [Test]
        public void UnformatInvariant_UnusedFormatPlaceholders ()
        {
            var actual = Parent.Unformatter.UnformatInvariant ("zero one two four", "{0} {1} {2} {4}");
            EnumerableExtensions.EnumerateSame (new[] { "zero", "one", "two", String.Empty, "four" }, actual);
        }
    }
}
