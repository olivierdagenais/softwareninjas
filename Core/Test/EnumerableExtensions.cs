using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Parent = SoftwareNinjas.Core;

namespace SoftwareNinjas.Core.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.EnumerableExtensions"/>
    /// </summary>
    [TestFixture]
    public class EnumerableExtensions
    {
        /// <summary>
        /// Tests both <see cref="Parent.EnumerableExtensions.Compose{T}(T,IEnumerable{T})"/>.
        /// </summary>
        [Test]
        public void Compose_Prefix()
        {
            EnumerateSame(new int[] { 1, 2, 3 }, Parent.EnumerableExtensions.Compose(1, new int[] { 2, 3 }));
            EnumerateSame(new string[] { "first" }, Parent.EnumerableExtensions.Compose("first", new string[] { }));
        }

        /// <summary>
        /// Tests <see cref="Parent.EnumerableExtensions.Compose{T}(IEnumerable{T},T)"/>.
        /// </summary>
        [Test]
        public void Compose_Suffix()
        {
            EnumerateSame(new int[] { 1, 2, 3 }, Parent.EnumerableExtensions.Compose(new int[] { 1, 2 }, 3));
            EnumerateSame(new string[] { "last" }, Parent.EnumerableExtensions.Compose(new string[] { }, "last"));
        }

        /// <summary>
        /// Tests <see cref="Parent.EnumerableExtensions.Compose{T}(IEnumerable{T},IEnumerable{T})"/>.
        /// </summary>
        [Test]
        public void Compose_Append()
        {
            EnumerateSame(new int[] { },
                Parent.EnumerableExtensions.Compose(new int[] { }, new int[] { }));
            EnumerateSame(new int[] { 1, 2, 3, 4 },
                Parent.EnumerableExtensions.Compose(new int[] { 1, 2 }, new int[] { 3, 4 }));
            EnumerateSame(new string[] { "first" },
                Parent.EnumerableExtensions.Compose(new string[] { "first" }, new string[] { }));
            EnumerateSame(new string[] { "last" },
                Parent.EnumerableExtensions.Compose(new string[] { }, new string[] { "last" }));
        }

        /// <summary>
        /// Convenience method for making sure two <see cref="IEnumerable{T}"/> instances will enumerate identical
        /// items.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of elements to enumerate.
        /// </typeparam>
        /// 
        /// <param name="expected">
        /// The expected elements.
        /// </param>
        /// 
        /// <param name="actual">
        /// The actual elements.
        /// </param>
        private static void EnumerateSame<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            var eEnum = expected.GetEnumerator();
            var aEnum = actual.GetEnumerator();
            while (eEnum.MoveNext())
            {
                Assert.IsTrue(aEnum.MoveNext());
                Assert.AreEqual(eEnum.Current, aEnum.Current);
            }
            Assert.IsFalse(aEnum.MoveNext());
        }

        /// <summary>
        /// Tests <see cref="Parent.EnumerableExtensions.Join{T}(IEnumerable{T},String)"/>.
        /// </summary>
        [Test]
        public void Join()
        {
            Assert.AreEqual("1,2,3", new string[] { "1", "2", "3" }.Join(","));
            Assert.AreEqual("1, 2, 3", new string[] { "1", "2", "3" }.Join(", "));
            Assert.AreEqual("", new string[] { }.Join(","));
            Assert.AreEqual("1", new string[] { "1" }.Join(","));
            Assert.AreEqual("1,2", new string[] { "1", "2" }.Join(","));
        }

        /// <summary>
        /// Tests <see cref="Parent.EnumerableExtensions.QuoteForShell{T}(IEnumerable{T})"/>
        /// </summary>
        [Test]
        public void QuoteForShell()
        {
            Assert.AreEqual(String.Empty, new string[] { }.QuoteForShell());
            Assert.AreEqual(String.Empty, new string[] { "" }.QuoteForShell());
            Assert.AreEqual("one", new string[] { "one" }.QuoteForShell());
            Assert.AreEqual("\"with space\"", new string[] { "with space" }.QuoteForShell());

            Assert.AreEqual("\"with space\" \"with another\"", 
                new string[] { "with space", "with another" }.QuoteForShell());
            Assert.AreEqual("\"mixed content\" 42",
                new object[] { "mixed content", 42 }.QuoteForShell());

            Assert.AreEqual("\"--summary=$projectName ($reviewer): $summary\" --target-people=$reviewer", 
                new string[] { "--summary=$projectName ($reviewer): $summary", 
                        "--target-people=$reviewer" }.QuoteForShell());
        }
    }
}
