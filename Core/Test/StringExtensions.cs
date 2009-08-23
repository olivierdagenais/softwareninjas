using System;
using System.Globalization;

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
    }
}
