using System;
using System.Collections.Generic;

using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

using SoftwareNinjas.Core;

namespace SoftwareNinjas.MSBuild
{
    /// <summary>
    /// A pass-through "assembly" resolver that does not perform any dependency analysis but collects &lt;Assemblies&gt;
    /// and &lt;AssemblyFiles&gt; as &lt;ResolvedFiles&gt;.  More functionality may be added in the future.
    /// </summary>
    /// 
    /// <example>
    ///   <para>
    ///     The following demonstrates the use of the task.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    /// <ResolveJavaReferences
    ///     Assemblies="@(Reference)"
    ///     AssemblyFiles="@(_ResolvedProjectReferencePaths)"
    ///     >
    ///     <Output TaskParameter="ResolvedFiles" ItemName="ReferencePath"/>
    ///     <Output TaskParameter="ResolvedFiles" ItemName="_ResolveAssemblyReferenceResolvedFiles"/>
    /// </ResolveJavaReferences>
    ///     ]]>
    ///   </code>
    /// </example>
    public class ResolveJavaReferences : Task
    {
        /// <summary>
        /// References to binaries.
        /// </summary>
        public ITaskItem[] Assemblies
        {
            get;
            set;
        }

        /// <summary>
        /// References to project outputs.
        /// </summary>
        public ITaskItem[] AssemblyFiles
        {
            get;
            set;
        }

        /// <summary>
        /// All references needed to compile.
        /// </summary>
        [Output]
        public ITaskItem[] ResolvedFiles
        {
            get;
            set;
        }

        /// <summary>
        /// Performs the "analysis".
        /// </summary>
        /// 
        /// <returns>
        /// <see langword="true"/> if there were no errors; <see langword="false"/> otherwise.
        /// </returns>
        public override bool Execute()
        {
            var resolvedFiles = new List<ITaskItem>();

            Log.LogMessage(MessageImportance.Low, "Resolving dependencies...");
            foreach (ITaskItem item in EnumerableExtensions.Compose(Assemblies, AssemblyFiles))
            {
                // TODO: we may want to check if the associated file exists, first
                Log.LogMessage(MessageImportance.Low, "Adding {0}...", item.ItemSpec);
                resolvedFiles.Add(item);
            }

            ResolvedFiles = resolvedFiles.ToArray();
            return !Log.HasLoggedErrors;
        }
    }
}
