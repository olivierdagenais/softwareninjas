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
        public AbstractProjectTask(bool logging) : base(logging)
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
        /// A comma-separated list of strings representing sub-folder names where processing is to be performed.
        /// </summary>
        [TaskAttribute("projects", Required = true)]
        public string Projects
        {
            get;
            set;
        }

        /// <summary>
        /// Allows sub-classes to process projects one by one.
        /// </summary>
        /// 
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of <see cref="String"/> representing individual projects.
        /// </returns>
        public IEnumerable<string> EnumerateProjects()
        {
            return ParseProjects(Projects);
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
