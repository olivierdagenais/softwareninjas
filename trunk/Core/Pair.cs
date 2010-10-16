using System;

namespace SoftwareNinjas.Core
{
    /// <summary>
    /// Provides a basic utility class that is used to store two related items.
    /// </summary>
    /// 
    /// <typeparam name="TFirst">
    /// The type of the first item.
    /// </typeparam>
    /// 
    /// <typeparam name="TSecond">
    /// The type of the second item.
    /// </typeparam>
    /// 
    /// <seealso href="http://msdn.microsoft.com/en-us/library/system.web.ui.pair.aspx">System.Web.UI.Pair</seealso>
    public sealed class Pair<TFirst, TSecond>
    {
        private readonly TFirst _first;
        private readonly TSecond _second;

        /// <summary>
        /// Initializes an instance of the <see cref="Pair{TFirst,TSecond}"/> class, using the specified item pair.
        /// </summary>
        /// 
        /// <param name="first">
        /// The first item to set.
        /// </param>
        /// 
        /// <param name="second">
        /// The second item to set.
        /// </param>
        public Pair(TFirst first, TSecond second)
        {
            _first = first;
            _second = second;
        }

        /// <summary>
        /// The first item.
        /// </summary>
        public TFirst First
        {
            get
            {
                return _first;
            }
        }

        /// <summary>
        /// The second item.
        /// </summary>
        public TSecond Second
        {
            get
            {
                return _second;
            }
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified <paramref name="object"/>.
        /// </summary>
        /// 
        /// <param name="object">
        /// An object to compare to this instance.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="object"/> is a <see cref="Pair{TFirst,TSecond}"/> and contains the
        /// same <see cref="First"/> and <see cref="Second"/> values as this <see cref="Pair{TFirst,TSecond}"/>;
        /// otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals (Object @object)
        {
            var that = @object as Pair<TFirst, TSecond>;
            if (null == that)
            {
                return false;
            }
            return Equals(_first, that._first) && Equals (_second, that._second);
        }

        /// <summary>
        /// Returns the hash code for this <see cref="Pair{TFirst,TSecond}"/>.
        /// </summary>
        /// 
        /// <returns>
        /// The hash code for this <see cref="Pair{TFirst,TSecond}"/> instance.
        /// </returns>
        public override int GetHashCode ()
        {
            var firstCode = Equals (default (TFirst), _first) ? 0 : _first.GetHashCode ();
            var secondCode = Equals (default (TSecond), _second) ? 0 : _second.GetHashCode ();
            return firstCode ^ secondCode;
        }

        /// <summary>
        /// Creates a <see cref="String"/> representation of this <see cref="Pair{TFirst,TSecond}"/>.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="String"/> containing the <see cref="First"/> and <see cref="Second"/> values of this
        /// <see cref="Pair{TFirst,TSecond}"/> instance.
        /// </returns>
        public override string ToString ()
        {
            var first = Equals (default (TFirst), _first) ? "default" : _first.ToString ();
            var second = Equals (default (TSecond), _second) ? "default" : _second.ToString ();
            var result = "<{0}, {1}>".FormatInvariant (first, second);
            return result;
        }
    }
}
