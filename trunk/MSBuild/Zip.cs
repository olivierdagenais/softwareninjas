using System.IO;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using ICSharpCode.SharpZipLib.Zip;

namespace SoftwareNinjas.MSBuild
{
    /// <summary>
    /// Creates a ZIP archive from the specified files.
    /// </summary>
    /// 
    /// <example>
    ///   <para>
    ///     The following code demonstrates how the task it typically used.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <ItemGroup>
    ///     <FilesToZip Include="program.exe; dependency.dll" />
    ///     <FilesToZip Include="**/*.html" />
    /// </ItemGroup>
    /// <Zip ZipFile="example.zip" SourceFiles="@(FilesToZip)" />
    ///     ]]>
    ///   </code>
    /// </example>
    public class Zip : Task
    {
        const int CopyBufferSize = 4096;

        #region Properties
        /// <summary>
        /// The path to the ZIP archive to be created.
        /// </summary>
        public ITaskItem ZipFile
        {
            get;
            set;
        }

        /// <summary>
        /// The files to add to the ZIP archive.
        /// </summary>
        public ITaskItem[] SourceFiles
        {
            get;
            set;
        }

        /// <summary>
        /// <see langword="true"/> if the source paths are to be discarded;
        /// <see langword="false"/> otherwise.
        /// </summary>
        public bool Flatten
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
            Log.LogMessage(MessageImportance.Low, "Creating ZIP {0}...", ZipFile.ItemSpec);

            using (var outputStream =
                new FileStream(ZipFile.ItemSpec, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                using (var zipStream = new ZipOutputStream (outputStream))
                {
                    zipStream.SetLevel (9);
                    foreach (var sourceItem in SourceFiles)
                    {
                        var itemInfo = new FileInfo (sourceItem.ItemSpec);
                        var entryName = Flatten ? itemInfo.Name : ZipEntry.CleanName (sourceItem.ItemSpec);
                        var entry = new ZipEntry (entryName)
                        {
                            DateTime = itemInfo.LastWriteTime,
                            Size = itemInfo.Length,
                        };
                        zipStream.PutNextEntry (entry);
                        using (var sourceStream = File.OpenRead(sourceItem.ItemSpec))
                        {
                            var buffer = new byte[CopyBufferSize];
                            var bytesRead = sourceStream.Read (buffer, 0, CopyBufferSize);
                            while (bytesRead > 0)
                            {
                                zipStream.Write (buffer, 0, bytesRead);
                                bytesRead = sourceStream.Read (buffer, 0, CopyBufferSize);
                            }
                        }
                        zipStream.CloseEntry ();
                    }
                }
            }

            return !Log.HasLoggedErrors;
        }
    }
}
