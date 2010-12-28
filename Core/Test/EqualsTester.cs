using System;
using NUnit.Framework;

namespace SoftwareNinjas.Core.Test
{
    /// <summary>
    /// A class to help test implementations of <see cref="Object.Equals(Object)"/>
    /// and <see cref="Object.GetHashCode()"/> for the type <typeparamref name="T"/>.
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// A type that overrides  <see cref="Object.Equals(Object)"/> and <see cref="Object.GetHashCode()"/>.
    /// </typeparam>
    /// 
    /// <remarks>
    /// <see cref="EqualsTester{T}"/> can be used as a super-class for a test fixture or invoked manually.
    /// </remarks>
    public class EqualsTester<T>
    {
        private readonly T _x, _y, _z, _a;

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualsTester{T}"/> with the provided samples of
        /// <typeparamref name="T"/> where <paramref name="x"/>, <paramref name="y"/> and <paramref name="z"/> are
        /// expected to be equal to one another and <paramref name="a"/> is expected to be not equal to any of the
        /// others.
        /// </summary>
        /// 
        /// <param name="x">
        /// A sample of <typeparamref name="T"/> equal to <paramref name="y"/> and <paramref name="z"/>.
        /// </param>
        /// 
        /// <param name="y">
        /// A sample of <typeparamref name="T"/> equal to <paramref name="x"/> and <paramref name="z"/>.
        /// </param>
        /// 
        /// <param name="z">
        /// A sample of <typeparamref name="T"/> equal to <paramref name="x"/> and <paramref name="y"/>.
        /// </param>
        /// 
        /// <param name="a">
        /// A sample of <typeparamref name="T"/> equal to none of <paramref name="x"/>, <paramref name="y"/> and
        /// <paramref name="z"/>.
        /// </param>
        public EqualsTester(T x, T y, T z, T a)
        {
            _x = x;
            _y = y;
            _z = z;
            _a = a;
        }

        /// <summary>
        /// Runs all tests.
        /// </summary>
        public void Run()
        {
            AssertAssumptions ();
            AssertReflexivity ();
            AssertSymmetry ();
            AssertTransitivity ();
            AssertNotEqualToNull ();
            AssertConsistentWithHashCode ();
        }

        /// <summary>
        /// Confirms that <c>x</c>, <c>y</c> and <c>z</c> are all equal to each other and that <c>a</c> is different
        /// from all of them.
        /// </summary>
        [Test]
        public void AssertAssumptions()
        {
            Assert.AreEqual (true, _x.Equals (_y));
            Assert.AreEqual (true, _y.Equals (_z));
            Assert.AreEqual (true, _x.Equals (_z));

            Assert.AreEqual (false, _a.Equals (_x));
            Assert.AreEqual (false, _a.Equals (_y));
            Assert.AreEqual (false, _a.Equals (_z));
        }

        /// <summary>
        /// Confirms that <c>x.Equals(x)</c> returns <see langword="true"/> for any <c>x</c>.
        /// </summary>
        /// 
        /// <seealso href="http://en.wikipedia.org/wiki/Reflexive_relation">
        /// Reflexive relation
        /// </seealso>
        [Test]
        public void AssertReflexivity()
        {
            Assert.AreEqual (true, _x.Equals (_x));
            Assert.AreEqual (true, _y.Equals (_y));
            Assert.AreEqual (true, _z.Equals (_z));
            Assert.AreEqual (true, _a.Equals (_a));
        }

        /// <summary>
        /// Confirms that <c>x.Equals(y)</c> returns the same value as <c>y.Equals(x)</c>.
        /// </summary>
        /// 
        /// <seealso href="http://en.wikipedia.org/wiki/Symmetric_relation">
        /// Symmetric relation
        /// </seealso>
        [Test]
        public void AssertSymmetry()
        {
            Assert.AreEqual (_x.Equals (_y), _y.Equals (_x));
            Assert.AreEqual (_x.Equals (_z), _z.Equals (_x));
            Assert.AreEqual (_y.Equals (_z), _z.Equals (_y));

            Assert.AreEqual (_x.Equals (_a), _a.Equals (_x));
            Assert.AreEqual (_y.Equals (_a), _a.Equals (_y));
            Assert.AreEqual (_z.Equals (_a), _a.Equals (_z));
        }

        /// <summary>
        /// Confirms that <c>(x.Equals(y) &amp;&amp; y.Equals(z))</c> returns <see langword="true"/> if and only if
        /// <c>x.Equals(z)</c> returns <see langword="true"/>.
        /// </summary>
        /// 
        /// <seealso href="http://en.wikipedia.org/wiki/Transitive_relation">
        /// Transitive relation
        /// </seealso>
        [Test]
        public void AssertTransitivity()
        {
            if (_x.Equals (_y) && _y.Equals (_z))
            {
                Assert.AreEqual (true, _x.Equals (_z));
            }
        }

        /// <summary>
        /// Confirms that <c>x.Equals(null)</c> returns <see langword="false"/>.
        /// </summary>
        [Test]
        public void AssertNotEqualToNull()
        {
            Assert.AreEqual (false, _x.Equals (null));
            Assert.AreEqual (false, _y.Equals (null));
            Assert.AreEqual (false, _z.Equals (null));
            Assert.AreEqual (false, _a.Equals (null));
        }

        /// <summary>
        /// Confirms that if two instances of <typeparamref name="T"/> compare as equal, the
        /// <see cref="Object.GetHashCode()"/> method for each object returns the same value.
        /// </summary>
        [Test]
        public void AssertConsistentWithHashCode()
        {
            Assert.AreEqual (_x.GetHashCode (), _y.GetHashCode ());
            Assert.AreEqual (_y.GetHashCode (), _z.GetHashCode ());
            Assert.AreEqual (_x.GetHashCode (), _z.GetHashCode ());
        }
    }
}
