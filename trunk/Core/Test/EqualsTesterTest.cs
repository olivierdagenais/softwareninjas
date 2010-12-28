using NUnit.Framework;

namespace SoftwareNinjas.Core.Test
{
    /// <summary>
    /// A class to test <see cref="EqualsTester{T}"/>.
    /// </summary>
    [TestFixture]
    public class EqualsTesterTest
    {
        /// <summary>
        /// Tests the <see cref="EqualsTester{T}.Run()" /> method with
        /// x, y and z that aren't all equal to one another.
        /// </summary>
        [Test, ExpectedException(typeof(AssertionException))]
        public void Run_InvalidAssumptions_FirstThreeNotEqual ()
        {
            var x = new Pair<int, int> (1, 2);
            var y = new Pair<int, int> (2, 1);
            var z = new Pair<int, int> (1, 2);
            var a = new Pair<int, int> (0, 0);

            var tester = new EqualsTester<Pair<int, int>> (x, y, z, a);
            tester.Run ();
        }

        /// <summary>
        /// Tests the <see cref="EqualsTester{T}.Run()" /> method with
        /// a equal to x, y and z.
        /// </summary>
        [Test, ExpectedException (typeof (AssertionException))]
        public void Run_InvalidAssumptions_LastOneNotDifferent ()
        {
            var x = new Pair<int, int> (1, 2);
            var y = new Pair<int, int> (1, 2);
            var z = new Pair<int, int> (1, 2);
            var a = new Pair<int, int> (1, 2);

            var tester = new EqualsTester<Pair<int, int>> (x, y, z, a);
            tester.Run ();
        }
    }
}
