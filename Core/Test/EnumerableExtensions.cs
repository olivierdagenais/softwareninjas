using System;
using System.Collections.Generic;

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
            EnumerateSame(expected, actual, t => t);
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
            bool moved;
            while (eEnum.MoveNext())
            {
                var expectedItem = comparisonBasis(eEnum.Current);
                moved = aEnum.MoveNext();
                if (!moved)
                {
                    Assert.Fail("Insufficient items in 'actual' sequence.  Next expected item: {0}", expectedItem);
                }
                Assert.AreEqual(expectedItem, comparisonBasis(aEnum.Current));
            }
            moved = aEnum.MoveNext();
            if (moved)
            {
                var actualItem = comparisonBasis(aEnum.Current);
                Assert.Fail("Too many items in 'actual' sequence.  Next actual item: {0}", actualItem);
            }
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
        /// Tests the <see cref="Parent.EnumerableExtensions.Insert{T}(IEnumerable{T},IEnumerable{T},int)" /> method
        /// with the typical case.
        /// </summary>
        [Test]
        public void Insert_Typical()
        {
            var items = new[] { 0.0, 1.0, 2.0, 3.0, 4.0, 5.0 };
            var insertedItems = new[] { 4.1, 4.2, 4.3 };
            var expected = new[] { 0.0, 1.0, 2.0, 3.0, 4.0, 4.1, 4.2, 4.3, 5.0 };

            var actual = Parent.EnumerableExtensions.Insert(items, insertedItems, 5);

            EnumerateSame(expected, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.EnumerableExtensions.Insert{T}(IEnumerable{T},IEnumerable{T},int)" /> method
        /// with the typical case, this time using multi-line strings.
        /// </summary>
        [Test]
        public void Insert_TypicalWithMultiLineStrings()
        {
            var items = @"0
1
2
3
4
5";
            var insertedItems = @"4.1
4.2
4.3";

            var expected = @"0
1
2
3
4
4.1
4.2
4.3
5";

            var actual = Parent.EnumerableExtensions.Insert(items.Lines(), insertedItems.Lines(), 5);

            EnumerateSame(expected.Lines(), actual);
        }
        /// <summary>
        /// Tests the <see cref="Parent.EnumerableExtensions.Insert{T}(IEnumerable{T},IEnumerable{T},int)" /> method
        /// with the edge case of the inserted items going before all the items, like calling
        /// <see cref="Parent.EnumerableExtensions.Compose{T}(IEnumerable{T},IEnumerable{T})"/>.
        /// </summary>
        [Test]
        public void Insert_AtTheBeginning()
        {
            var items = new[] { 0.0, 1.0, 2.0, 3.0, 4.0, 5.0 };
            var insertedItems = new[] { -1.1, -1.2, -1.3 };
            var expected = new[] { -1.1, -1.2, -1.3, 0.0, 1.0, 2.0, 3.0, 4.0, 5.0 };

            var actual = Parent.EnumerableExtensions.Insert(items, insertedItems, 0);

            EnumerateSame(expected, actual);
        }

        /// <summary>
        /// Tests the <see cref="Parent.EnumerableExtensions.Insert{T}(IEnumerable{T},IEnumerable{T},int)" /> method
        /// with the edge case of the inserted items going after all the items, like calling
        /// <see cref="Parent.EnumerableExtensions.Compose{T}(IEnumerable{T},IEnumerable{T})"/>.
        /// </summary>
        [Test]
        public void Insert_AtTheEnd()
        {
            var items = new[] { 0.0, 1.0, 2.0, 3.0, 4.0, 5.0 };
            var insertedItems = new[] { 5.1, 5.2, 5.3 };
            var expected = new[] { 0.0, 1.0, 2.0, 3.0, 4.0, 5.0, 5.1, 5.2, 5.3 };

            var actual = Parent.EnumerableExtensions.Insert(items, insertedItems, 6);

            EnumerateSame(expected, actual);
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

        /// <summary>
        /// Tests <see cref="Parent.EnumerableExtensions.ToList{T}(IEnumerable{T})"/>
        /// with an empty sequence.
        /// </summary>
        [Test]
        public void ToList_EmptyEnumerableOfInt()
        {
            var actual = Parent.EnumerableExtensions.ToList(new int[] { });
            Assert.AreEqual(0, actual.Count);
        }

        /// <summary>
        /// Tests <see cref="Parent.EnumerableExtensions.ToList{T}(IEnumerable{T})"/>
        /// to make sure we can't add to the list.
        /// </summary>
        [Test, ExpectedException(typeof(NotSupportedException))]
        public void ToList_ListIsReadOnly()
        {
            var actual = Parent.EnumerableExtensions.ToList(new int[] { });
            actual.Add(42);
        }

        private static IEnumerable<T> Generate<T>(params T[] strings)
        {
            foreach (var s in strings)
            {
                yield return s;
            }
        }

        /// <summary>
        /// Tests <see cref="Parent.EnumerableExtensions.ToList{T}(IEnumerable{T})"/>
        /// with a generator/iterator (i.e. a method that yields a sequence of items).
        /// </summary>
        [Test]
        public void ToList_StringGenerator()
        {
            var generator = Generate("zero", "one", "two");
            var actual = Parent.EnumerableExtensions.ToList(generator);
            Assert.AreEqual(3, actual.Count);
            Assert.AreEqual("zero", actual[0]);
            Assert.AreEqual("one", actual[1]);
            Assert.AreEqual("two", actual[2]);
        }
    }
}
