using System;
using System.Collections.Generic;
using System.IO;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace SoftwareNinjas.NAnt.Tasks
{
    /// <summary>
    /// Common code for operating on one or more Visual Studio projects.
    /// </summary>
    public abstract class AbstractProjectTask : TestableTask
    {
        /// <summary>
        /// Parameterized constructor for unit testing.
        /// </summary>
        ///
        /// <param name="logging">
        /// Whether logging is enabled or not.
        /// </param>
        protected AbstractProjectTask(bool logging) : base(logging)
        {
        }

        private DirectoryInfo _baseDirectory;
        /// <summary>
        /// The directory in which the projects are found.  The default is the project base directory.
        /// </summary>
        [TaskAttribute("basedir", Required = false)]
        public DirectoryInfo BaseDirectory
        {
            get
            {
                if (_baseDirectory == null && Project != null)
                {
                    _baseDirectory = new DirectoryInfo(Project.BaseDirectory);
                    Log(Level.Verbose, "Initializing default BaseDirectory to {0}", _baseDirectory.FullName);
                }
                return _baseDirectory;
            }
            set
            {
                _baseDirectory = value;
            }
        }

        /// <summary>
        /// A comma-separated list of strings representing sub-folder names where processing is to be performed.
        /// </summary>
        [TaskAttribute("projects", Required = true)]
        public string Projects
        {
            get;
            set;
        }

        /// <summary>
        /// Allows sub-classes to process <see cref="Project"/> instances one by one.
        /// </summary>
        /// 
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="Project"/> instances representing individual projects.
        /// </returns>
        public IEnumerable<Project> EnumerateProjects()
        {
            ProjectLoader loader = new ProjectLoader();
            var solutionFolder = BaseDirectory.FullName;
            foreach (string projectName in EnumerateProjectNames())
            {
                yield return loader.Create(solutionFolder, projectName);
            }
        }

        /// <summary>
        /// Allows sub-classes to process project names one by one.
        /// </summary>
        /// 
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="String"/> representing individual projects.
        /// </returns>
        public IEnumerable<string> EnumerateProjectNames()
        {
            return ParseProjectNames(Projects);
        }

        internal static IEnumerable<String> ParseProjectNames(string input)
        {
            var inputs = input.Split(',');
            foreach (var project in inputs)
            {
                yield return project.Trim();
            }
        }
    }
}
