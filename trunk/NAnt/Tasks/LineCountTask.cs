using System;
using System.IO;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace SoftwareNinjas.NAnt.Tasks
{
    /// <summary>
    /// Counts the number of lines in a text file.
    /// </summary>
    /// <example>
    ///   <para>
    ///   The example will count the number of lines in the <c>ReadMe.txt</c> file.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <lineCount
    ///     file="ReadMe.txt"
    ///     property="numberOfDifferences"
    /// />
    ///     ]]>
    ///   </code>
    /// </example>
    [TaskName("lineCount")]
    public class LineCountTask : TestableTask
    {
        /// <summary>
        /// Default constructor for NAnt itself.
        /// </summary>
        public LineCountTask()
            : base(true)
        {    
        }

        internal LineCountTask(bool logging) : base(logging)
        {
        }

        /// <summary>
        /// The path to the text file to read from.
        /// </summary>
        [TaskAttribute("file", Required = true)]
        public FileInfo SourceFile
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the property that receives number of lines.
        /// </summary>
        [TaskAttribute("property", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public string Property
        {
            get;
            set;
        }

        /// <summary>
        /// Runs the line count task.
        /// </summary>
        protected override void ExecuteTask()
        {
            var c = 0;
            Log(Level.Info, "Counting lines in {0}...", SourceFile.FullName);
            using (var sr = new StreamReader(SourceFile.FullName))
            {
                string line;
                while((line = sr.ReadLine()) != null)
                {
                    Log(Level.Debug, "{0}: {1}", c, line);
                    c++;
                }
            }
            Project.Properties[Property] = Convert.ToString(c, 10);
        }
    }
}
