using System;
using System.Globalization;
using System.IO;

using Parent = SoftwareNinjas.Core;
using NUnit.Framework;

namespace SoftwareNinjas.Core.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.StringExtensions"/>.
    /// </summary>
    [TestFixture]
    public class StringExtensions
    {
        private const string Str = "No formatting necessary";
        private static readonly CultureInfo FrCa = CultureInfo.CreateSpecificCulture("fr-ca");

        /// <summary>
        /// Tests the <see cref="Parent.StringExtensions.Contains(String,String,StringComparison)" /> method with
        /// a few simple scenarios.
        /// </summary>
        [Test]
        public void Contains_SimpleScenarios()
        {
            Assert.IsTrue(Parent.StringExtensions.Contains(Str, Str, StringComparison.InvariantCulture));
            Assert.IsTrue(Parent.StringExtensions.Contains("one", "o", StringComparison.InvariantCulture));
            Assert.IsTrue(Parent.StringExtensions.Contains("one", "n", StringComparison.InvariantCulture));
            Assert.IsTrue(Parent.StringExtensions.Contains("one", "e", StringComparison.InvariantCulture));
            Assert.IsTrue(Parent.StringExtensions.Contains("OnE", "oNe", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(Parent.StringExtensions.Contains("One", "O", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(Parent.StringExtensions.Contains("One", "n", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(Parent.StringExtensions.Contains("One", "e", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(Parent.StringExtensions.Contains("One", "o", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(Parent.StringExtensions.Contains("One", "N", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(Parent.StringExtensions.Contains("OnE", "e", StringComparison.InvariantCultureIgnoreCase));

            Assert.IsFalse(Parent.StringExtensions.Contains("OnE", "oNe", StringComparison.InvariantCulture));
            Assert.IsFalse(Parent.StringExtensions.Contains("OnE", "o", StringComparison.InvariantCulture));
            Assert.IsFalse(Parent.StringExtensions.Contains("One", "a", StringComparison.InvariantCulture));
            Assert.IsFalse(Parent.StringExtensions.Contains("One", "a", StringComparison.InvariantCultureIgnoreCase));
        }


        /// <summary>
        /// Tests <see cref="Parent.StringExtensions.FormatInvariant(String,Object[])"/>
        /// with no arguments.
        /// </summary>
        [Test]
        public void FormatInvariant_NoArgs()
        {
            Assert.AreEqual(Str, Str.FormatInvariant());
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
            Assert.AreEqual(Str, Str.FormatProvider(FrCa));
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
        /// Tests the <see cref="Parent.StringExtensions.InsertLines(String, String, int)" /> method with
        /// the typical case.
        /// </summary>
        [Test]
        public void Insert_Typical()
        {
            const string source = @"1
2
3
4
5
6";
            const string inserted = @"5.1
5.2
5.3";

            const string expected = @"1
2
3
4
5
5.1
5.2
5.3
6";
            var actual = Parent.StringExtensions.InsertLines(source, inserted, 6);
            Assert.AreEqual(expected, actual);
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
			EnumerableExtensions.EnumerateSame(new[] { Str }, Str.Lines());
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
