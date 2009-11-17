using System;
using System.Globalization;

using Parent = SoftwareNinjas.Core;
using NUnit.Framework;
using System.IO;

namespace SoftwareNinjas.Core.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.StringExtensions"/>.
    /// </summary>
    [TestFixture]
    public class StringExtensions
    {
        private const string str = "No formatting necessary";
        private readonly CultureInfo FrCa = CultureInfo.CreateSpecificCulture("fr-ca");

        /// <summary>
        /// Tests <see cref="Parent.StringExtensions.FormatInvariant(String,Object[])"/>
        /// with no arguments.
        /// </summary>
        [Test]
        public void FormatInvariant_NoArgs()
        {
            Assert.AreEqual(str, str.FormatInvariant());
        }

        /// <summary>
        /// Tests <see cref="Parent.StringExtensions.FormatInvariant(String,Object[])"/>
        /// with one argument.
        /// </summary>
        [Test]
        public void FormatInvariant_OneArg()
        {
            Assert.AreEqual("One argument was replaced",
                "{0} argument was replaced".FormatInvariant("One"));
        }

        /// <summary>
        /// Tests <see cref="Parent.StringExtensions.FormatInvariant(String,Object[])"/>
        /// with many arguments.
        /// </summary>
        [Test]
        public void FormatInvariant_ManyArgs()
        {
            Assert.AreEqual("Many arguments were replaced, Many times",
                "{0} arguments were replaced, {0} {1}".FormatInvariant("Many", "times"));
        }

        /// <summary>
        /// Tests <see cref="Parent.StringExtensions.FormatProvider(String,IFormatProvider,Object[])"/>
        /// with no arguments.
        /// </summary>
        [Test]
        public void FormatProvider_NoArgs()
        {
            Assert.AreEqual(str, str.FormatProvider(FrCa));
        }

        /// <summary>
        /// Tests <see cref="Parent.StringExtensions.FormatProvider(String,IFormatProvider,Object[])"/>
        /// with one argument.
        /// </summary>
        [Test]
        public void FormatProvider_OneArg()
        {
            Assert.AreEqual("3,5 argument was replaced",
                "{0} argument was replaced".FormatProvider(FrCa, 3.5));
        }

		/// <summary>
		/// Tests <see cref="Core.StringExtensions.Lines(String)"/>
		/// with an empty string.
		/// </summary>
		[Test]
		public void Lines_Empty()
		{
			EnumerableExtensions.EnumerateSame(new string[] { }, String.Empty.Lines());
		}

		/// <summary>
		/// Tests <see cref="Core.StringExtensions.Lines(String)"/>
		/// with a string that's only one line.
		/// </summary>
		[Test]
		public void Lines_OneLine()
		{
			EnumerableExtensions.EnumerateSame(new[] { str }, str.Lines());
		}

		/// <summary>
		/// Tests <see cref="Core.StringExtensions.Lines(String)"/>
		/// with a multi-line string.
		/// </summary>
		[Test]
		public void Lines_Typical()
		{
			var expected = new [] { "one", "two", "three" };
			EnumerableExtensions.EnumerateSame(expected, expected.Join("\r\n").Lines());
			EnumerableExtensions.EnumerateSame(expected, expected.Join("\r").Lines());
			EnumerableExtensions.EnumerateSame(expected, expected.Join("\n").Lines());
		}

        /// <summary>
        /// Tests <see cref="Parent.StringExtensions.CombinePath(String,String[])"/>
        /// with zero additional path fragments.
        /// </summary>
        [Test]
        public void CombinePath_ZeroArguments ()
        {
            var actual = Parent.StringExtensions.CombinePath ("one");
            Assert.AreEqual ("one", actual);
        }

        /// <summary>
        /// Tests <see cref="Parent.StringExtensions.CombinePath(String,String[])"/>
        /// with a single path fragment to add.
        /// </summary>
        [Test]
        public void CombinePath_OneArgument ()
        {
            var expected = Path.Combine ("one", "two");
            var actual = Parent.StringExtensions.CombinePath ("one", "two");
            Assert.AreEqual (expected, actual);
        }

        /// <summary>
        /// Tests <see cref="Parent.StringExtensions.CombinePath(String,String[])"/>
        /// by adding two path fragments.
        /// </summary>
        [Test]
        public void CombinePath_TwoArguments ()
        {
            var expected = Path.Combine (Path.Combine ("one", "two"), "three");
            var actual = Parent.StringExtensions.CombinePath ("one", "two", "three");
            Assert.AreEqual (expected, actual);
        }
    }
}
