using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using ICSharpCode.SharpZipLib.Zip;

namespace SoftwareNinjas.MSBuild
{
    /// <summary>
    /// Creates a Java Archive from class files and other miscellaneous files.
    /// </summary>
    /// 
    /// <example>
    ///   <para>
    ///     The following code demonstrates how the task it typically used in the <c>CopyFilesToOutputDirectory</c>
    ///     target.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <Jar
    ///     DestinationFolder="$(OutDir)"
    ///     SourceFiles="@(IntermediateAssembly)"
    ///     >
    ///     <Output TaskParameter="DestinationFiles" ItemName="MainAssembly"/>
    ///     <Output TaskParameter="DestinationFiles" ItemName="FileWrites"/>
    /// </Jar>
    ///     ]]>
    ///   </code>
    /// </example>
    public class Jar : Task
    {
        #region Properties
        /// <summary>
        /// The list of files that were created in <see cref="DestinationFolder"/>.
        /// </summary>
        [Output]
        public ITaskItem[] DestinationFiles
        {
            get;
            set;
        }

        /// <summary>
        /// The folder where the JAR will be created.
        /// </summary>
        public ITaskItem DestinationFolder
        {
            get;
            set;
        }

        /// <summary>
        /// The alleged "IntermediateAssembly" to copy; used to determine the source folder.
        /// </summary>
        public ITaskItem[] SourceFiles
        {
            get;
            set;
        }
        #endregion

        /// <summary>
        /// Performs the actual assembly of the JAR file
        /// </summary>
        /// 
        /// <returns>
        /// <see langword="true"/> if the assembly succeeded; <see langword="false"/> otherwise.
        /// </returns>
        public override bool Execute()
        {
            var sourceFile = SourceFiles[0].ItemSpec;
            var destinationFile = Path.GetFileNameWithoutExtension(sourceFile) + ".jar";
            var destinationFilePath = Path.Combine(DestinationFolder.ItemSpec, destinationFile);
            Log.LogMessage(MessageImportance.Low, "Creating JAR {0}...", destinationFilePath);
            ITaskItem destinationFileItem = new TaskItem(destinationFilePath);

            var sourceDirectory = Path.GetDirectoryName(SourceFiles[0].ItemSpec);
            var zip = new FastZip();
            zip.CreateZip(destinationFilePath, sourceDirectory, true, String.Empty);

            DestinationFiles = new ITaskItem[] { destinationFileItem };
            return !Log.HasLoggedErrors;
        }
    }
}
