using System.IO;

using Parent = SoftwareNinjas.Core;
using NUnit.Framework;

namespace SoftwareNinjas.Core.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.StreamExtensions"/>.
    /// </summary>
    [TestFixture]
    public class StreamExtensions
    {
        /// <summary>
        /// Tests the <see cref="Parent.StreamExtensions.EnumerateBytes" /> method with
        /// a memory stream that was initialized from a byte array.
        /// </summary>
        [Test]
        public void EnumerateBytes_MemoryStreamFromByteArray ()
        {
            var expectedBytes = new byte[] { 4, 8, 15, 16, 23, 42 };
            using (var stream = new MemoryStream(expectedBytes))
            {
                var actualBytes = Parent.StreamExtensions.EnumerateBytes (stream);
                EnumerableExtensions.EnumerateSame (expectedBytes, actualBytes);
            }
        }

    }
}
