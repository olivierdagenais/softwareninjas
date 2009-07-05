using System;
using System.IO;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using Microsoft.Win32;

using SoftwareNinjas.Core;
using SoftwareNinjas.Core.Process;

namespace SoftwareNinjas.MSBuild
{
    /// <summary>
    /// Compiles <b>Java</b> source code into class files.
    /// </summary>
    /// 
    /// <example>
    ///   <para>
    ///     Compiles all files marked as "Compile", uses all "Reference" items to build up a classpath and copies any
    ///     files marked as "Resource" to the target folder specified by OutputAssembly.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <JavaCompiler
    ///     DefineConstants="$(DefineConstants"
    ///     DisabledWarnings="$(_DisabledWarnings)"
    ///     EmitDebugInformation="$(DebugSymbols)"
    ///     Optimize="$(Optimize)"
    ///     OutputAssembly="@(IntermediateAssembly)"
    ///     References="@(ReferencePath)"
    ///     Resources="@(ManifestResourceWithNoCulture);@(ManifestNonResxWithNoCultureOnDisk);@(CompiledLicenseFile)"
    ///     Sources="@(Compile)"
    ///     />
    ///     ]]>
    ///   </code>
    /// </example>
    public class JavaCompiler : Task
    {
        private const string javaDevelopmentKit16 =
            @"HKEY_LOCAL_MACHINE\Software\JavaSoft\Java Development Kit\1.6";

        #region Properties
        /// <summary>
        /// Compilation-time parameters.
        /// </summary>
        public string[] DefineConstants
        {
            get;
            set;
        }

        /// <summary>
        /// Java compiler warnings to disable.
        /// </summary>
        public string[] DisabledWarnings
        {
            get;
            set;
        }

        /// <summary>
        /// Whether to compile the code with meta-data that aids debuggers.
        /// </summary>
        public bool EmitDebugInformation
        {
            get;
            set;
        }

        /// <summary>
        /// Whether to try to increase the performance of the compiled code by transforming it.
        /// </summary>
        public bool Optimize
        {
            get;
            set;
        }

        /// <summary>
        /// Uses this path to determine the compilation's output folder.
        /// </summary>
        public string OutputAssembly
        {
            get;
            set;
        }

        /// <summary>
        /// Shared libraries used by the code.
        /// </summary>
        public ITaskItem[] References
        {
            get;
            set;
        }

        /// <summary>
        /// Files not compiled but nonetheless included alongside compiled source code.
        /// </summary>
        public ITaskItem[] Resources
        {
            get;
            set;
        }

        /// <summary>
        /// Files to be compiled.
        /// </summary>
        public ITaskItem[] Sources
        {
            get;
            set;
        }
        #endregion

        internal static string GenerateFullPathToCompiler()
        {
            string javaHomePath = Registry.GetValue(javaDevelopmentKit16, "JavaHome", null) as string;
            if (null == javaHomePath)
            {
                throw new ArgumentException("Unable to find an installation of the Java Development Kit (JDK) 1.6");
            }
            string compiler = Path.Combine(javaHomePath, "bin/javac.exe");
            return compiler;
        }

        internal static string GenerateCompilerCommandLine(
            string outputFolder, ITaskItem[] references, ITaskItem[] sources, bool emitDebugInformation,
            string[] disabledWarnings)
        {
            var builder = new CommandLineBuilder();

            #region javac <options> <source files>
            /* where possible options include:
  -g                         Generate all debugging info
  -g:none                    Generate no debugging info
  -g:{lines,vars,source}     Generate only some debugging info
  -nowarn                    Generate no warnings
  -verbose                   Output messages about what the compiler is doing
  -deprecation               Output source locations where deprecated APIs are used
  -classpath <path>          Specify where to find user class files and annotation processors
  -cp <path>                 Specify where to find user class files and annotation processors
  -sourcepath <path>         Specify where to find input source files
  -bootclasspath <path>      Override location of bootstrap class files
  -extdirs <dirs>            Override location of installed extensions
  -endorseddirs <dirs>       Override location of endorsed standards path
  -proc:{none,only}          Control whether annotation processing and/or compilation is done.
  -processor <class1>[,<class2>,<class3>...]Names of the annotation processors to run; bypasses default discovery process
  -processorpath <path>      Specify where to find annotation processors
  -d <directory>             Specify where to place generated class files
  -s <directory>             Specify where to place generated source files
  -implicit:{none,class}     Specify whether or not to generate class files for implicitly referenced files
  -encoding <encoding>       Specify character encoding used by source files
  -source <release>          Provide source compatibility with specified release
  -target <release>          Generate class files for specific VM version
  -version                   Version information
  -help                      Print a synopsis of standard options
  -Akey[=value]              Options to pass to annotation processors
  -X                         Print a synopsis of nonstandard options
  -J<flag>                   Pass <flag> directly to the runtime system */
            #endregion

            #region Debugging information
            if (emitDebugInformation)
            {
                builder.AppendSwitch("-g");
            }
            else
            {
                builder.AppendSwitch("-g:none");
            }
            #endregion

            builder.AppendSwitch("-d");
            builder.AppendFileNameIfNotNull(outputFolder);

            builder.AppendSwitch("-Xlint:all");
            if (disabledWarnings != null)
            {
                foreach (string warning in disabledWarnings)
                {
                    builder.AppendSwitch("-Xlint:-" + warning);
                }
            }

            #region Project references built into a classpath
            if (references != null)
            {
                builder.AppendSwitch("-classpath");
                builder.AppendSwitch('"' + references.Join(";", (item) => item.GetMetadata("HintPath")) + '"');
            }
            #endregion

            builder.AppendFileNamesIfNotNull(sources, " ");

            var result = builder.ToString();
            return result;
        }

        /// <summary>
        /// Performs the compilation and copying.
        /// </summary>
        /// 
        /// <returns>
        /// <see langword="true"/> if the processing succeeded; <see langword="false"/> otherwise.
        /// </returns>
        public override bool Execute()
        {
            // TODO: We may eventually want to check first if the output JAR is up-to-date against the inputs
            // TODO: We might need to delete the contents of the obj folder first
            var process = new CapturedProcess(
                GenerateFullPathToCompiler(),
                null, 
                (line) => Log.LogMessage(MessageImportance.Normal, line), 
                (line) => Log.LogMessage(MessageImportance.High, line));

            var outputFolder = Path.GetDirectoryName(OutputAssembly);
            process.ArgumentString =
                GenerateCompilerCommandLine(outputFolder, References, Sources, EmitDebugInformation, DisabledWarnings);
            Log.LogCommandLine(MessageImportance.Normal, process.ArgumentString);

            int exitCode = process.Run();
            if (exitCode != 0)
            {
                Log.LogError("Process exited with code {0}", exitCode);
            }
            else
            {
                #region Copy resource files to outputFolder
                if (Resources != null)
                {
                    foreach (ITaskItem item in Resources)
                    {
                        var inputFile = item.ItemSpec;
                        var outputFile = Path.Combine(outputFolder, item.ItemSpec);
                        Log.LogMessage(MessageImportance.Low, "Copying {0} to {1}...", inputFile, outputFile);
                        var outputDir = Path.GetDirectoryName(outputFile);
                        Directory.CreateDirectory(outputDir);
                        File.Copy(inputFile, outputFile, true);
                    }
                }
                #endregion
            }
            return !Log.HasLoggedErrors;
        }
    }
}
