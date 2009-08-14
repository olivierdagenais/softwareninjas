using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;
using SoftwareNinjas.Core.Test;
using Parent = SoftwareNinjas.NAnt.Tasks;

namespace SoftwareNinjas.NAnt.Tasks.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.CleanTask"/>.
    /// </summary>
    [TestFixture]
    public class CleanTask
    {
        private string _baseFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        /// <summary>
        /// Creates the folder structure used for the ExecuteTask tests.
        /// </summary>
        [SetUp]
        public void CreateStructure()
        {
            List<string> subFolders = new List<string>();
            subFolders.Add("One");
            subFolders.Add("One/bin/Debug");
            subFolders.Add("One/bin/Release");
            subFolders.Add("One/obj/Debug");
            subFolders.Add("One/obj/Release");

            subFolders.Add("Two");
            subFolders.Add("Two/bin/Debug");
            subFolders.Add("Two/bin/Release");
            subFolders.Add("Two/obj/Debug");
            subFolders.Add("Two/obj/Release");

            foreach (var sub in subFolders )
            {
                var dir = Path.Combine(_baseFolder, sub);
                Directory.CreateDirectory(dir);
                var file = Path.Combine(dir, "file.txt");
                File.WriteAllText(file, "text file for testing purposes");
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
        /// Tests <see cref="Parent.CleanTask.ExecuteTask()"/> with the default of deleting all configurations.
        /// </summary>
        [Test]
        public void ExecuteTask_Default()
        {
            Parent.CleanTask victim = new Parent.CleanTask(false);
            victim.BaseDirectory = new DirectoryInfo(_baseFolder);
            victim.Projects = "One,Two";
            victim.ExecuteForTest();

            List<string> mustNotExist = new List<string>();
            mustNotExist.Add("One/bin/Debug");
            mustNotExist.Add("One/bin/Release");
            mustNotExist.Add("One/obj/Debug");
            mustNotExist.Add("One/obj/Release");

            mustNotExist.Add("Two/bin/Debug");
            mustNotExist.Add("Two/bin/Release");
            mustNotExist.Add("Two/obj/Debug");
            mustNotExist.Add("Two/obj/Release");

            List<string> mustExist = new List<string>();
            mustExist.Add("One");
            mustExist.Add("Two");

            checkStructure(mustNotExist, mustExist);
        }

        /// <summary>
        /// Tests <see cref="Parent.CleanTask.ExecuteTask()"/> when specifying a configuration.
        /// </summary>
        [Test]
        public void ExecuteTask_WithConfiguration()
        {
            Parent.CleanTask victim = new Parent.CleanTask(false);
            victim.BaseDirectory = new DirectoryInfo(_baseFolder);
            victim.Projects = "One,Two";
            victim.Configuration = "Debug";
            victim.ExecuteForTest();

            List<string> mustNotExist = new List<string>();
            mustNotExist.Add("One/bin/Debug");
            mustNotExist.Add("One/obj/Debug");

            mustNotExist.Add("Two/bin/Debug");
            mustNotExist.Add("Two/obj/Debug");

            List<string> mustExist = new List<string>();
            mustExist.Add("One/bin/Release");
            mustExist.Add("One/obj/Release");

            mustExist.Add("Two/bin/Release");
            mustExist.Add("Two/obj/Release");

            checkStructure(mustNotExist, mustExist);
        }

        private void checkStructure(List<string> mustNotExist, List<string> mustExist)
        {
            foreach (var sub in mustNotExist)
            {
                Assert.IsFalse(Directory.Exists(Path.Combine(_baseFolder, sub)));
            }

            foreach (var sub in mustExist)
            {
                Assert.IsTrue(Directory.Exists(Path.Combine(_baseFolder, sub)));
            }
        }
    }
}
