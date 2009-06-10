using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SoftwareNinjas.Core
{
    /// <summary>
    /// Extension methods to augment the <see cref="Assembly"/> class.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Convenience method to obtain the first (and usually only) assembly-level attribute of type
        /// <typeparamref name="T"/> from the specified <paramref name="assembly"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of an <see cref="Attribute"/> known (or thought) to be applied at the assembly level.
        /// </typeparam>
        /// 
        /// <param name="assembly">
        /// The <see cref="Assembly"/> to query for an attribute of type <typeparamref name="T"/>.
        /// </param>
        /// 
        /// <returns>
        /// An instance of <typeparamref name="T"/> representing the applied attribute if such an attribute was found;
        /// <see langword="null"/> otherwise.
        /// </returns>
        public static T GetCustomAttribute<T>(this Assembly assembly) where T : Attribute
        {
            object[] attributes = assembly.GetCustomAttributes(typeof(T), false);
            T result = null;
            if ( attributes != null && attributes.Length > 0 )
            {
                result = (T) attributes[0];
            }
            return result;
        }

        /// <summary>
        /// Combines a few attributes decorating the provided <paramref name="assembly"/> into a descriptive string to
        /// inform end-users about the product, version, copyright and registered user.
        /// </summary>
        /// 
        /// <param name="assembly">
        /// The <see cref="Assembly"/> to query.
        /// </param>
        /// 
        /// <returns>
        /// A user-friendly string representing two lines of text.
        /// </returns>
        public static string GenerateHeader(this Assembly assembly)
        {
            string product = assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
            string copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
            string version = assembly.GetName().Version.ToString(3);
            var registeredUser = RegisteredUserAttribute.ExtractFromCallingAssembly();

            StringBuilder result = new StringBuilder();
            result.AppendFormat("{0} version {1} - {2}", product, version, copyright);
            result.AppendLine();
            result.AppendFormat("Registered to: {0} <{1}>", registeredUser.DisplayName, registeredUser.EmailAddress);
            return result.ToString();
        }
    }
}
