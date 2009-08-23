using System;
using System.Globalization;

namespace SoftwareNinjas.Core
{
    /// <summary>
    /// Extension methods to augment implementations of <see cref="String"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Replaces the format item in a specified <see cref="String"/> with the text equivalent of the value of a
        /// corresponding <see cref="Object"/> instance in a specified array, using the
        /// <see cref="CultureInfo.InvariantCulture"/> for formatting.
        /// </summary>
        /// 
        /// <param name="format">
        /// A composite format string.
        /// </param>
        /// 
        /// <param name="args">
        /// An <see cref="Object"/> array containing zero or more objects to format. 
        /// </param>
        /// 
        /// <returns>
        /// A copy of <paramref name="format"/> in which the format items have been replaced by the <see cref="String"/>
        /// equivalent of the corresponding instances of <see cref="Object"/> in <paramref name="args"/>.
        /// </returns>
        /// 
        /// <seealso cref="String.Format(IFormatProvider, String, Object[])"/>
        public static string FormatInvariant(this string format, params object[] args)
        {
            return String.Format(CultureInfo.InvariantCulture, format, args);
        }

        /// <summary>
        /// Replaces the format item in a specified <see cref="String"/> with the text equivalent of the value of a
        /// corresponding <see cref="Object"/> instance in a specified array, using the
        /// <see cref="CultureInfo.CurrentCulture"/> for formatting.
        /// </summary>
        /// 
        /// <param name="format">
        /// A composite format string.
        /// </param>
        /// 
        /// <param name="args">
        /// An <see cref="Object"/> array containing zero or more objects to format. 
        /// </param>
        /// 
        /// <returns>
        /// A copy of <paramref name="format"/> in which the format items have been replaced by the <see cref="String"/>
        /// equivalent of the corresponding instances of <see cref="Object"/> in <paramref name="args"/>.
        /// </returns>
        /// 
        /// <seealso cref="String.Format(IFormatProvider, String, Object[])"/>
        public static string FormatCurrentCulture(this string format, params object[] args)
        {
            return String.Format(CultureInfo.CurrentCulture, format, args);
        }

        /// <summary>
        /// Replaces the format item in a specified <see cref="String"/> with the text equivalent of the value of a
        /// corresponding <see cref="Object"/> instance in a specified array, using the
        /// <see cref="CultureInfo.CurrentUICulture"/> for formatting.
        /// </summary>
        /// 
        /// <param name="format">
        /// A composite format string.
        /// </param>
        /// 
        /// <param name="args">
        /// An <see cref="Object"/> array containing zero or more objects to format. 
        /// </param>
        /// 
        /// <returns>
        /// A copy of <paramref name="format"/> in which the format items have been replaced by the <see cref="String"/>
        /// equivalent of the corresponding instances of <see cref="Object"/> in <paramref name="args"/>.
        /// </returns>
        /// 
        /// <seealso cref="String.Format(IFormatProvider, String, Object[])"/>
        public static string FormatCurrentUICulture(this string format, params object[] args)
        {
            return String.Format(CultureInfo.CurrentUICulture, format, args);
        }

        /// <summary>
        /// Replaces the format item in a specified <see cref="String"/> with the text equivalent of the value of a
        /// corresponding <see cref="Object"/> instance in a specified array.  A specified parameter supplies
        /// culture-specific formatting information.
        /// </summary>
        /// 
        /// <param name="format">
        /// A composite format string.
        /// </param>
        /// 
        /// <param name="provider">
        /// An <see cref="IFormatProvider"/> that supplies culture-specific formatting information.
        /// </param>
        /// 
        /// <param name="args">
        /// An <see cref="Object"/> array containing zero or more objects to format. 
        /// </param>
        /// 
        /// <returns>
        /// A copy of <paramref name="format"/> in which the format items have been replaced by the <see cref="String"/>
        /// equivalent of the corresponding instances of <see cref="Object"/> in <paramref name="args"/>.
        /// </returns>
        /// 
        /// <seealso cref="String.Format(IFormatProvider, String, Object[])"/>
        public static string FormatProvider(this string format, IFormatProvider provider, params object[] args)
        {
            return String.Format(provider, format, args);
        }
    }
}
