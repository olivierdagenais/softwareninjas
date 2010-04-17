using System;
using System.IO;
using System.Xml;

using Parent = SoftwareNinjas.NAnt.Tasks;
using NUnit.Framework;
using SoftwareNinjas.Core;

namespace SoftwareNinjas.NAnt.Tasks.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.CustomizeAssemblyTask"/>.
    /// </summary>
    [TestFixture]
    public class CustomizeAssemblyTask
    {
        private const string BaselineFileContents = "empty file as a baseline";
        private static readonly string[] Projects = new[] { "One", "Two" };

        private readonly string _baseFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        /// <summary>
        /// Creates the folder structure used for the ExecuteTask tests.
        /// </summary>
        [SetUp]
        public void CreateStructure()
        {
            foreach (var project in Projects)
            {
                var dir = Path.Combine(_baseFolder, project + "/Properties");
                Directory.CreateDirectory(dir);
                var file = Path.Combine(dir, "CustomInfo.cs");
                File.WriteAllText(file, BaselineFileContents);
            }
        }

        /// <summary>
        /// The complement to <see cref="CreateStructure()"/>, deletes the base folder.
        /// </summary>
        [TearDown]
        public void WipeStructure()
        {
            Directory.Delete(_baseFolder, true);
        }

        /// <summary>
        /// Tests the <see cref="Parent.TestableTask.ExecuteForTest()"/> method with typical numbers and an
        /// unregistered user.
        /// </summary>
        [Test]
        public void ExecuteForTest_Typical()
        {
            CheckExecute(1, 2, 3, "Unregistered User", "unregistered.user@example.com");
            CheckExecute(1, 0, 0, "Unregistered User", "unregistered.user@example.com");
        }

        /// <summary>
        /// Tests the <see cref="Parent.TestableTask.ExecuteForTest()"/> method with the special case of a build number
        /// of -1, which should be the case when nothing has been configured, such as running on a developer's computer.
        /// </summary>
        [Test]
        public void ExecuteForTest_DoNothing()
        {
            CheckExecute(1, 0, -1, "Unregistered User", "unregistered.user@example.com");
        }

        /// <summary>
        /// Tests the <see cref="Parent.TestableTask.ExecuteForTest()"/> method with some missing data.
        /// </summary>
        [Test]
        public void ExecuteForTest_EdgeCases()
        {
            CheckExecute(1, 2, 3, "Unregistered User", null);
            CheckExecute(1, 2, 3, null, "unregistered.user@example.com");
            CheckExecute(1, 2, 3, null, null);
            CheckExecute(0, 0, 0, "", "");
        }

        private void CheckExecute(int major, int minor, int buildNumber,
            string registeredUserDisplayName, string registeredUserEmailAddress)
        {
            var versionXmlString = String.Format(@"<version major=""{0}"" minor=""{1}"" />", major, minor);
            var doc = new XmlDocument();
            doc.LoadXml(versionXmlString);

            var task = new Parent.CustomizeAssemblyTask(false, buildNumber, 
                registeredUserDisplayName, registeredUserEmailAddress);
            task.Version = doc;
            task.Projects = Projects.Join(",");
            task.BaseDirectory = new DirectoryInfo(_baseFolder);
            task.ExecuteForTest();

            var versionString = String.Format(@"""{0}.{1}.{2}""", major, minor, buildNumber);
            var registeredUserString = String.Format(
                @"[assembly: RegisteredUser ( ""{0}"", ""{1}"" )]", 
                registeredUserDisplayName, registeredUserEmailAddress);
            foreach (var project in Projects)
            {
                var file = Path.Combine(_baseFolder, project + "/Properties/CustomInfo.cs");
                Assert.IsTrue(File.Exists(file));
                var contents = File.ReadAllText(file);
                if (-1 == buildNumber)
                {
                    Assert.IsTrue(contents.Contains(BaselineFileContents));
                }
                else
                {
                    Assert.IsFalse(contents.Contains(BaselineFileContents));

                    if (String.IsNullOrEmpty(registeredUserDisplayName) 
                        || String.IsNullOrEmpty(registeredUserEmailAddress))
                    {
                        Assert.IsFalse(contents.Contains("RegisteredUser"));
                    }
                    else
                    {
                        Assert.IsTrue(contents.Contains(registeredUserString));
                    }

                    Assert.IsTrue(contents.Contains(versionString));
                }
            }
        }
    }
}
