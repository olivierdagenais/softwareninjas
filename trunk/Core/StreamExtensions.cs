using System.Collections.Generic;
using System.IO;

namespace SoftwareNinjas.Core
{
    /// <summary>
    /// Provides extension methods for <see cref="Stream"/>.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Enumerates the bytes in the provided <paramref name="stream"/>.
        /// </summary>
        /// 
        /// <param name="stream">
        /// The <see cref="Stream"/> to read, one byte at a time.
        /// </param>
        /// 
        /// <returns>
        /// A <see cref="byte"/> sequence originating from the specified <paramref name="stream"/>.
        /// </returns>
        public static IEnumerable<byte> EnumerateBytes(this Stream stream)
        {
            int @byte;
            while ((@byte = stream.ReadByte ()) != -1)
            {
                yield return (byte) @byte;
            }
        }
    }
}
