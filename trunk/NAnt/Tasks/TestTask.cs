using System.Collections.Generic;
using System.IO;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.NUnit.Types;
using NAnt.NUnit2.Tasks;
using NAnt.NUnit2.Types;

using SoftwareNinjas.Core;
using SoftwareNinjas.Core.Process;
using ICSharpCode.SharpZipLib.Zip;

namespace SoftwareNinjas.NAnt.Tasks
{
    /// <summary>
    /// Runs NUnit- or JUnit-based unit tests for all projects.
    /// </summary>
    /// 
    /// <example>
    ///   <para>
    ///     Runs the tests contained in each of the outputs of the <b>Core</b> and <b>NAnt</b> projects.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <test projects="Core,NAnt" />
    ///     ]]>
    ///   </code>
    /// </example>
    [TaskName("test")]
    public class TestTask : AbstractProjectTask
    {
        private readonly ICapturedProcessFactory _capturedProcessFactory;

        /// <summary>
        /// Default constructor for NAnt itself.
        /// </summary>
        public TestTask()
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
        internal TestTask(bool logging, ICapturedProcessFactory capturedProcessFactory)
            : base(logging)
        {
            _capturedProcessFactory = capturedProcessFactory;
        }
        
        /// <summary>
        /// Usually one of <b>debug</b> or <b>release</b>.
        /// </summary>
        [TaskAttribute("configuration", Required=true)]
        public string Configuration
        {
            get;
            set;
        }

        /// <summary>
        /// Performs the testing.
        /// </summary>
        protected override void ExecuteTask()
        {
            bool hasErrors = false;
            string pathToJava = null;
            foreach (var project in EnumerateProjects())
            {
                Log(Level.Info, "Testing {0}...", project.Name);
                var outputFolder = Path.Combine(project.Folder, "bin/" + Configuration);
                var outputFilePath = Path.Combine(outputFolder, project.OutputName);
                if (project.Language == SupportedLanguage.Java)
                {
                    if (pathToJava == null)
                    {
                        pathToJava = Java.GenerateFullPathToRuntime();
                    }
                    hasErrors |= TestWithJUnit(pathToJava, project, outputFilePath);
                }
                else
                {
                    hasErrors |= TestWithNUnit(outputFilePath);
                }
            }
            if (hasErrors)
            {
                throw new BuildException("Tests failed");
            }
        }

        private bool TestWithNUnit(string outputFilePath)
        {
            bool hasErrors = false;
            #region <nunit2>
            var task = new NUnit2Task();
            // this little assignment makes the whole TestTask very difficult to unit test
            // unless maybe we subclass Project for testing?
            task.Project = Project;

            #region <formatter type="Plain" />
            var formatter = new FormatterElement();
            formatter.Type = FormatterType.Plain;
            task.FormatterElements.Add(formatter);
            #endregion

            #region <test assemblyname="outputFilePath" />
            var test = new NUnit2Test();
            test.AssemblyFile = new FileInfo(outputFilePath);
            task.Tests.Add(test);
            #endregion

            try
            {
                task.Execute();
            }
            catch (BuildException be)
            {
                hasErrors = true;
                Log(Level.Error, be.Message);
            }
            #endregion
            return hasErrors;
        }

        internal static bool IsTestClassFile(string fileName)
        {
            return fileName.EndsWith("Test.class");
        }

        internal static string ConvertClassFilenameToClassName(string fileName)
        {
            return fileName.Replace('/', '.').Replace(".class", "");
        }

        private bool TestWithJUnit(string pathToJava, Project project, string outputFilePath)
        {
            bool hasErrors = false;
            int exitCode;

            var requiredJars = project.References.Compose(new FileInfo(outputFilePath));

            var filesInZip = EnumerateFilesInZip(outputFilePath);
            var testFilesInZip = filesInZip.Filter(IsTestClassFile);
            var testClasses = testFilesInZip.Map<string, string>(ConvertClassFilenameToClassName);
            var arguments = new string[]
            {
                "-classpath",
                requiredJars.Join(";", item => item.FullName),
                "org.junit.runner.JUnitCore"
            }.Compose(testClasses);
            using (ICapturedProcess process = _capturedProcessFactory.Create(
                pathToJava,
                arguments.Map(str => (object) str) /* For some reason, IEnumerable<string> is not acceptable */,
                line => Log(Level.Info, line),
                line => Log(Level.Error, line)))
            {
                exitCode = process.Run();
            }
            if (exitCode != 0)
            {
                hasErrors = true;
            }
            return hasErrors;
        }

        internal static IEnumerable<string> EnumerateFilesInZip(string zipFile)
        {
            using (var inputStream = File.Open(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var zis = new ZipInputStream(inputStream))
                {
                    ZipEntry entry;
                    while (( entry = zis.GetNextEntry() ) != null)
                    {
                        if (entry.IsFile)
                        {
                            yield return entry.Name;
                        }
                    }
                }
            }
        }
    }
}
