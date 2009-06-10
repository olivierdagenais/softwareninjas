using System;

namespace SoftwareNinjas.Core
{
    /// <summary>
    /// Provides a basic utility class that is used to store two related items.
    /// </summary>
    /// 
    /// <typeparam name="F">
    /// The type of the first item.
    /// </typeparam>
    /// 
    /// <typeparam name="S">
    /// The type of the second item.
    /// </typeparam>
    /// 
    /// <seealso href="http://msdn.microsoft.com/en-us/library/system.web.ui.pair.aspx">System.Web.UI.Pair</seealso>
    public sealed class Pair<F, S>
    {
        private F _first;
        private S _second;

        /// <summary>
        /// Initializes an instance of the <see cref="Pair{F,S}"/> class, using the specified item pair.
        /// </summary>
        /// 
        /// <param name="first">
        /// The first item to set.
        /// </param>
        /// 
        /// <param name="second">
        /// The second item to set.
        /// </param>
        public Pair(F first, S second)
        {
            _first = first;
            _second = second;
        }

        /// <summary>
        /// The first item.
        /// </summary>
        public F First
        {
            get
            {
                return _first;
            }
        }

        /// <summary>
        /// The second item.
        /// </summary>
        public S Second
        {
            get
            {
                return _second;
            }
        }
    }
}
