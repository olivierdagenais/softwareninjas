using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SoftwareNinjas.Core
{
    /// <summary>
    /// Represents data about the user for which an assembly was compiled.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=false)]
    public class RegisteredUserAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredUserAttribute"/> class with the specified
        /// human-readable version of the user's name and their e-mail address.
        /// </summary>
        /// 
        /// <param name="displayName">
        /// The full name of the user who registered the software.
        /// </param>
        /// 
        /// <param name="emailAddress">
        /// The e-mail addressed used when registering the assembly.
        /// </param>
        public RegisteredUserAttribute(string displayName, string emailAddress)
        {
            this.DisplayName = displayName;
            this.EmailAddress = emailAddress;
        }

        /// <summary>
        /// The full name of the user, to be used in labels, etc.
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// The e-mail address of the user, to be used in hyperlinks, etc.
        /// </summary>
        public string EmailAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredUserAttribute"/> class based on its use in the
        /// specified assembly.
        /// </summary>
        /// 
        /// <param name="assembly">
        /// The <see cref="Assembly"/> in which to search for the attribute.
        /// </param>
        /// 
        /// <returns>
        /// A <see cref="RegisteredUserAttribute"/> from the assembly.
        /// </returns>
        public static RegisteredUserAttribute ExtractFrom(Assembly assembly)
        {
            return assembly.GetCustomAttribute<RegisteredUserAttribute> ( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredUserAttribute"/> class based on its use in the
        /// calling assembly.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="RegisteredUserAttribute"/> from the calling assembly.
        /// </returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static RegisteredUserAttribute ExtractFromCallingAssembly()
        {
            return ExtractFrom(Assembly.GetCallingAssembly ( ));
        }
    }
}
