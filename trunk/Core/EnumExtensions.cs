using System;

namespace SoftwareNinjas.Core
{
    /// <summary>
    /// Extension methods to augment implementations of <see cref="Enum"/>.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Determines if an <see cref="Enum"/> value contains the specified <paramref name="flag"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of the <paramref name="enumValue"/> and the <paramref name="flag"/>.
        /// </typeparam>
        /// 
        /// <param name="enumValue">
        /// An enum value to be tested.
        /// </param>
        /// 
        /// <param name="flag">
        /// An enum value to test for.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if <paramref name="flag"/> is found to make up [part of]
        /// <paramref name="enumValue"/>; <see langword="false"/> otherwise.
        /// </returns>
        /// 
        /// <example>
        /// <para>
        /// The following code example prints the first letter of the words representing the colours making up a
        /// user-provided colour.
        /// </para>
        /// 
        /// <code lang="C#">
        /// <![CDATA[
        /// using System;
        /// using SoftwareNinjas.Core;
        /// 
        /// [Flags]
        /// enum PrimaryColours
        /// {
        ///     Red = 1,
        ///     Green = 2,
        ///     Blue = 4,
        /// }
        /// 
        /// class HasFlagDemo
        /// {
        ///     static void DescribePrimaryColours(PrimaryColours targetColour)
        ///     {
        ///         Console.Write ( targetColour.HasFlag(PrimaryColours.Red)   ? "R" : " " );
        ///         Console.Write ( targetColour.HasFlag(PrimaryColours.Green) ? "G" : " " );
        ///         Console.Write ( targetColour.HasFlag(PrimaryColours.Blue)  ? "B" : " " );
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// 
        /// <remarks>
        /// This method is simply a shortcut to performing a bitwise AND operation between the integer representation of
        /// <paramref name="enumValue"/> and the integer representation of <paramref name="flag"/> and testing the
        /// result for equality to <paramref name="flag"/>.
        /// </remarks>
        /// 
        /// <seealso cref="Enum"/>
        public static bool HasFlag<T>(this T enumValue, T flag) where T : IConvertible
        {
            int enumValueAsInteger = Convert.ToInt32(enumValue);
            int flagAsInteger = Convert.ToInt32(flag);
            bool result = (enumValueAsInteger & flagAsInteger) == flagAsInteger;
            return result;
        }
    }
}
