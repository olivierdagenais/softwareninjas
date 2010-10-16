using Parent = SoftwareNinjas.Core;
using NUnit.Framework;

namespace SoftwareNinjas.Core.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.Pair{TFirst,TSecond}"/>.
    /// </summary>
    [TestFixture]
    public class PairTest : EqualsTester<Pair<string, string>>
    {
        private static readonly Pair<string, string> X = new Pair<string, string> ("I say", "hello");
        private static readonly Pair<string, string> Y = new Pair<string, string> ("I say", "hello");
        private static readonly Pair<string, string> Z = new Pair<string, string> ("I say", "hello");
        private static readonly Pair<string, string> A = new Pair<string, string> ("obladi", "oblada");

        /// <summary>
        /// Initializes the <see cref="EqualsTester{T}"/> super-class with four instances of
        /// <see cref="Pair{TFirst,TSecond}"/>.
        /// </summary>
        public PairTest () : base (X, Y, Z, A)
        {
        }

        /// <summary>
        /// Tests the <see cref="Parent.Pair{TFirst,TSecond}.ToString()" /> method with
        /// the samples used for equality testing.
        /// </summary>
        [Test]
        public void ToString_UsingSamples ()
        {
            Assert.AreEqual ("<I say, hello>", X.ToString ());
            Assert.AreEqual ("<I say, hello>", Y.ToString ());
            Assert.AreEqual ("<I say, hello>", Z.ToString ());
            Assert.AreEqual ("<obladi, oblada>", A.ToString ());
        }
    }
}
