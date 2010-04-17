using System.IO;
using System.Xml;

using Parent = SoftwareNinjas.NAnt.Tasks;
using NUnit.Framework;
using SoftwareNinjas.Core.Test;

namespace SoftwareNinjas.NAnt.Tasks.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.ProjectLoader"/>.
    /// </summary>
    [TestFixture]
    public class ProjectLoader
    {
        private static XmlDocument LoadXml(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }

        /// <summary>
        /// Tests <see cref="Parent.ProjectLoader.LoadLanguage(XmlDocument)"/> with the default, a C# project.
        /// </summary>
        [Test]
        public void LoadLanguage_CSharp()
        {
            var xml = @"
<Project ToolsVersion=""3.5"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>";
            var actualLanguage = Parent.ProjectLoader.LoadLanguage(LoadXml(xml));
            Assert.AreEqual(SupportedLanguage.CSharp, actualLanguage);
        }

        /// <summary>
        /// Tests <see cref="Parent.ProjectLoader.LoadLanguage(XmlDocument)"/> with the exception, a Java project.
        /// </summary>
        [Test]
        public void LoadLanguage_Java()
        {
            var xml = @"
<Project ToolsVersion=""3.5"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""..\Tools\SoftwareNinjas.Java.targets"" />
</Project>";
            var actualLanguage = Parent.ProjectLoader.LoadLanguage(LoadXml(xml));
            Assert.AreEqual(SupportedLanguage.Java, actualLanguage);
        }

        /// <summary>
        /// Tests <see cref="Parent.ProjectLoader.LoadReferences(XmlDocument, string)"/>.
        /// </summary>
        [Test]
        public void LoadReferences()
        {
            var xml = @"
<Project ToolsVersion=""3.5"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <ItemGroup>
    <Reference Include=""nunit.framework, Version=2.2.8.0"">
      <SpecificVersion>False</SpecificVersion>
      <HintPath>..\Tools\nant\bin\lib\net\2.0\nunit.framework.dll</HintPath>
    </Reference>
    <Reference Include=""System"" />
  </ItemGroup>
</Project>";
            var actualReferences = Parent.ProjectLoader.LoadReferences(LoadXml(xml), "base");
            var expectedReferences = new FileInfo[]
            {
                new FileInfo( @"base\..\Tools\nant\bin\lib\net\2.0\nunit.framework.dll")
            };
            EnumerableExtensions.EnumerateSame(expectedReferences, actualReferences, fi => fi.ToString());
        }

        /// <summary>
        /// Tests <see cref="Parent.ProjectLoader.LoadOutputName(XmlDocument, SupportedLanguage)"/> for a class library
        /// C# project.
        /// </summary>
        [Test]
        public void LoadOutputName_CSharpLibrary()
        {
            var xml = @"
<Project ToolsVersion=""3.5"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <OutputType>Library</OutputType>
    <AssemblyName>SoftwareNinjas.TestOriented.Core</AssemblyName>
  </PropertyGroup>
</Project>";

            var actualOutputName = Parent.ProjectLoader.LoadOutputName(LoadXml(xml), SupportedLanguage.CSharp);
            Assert.AreEqual("SoftwareNinjas.TestOriented.Core.dll", actualOutputName);
        }

        /// <summary>
        /// Tests <see cref="Parent.ProjectLoader.LoadOutputName(XmlDocument, SupportedLanguage)"/> for an executable
        /// C# project.
        /// </summary>
        [Test]
        public void LoadOutputName_CSharpExecutable()
        {
            var xml = @"
<Project ToolsVersion=""3.5"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <AssemblyName>SoftwareNinjas.BranchReview.Console</AssemblyName>
  </PropertyGroup>
</Project>";

            var actualOutputName = Parent.ProjectLoader.LoadOutputName(LoadXml(xml), SupportedLanguage.CSharp);
            Assert.AreEqual("SoftwareNinjas.BranchReview.Console.exe", actualOutputName);
        }

        /// <summary>
        /// Tests <see cref="Parent.ProjectLoader.LoadOutputName(XmlDocument, SupportedLanguage)"/> for a Java project.
        /// </summary>
        [Test]
        public void LoadOutputName_JavaLibrary()
        {
            var xml = @"
<Project ToolsVersion=""3.5"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <OutputType>Library</OutputType>
    <AssemblyName>SoftwareNinjas.TestOriented.Eclipse</AssemblyName>
  </PropertyGroup>
</Project>";

            var actualOutputName = Parent.ProjectLoader.LoadOutputName(LoadXml(xml), SupportedLanguage.Java);
            Assert.AreEqual("SoftwareNinjas.TestOriented.Eclipse.jar", actualOutputName);
        }
    }
}
