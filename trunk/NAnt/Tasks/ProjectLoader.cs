using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace SoftwareNinjas.NAnt.Tasks
{
    internal class ProjectLoader
    {
        /// <summary>
        /// Creates a new instance of <see cref="Project"/> initialized from the <c>.csproj</c> file named
        /// <paramref name="projectName"/> in the <paramref name="projectName"/> sub-folder under
        /// <paramref name="baseDirectory"/>.
        /// </summary>
        /// 
        /// <param name="baseDirectory">
        /// The solution folder, under which the project sub-folder will be found.
        /// </param>
        /// 
        /// <param name="projectName">
        /// The name of the sub-folder and the name of the <c>.csproj</c> file herein.
        /// </param>
        /// <returns></returns>
        internal Project Create(string baseDirectory, string projectName)
        {
            var folder = Path.Combine(baseDirectory, projectName);
            var projectFile = Path.Combine(folder, projectName + ".csproj");
            var doc = new XmlDocument();
            doc.Load(projectFile);

            var language = LoadLanguage(doc);
            var references = LoadReferences(doc, folder);
            var outputName = LoadOutputName(doc, language);
            Project result = new Project(projectName, folder, language, references, outputName);
            return result;
        }

        internal static XmlNamespaceManager CreateNamespaceManager(XmlDocument doc)
        {
            XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
            manager.AddNamespace("msbuild", "http://schemas.microsoft.com/developer/msbuild/2003");
            return manager;
        }

        internal static SupportedLanguage LoadLanguage(XmlDocument doc)
        {
            var manager = CreateNamespaceManager(doc);
            SupportedLanguage language = SupportedLanguage.CSharp;
            XmlAttribute projectAttribute = 
                doc.SelectSingleNode("msbuild:Project/msbuild:Import/@Project", manager) as XmlAttribute;
            if (projectAttribute != null)
            {
                var projectImport = projectAttribute.Value;
                if (projectImport.Contains("SoftwareNinjas.Java.targets"))
                {
                    language = SupportedLanguage.Java;
                }
            }
            return language;
        }

        internal static IList<FileInfo> LoadReferences(XmlDocument doc, string baseProjectPath)
        {
            var manager = CreateNamespaceManager(doc);
            var references = new List<FileInfo>();
            var referenceHintPathNodes =
                doc.SelectNodes("msbuild:Project/msbuild:ItemGroup/msbuild:Reference/msbuild:HintPath", manager);
            foreach (XmlNode referenceHintPathNode in referenceHintPathNodes)
            {
                var hintPath = referenceHintPathNode.InnerText;
                var resolvedPath = Path.Combine(baseProjectPath, hintPath);
                var reference = new FileInfo(resolvedPath);
                references.Add(reference);
            }
            return references;
        }

        internal static string LoadOutputName(XmlDocument doc, SupportedLanguage language)
        {
            var manager = CreateNamespaceManager(doc);
            string result = null;
            var assemblyNameElement = doc.SelectSingleNode(
                "msbuild:Project/msbuild:PropertyGroup/msbuild:AssemblyName", manager) as XmlElement;
            if (assemblyNameElement != null)
            {
                var fileName = assemblyNameElement.InnerText;
                var extension = ".jar";
                if (language != SupportedLanguage.Java)
                {
                    extension = ".exe";
                    var outputTypeElement = doc.SelectSingleNode(
                        "msbuild:Project/msbuild:PropertyGroup/msbuild:OutputType", manager) as XmlElement;
                    if (outputTypeElement != null)
                    {
                        var outputType = outputTypeElement.InnerText;
                        if (outputType == "Library")
                        {
                            extension = ".dll";
                        }
                    }
                }
                result = fileName + extension;
            }
            return result;
        }
    }
}
