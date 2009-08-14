using System;
using System.IO;
using Microsoft.Win32;

namespace SoftwareNinjas.Core
{
    /// <summary>
    /// Convenience methods for Sun's programming language runtime and tools.
    /// </summary>
    public class Java
    {
        private const string javaDevelopmentKit16 =
            @"HKEY_LOCAL_MACHINE\Software\JavaSoft\Java Development Kit\1.6";

        /// <summary>
        /// Determines the value of JAVA_HOME for the Java Development Kit (JDK), version 6 (also known as 1.6).
        /// </summary>
        /// 
        /// <returns>
        /// The full path to the root of the JDK 1.6 installation.
        /// </returns>
        /// 
        /// <exception cref="FileNotFoundException">
        /// If the Java Development Kit (JDK) 1.6 cannot be found.
        /// </exception>
        public static string GenerateFullPathToJavaDevelopmentKitHome()
        {
            string jdkHomePath = Registry.GetValue(javaDevelopmentKit16, "JavaHome", null) as string;
            if (null == jdkHomePath)
            {
                throw new FileNotFoundException("Unable to find an installation of the Java Development Kit (JDK) 1.6");
            }
            return jdkHomePath;
        }

        /// <summary>
        /// Determines the full path to the version 6 Java compiler.
        /// </summary>
        /// 
        /// <returns>
        /// The full path to <c>javac.exe</c>.
        /// </returns>
        /// 
        /// <exception cref="FileNotFoundException">
        /// If the Java Development Kit (JDK) 1.6 cannot be found.
        /// </exception>
        public static string GenerateFullPathToCompiler()
        {
            var jdkHomePath = GenerateFullPathToJavaDevelopmentKitHome();
            string compiler = Path.Combine(jdkHomePath, "bin/javac.exe");
            return compiler;
        }

        /// <summary>
        /// Determines the full path to the version 6 Java runtime.
        /// </summary>
        /// 
        /// <returns>
        /// The full path to <c>java.exe</c>.
        /// </returns>
        /// 
        /// <exception cref="FileNotFoundException">
        /// If the Java Development Kit (JDK) 1.6 cannot be found.
        /// </exception>
        public static string GenerateFullPathToRuntime()
        {
            var jdkHomePath = GenerateFullPathToJavaDevelopmentKitHome();
            string compiler = Path.Combine(jdkHomePath, "bin/java.exe");
            return compiler;
        }
    }
}
