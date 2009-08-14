using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.Win32;
using NAnt.Core;
using NAnt.Core.Attributes;
using SoftwareNinjas.Core.Process;
using System.Globalization;

namespace SoftwareNinjas.NAnt.Tasks
{
    /// <summary>
    /// Invokes the Mainsoft for Java EE 2.5 assembly to JAR converter.
    /// </summary>
    /// 
    /// <example>
    ///   <para>
    ///     Converts the NUnit Framework assembly to a JAR in the same folder as the input file.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <assemblyToJar assemblyPaths="Tools/nant/bin/lib/net/2.0/nunit.framework.dll" />
    ///     ]]>
    ///   </code>
    /// </example>
    [TaskName("assemblyToJar")]
    public class AssemblyToJarTask : TestableTask
    {
        private const string mainsoftForJava25 = 
            "HKEY_LOCAL_MACHINE\\SOFTWARE\\Mainsoft\\Mainsoft for Java EE 2.5\\9.0";

        private const string netFramework =
            "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\.NETFramework";

        private readonly ICapturedProcessFactory _capturedProcessFactory;

        /// <summary>
        /// Default constructor for NAnt itself.
        /// </summary>
        public AssemblyToJarTask()
            : this(true, new CapturedProcessFactory())
        {
        }

        /// <summary>
        /// Parameterized constructor for unit testing.
        /// </summary>
        /// 
        /// <param name="logging">
        /// Whether logging is enabled or not.
        /// </param>
        /// 
        /// <param name="capturedProcessFactory">
        /// An implementation of <see cref="ICapturedProcessFactory"/> to use for creating
        /// <see cref="ICapturedProcess"/> instances.
        /// </param>
        internal AssemblyToJarTask(bool logging, ICapturedProcessFactory capturedProcessFactory)
            : base(logging)
        {
            _capturedProcessFactory = capturedProcessFactory;
        }

        /// <summary>
        /// A comma-separated list of strings representing paths to assemblies to be converted to JARs.
        /// </summary>
        [TaskAttribute("assemblyPaths", Required = true)]
        public string AssemblyPaths
        {
            get;
            set;
        }

        /// <summary>
        /// Performs the conversion.
        /// </summary>
        protected override void ExecuteTask()
        {
            string mainsoftBinPath = Registry.GetValue(mainsoftForJava25, "basedir", null) as string;
            if (null == mainsoftBinPath)
            {
                throw new BuildException("Unable to find an installation of Mainsoft for Java EE 2.5");
            }
            string converter = Path.Combine(mainsoftBinPath, "Converter.exe");

            string installRoot = Registry.GetValue(netFramework, "InstallRoot", null) as string;
            if (null == installRoot)
            {
                throw new BuildException("Unable to find an installation of .NET");
            }
            string netfx2 = Path.Combine(installRoot, "v2.0.50727");

            foreach (FileInfo assemblyPath in ParseAssemblyPaths(AssemblyPaths))
            {
                Log(Level.Info, "Converting {0}...", assemblyPath.FullName);
                var outLines = new StringBuilder();
                var errLines = new StringBuilder();
                int exitCode;
                using (ICapturedProcess process = _capturedProcessFactory.Create(
                    converter, 
                    new string[]
                    {
                        assemblyPath.FullName,
                        "/lib:" + netfx2 + ";" + assemblyPath.DirectoryName,
                        "/out:" + Path.ChangeExtension(assemblyPath.FullName, ".jar"),
                    }, 
                    (line) => outLines.AppendLine(line),
                    (line) => errLines.AppendLine(line)))
                {
                    exitCode = process.Run();
                }
                if (exitCode != 0)
                {
                    Log(Level.Error, outLines.ToString());
                    Log(Level.Error, errLines.ToString());
                    var message = String.Format(
                        CultureInfo.InvariantCulture, "Conversion returned exit code {0}.", exitCode);
                    throw new BuildException(message);
                }
                var tempFile = Path.ChangeExtension(assemblyPath.FullName, ".vmwdb");
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        internal static IEnumerable<FileInfo> ParseAssemblyPaths(string input)
        {
            var inputs = input.Split(',');
            foreach (var assemblyPath in inputs)
            {
                string trimmedPath = assemblyPath.Trim();
                FileInfo result = new FileInfo(trimmedPath);
                yield return result;
            }
        }
    }
}
