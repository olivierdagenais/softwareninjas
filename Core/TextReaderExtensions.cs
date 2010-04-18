using System.Collections.Generic;
using System.IO;

namespace SoftwareNinjas.Core
{
    /// <summary>
    /// Provides extension methods for <see cref="TextReader"/>.
    /// </summary>
    public static class TextReaderExtensions
    {
        /// <summary>
        /// Enumerates the lines of text provided by the <paramref name="reader"/>.
        /// </summary>
        /// 
        /// <param name="reader">
        /// The <see cref="TextReader"/> to process on a line-by-line basis.
        /// </param>
        /// 
        /// <returns>
        /// A sequence of the lines of text provided by the stream underlying the specified <paramref name="reader"/>.
        /// </returns>
        public static IEnumerable<string> Lines(this TextReader reader)
        {
            string line;
            while (( line = reader.ReadLine() ) != null)
            {
                yield return line;
            }
        }
    }
}
