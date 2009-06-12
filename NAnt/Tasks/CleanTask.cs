using System;
using System.Collections.Generic;
using System.IO;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace SoftwareNinjas.NAnt.Tasks
{
    /// <summary>
    /// Deletes output folders for a set of projects.  Each project is assumed to be in its own folder.
    /// </summary>
    /// 
    /// <example>
    ///   <para>
    ///     Deletes the <b>obj</b> and <b>bin</b> folders for each of the <b>Core</b> and <b>NAnt</b> projects.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <clean projects="Core,NAnt" />
    ///     ]]>
    ///   </code>
    /// </example>
    /// 
    /// <example>
    ///   <para>
    ///     Deletes the <b>Debug</b> sub-folder of the <b>obj</b> and <b>bin</b> folders for each of the <b>Core</b>
    ///     and <b>NAnt</b> projects, located under the <b>c:\DotNet</b> folder.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <clean projects="Core,NAnt" basedir="c:\DotNet" configuration="Debug" />
    ///     ]]>
    ///   </code>
    /// </example>
    [TaskName("clean")]
    public class CleanTask : TestableTask
    {
        private static readonly string[] directoriesToClean = { "obj", "bin" };

        /// <summary>
        /// Default constructor for NAnt itself.
        /// </summary>
        public CleanTask() : this(true)
        {
        }

        /// <summary>
        /// Parameterized constructor for unit testing.
        /// </summary>
        /// 
        /// <param name="logging">
        /// Whether logging is enabled or not.
        /// </param>
        internal CleanTask(bool logging) : base(logging)
        {
        }

        /// <summary>
        /// The directory in which the projects are found.  The default is the project base directory.
        /// </summary>
        [TaskAttribute("basedir", Required = false)]
        public DirectoryInfo BaseDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// The name of a specific sub-folder under the <b>obj</b> and <b>bin</b> sub-folders to delete.  The default
        /// is all sub-folders.
        /// </summary>
        [TaskAttribute("configuration", Required = false)]
        public string Configuration
        {
            get;
            set;
        }

        /// <summary>
        /// A comma-separated list of strings representing sub-folder names where cleaning is to be performed.
        /// </summary>
        [TaskAttribute("projects", Required = true)]
        public string Projects
        {
            get;
            set;
        }

        /// <summary>
        /// Performs the cleaning.
        /// </summary>
        protected override void ExecuteTask()
        {
            if (null == BaseDirectory)
            {
                BaseDirectory = new DirectoryInfo(Project.BaseDirectory);
            }

            foreach (string project in ParseProjects(Projects))
            {
                Log(Level.Info, "Cleaning {0}...", project);
                var projectDir = Path.Combine(BaseDirectory.FullName, project);
                foreach (string dirName in directoriesToClean)
                {
                    var subDir = Path.Combine(projectDir, dirName);
                    if (Configuration != null)
                    {
                        subDir = Path.Combine(subDir, Configuration);
                    }
                    if (Directory.Exists(subDir))
                    {
                        Log(Level.Verbose, "Deleting {0}...", subDir);
                        Directory.Delete(subDir, true);
                    }
                    else
                    {
                        Log(Level.Verbose, "Directory '{0}' does not exist", subDir);
                    }
                }
            }
        }

        internal static IEnumerable<String> ParseProjects(string input)
        {
            var inputs = input.Split(',');
            foreach (var project in inputs)
            {
                yield return project.Trim();
            }
        }
    }
}
