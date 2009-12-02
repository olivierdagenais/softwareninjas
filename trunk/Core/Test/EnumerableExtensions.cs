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
        public static void EnumerateSame<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            EnumerateSame(expected, actual, (t) => t);
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
        /// 
        /// <param name="comparisonBasis">
        /// A method to use to manipulate the <typeparamref name="T"/> (perhaps extract a value) into what will be
        /// used to check if each <typeparamref name="T"/> is equal.
        /// </param>
        public static void EnumerateSame<T>(IEnumerable<T> expected, IEnumerable<T> actual, 
            Func<T, Object> comparisonBasis )
        {
            var eEnum = expected.GetEnumerator();
            var aEnum = actual.GetEnumerator();
            while (eEnum.MoveNext())
            {
                Assert.IsTrue(aEnum.MoveNext());
                Assert.AreEqual(comparisonBasis(eEnum.Current), comparisonBasis(aEnum.Current));
            }
            Assert.IsFalse(aEnum.MoveNext());
        }

        /// <summary>
        /// Tests the <see cref="Parent.EnumerableExtensions.FirstOrDefault{T}(IEnumerable{T})" /> method with
        /// empty sequences.
        /// </summary>
        [Test]
        public void FirstOrDefault_EmptySequence()
        {
            Assert.AreEqual(0, Parent.EnumerableExtensions.FirstOrDefault(new int[] { }));
            Assert.AreEqual(null, Parent.EnumerableExtensions.FirstOrDefault(new string[] { }));
        }

        /// <summary>
        /// Tests the <see cref="Parent.EnumerableExtensions.FirstOrDefault{T}(IEnumerable{T})" /> method with
        /// sequences that contain one item.
        /// </summary>
        [Test]
        public void FirstOrDefault_SequenceWithOneItem()
        {
            Assert.AreEqual(42, Parent.EnumerableExtensions.FirstOrDefault(new int[] { 42 }));
            Assert.AreEqual("chicken", Parent.EnumerableExtensions.FirstOrDefault(new string[] { "chicken" }));
        }

        /// <summary>
        /// Tests <see cref="Parent.EnumerableExtensions.ForElse{T}(IEnumerable{T},Action{T},Action)"/> with
        /// the case there the <see cref="IEnumerable{T}"/> yields only one item.
        /// </summary>
        [Test]
        public void ForElse_HasOneItem()
        {
            var dest = new List<string>();
            var input = new[] { "1" };
            input.ForElse(
                i => dest.Add(i),
                () => Assert.Fail("The 'else' should not be called")
            );
            EnumerateSame(input, dest);
        }

        /// <summary>
        /// Tests <see cref="Parent.EnumerableExtensions.ForElse{T}(IEnumerable{T},Action{T},Action)"/> with
        /// the typical case.
        /// </summary>
        [Test]
        public void ForElse_HasItems()
        {
            var dest = new List<string>();
            var input = new[] { "1", "2", "3" };
            input.ForElse(
                i => dest.Add(i),
                () => Assert.Fail("The 'else' should not be called")
            );
            EnumerateSame(input, dest);
        }

        /// <summary>
        /// Tests <see cref="Parent.EnumerableExtensions.ForElse{T}(IEnumerable{T},Func{T,bool},Action)"/> with
        /// the delegate returning <see langword="false"/>, which should end the enumeration right then and there.
        /// </summary>
        [Test]
        public void ForElse_StopAfterOne()
        {
            var dest = new List<string>();
            var input = new[] { "1", "2", "3" };
            input.ForElse(
                i =>
                {
                    dest.Add(i);
                    return false;
                },
                () => Assert.Fail("The 'else' should not be called")
            );
            Assert.AreEqual(1, dest.Count);
            Assert.AreEqual(input[0], dest[0]);
        }

        /// <summary>
        /// Tests <see cref="Parent.EnumerableExtensions.ForElse{T}(IEnumerable{T},Action{T},Action)"/> with
        /// the exceptional case where the <see cref="IEnumerable{T}"/> did not yield any items.
        /// </summary>
        [Test]
        public void ForElse_HasNoItems()
        {
            var input = new string[] { };
            var elseCalled = 0;
            input.ForElse(
                i => Assert.Fail("The 'each' should not be called"),
                () => elseCalled++
            );
            Assert.AreEqual(1, elseCalled);
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
