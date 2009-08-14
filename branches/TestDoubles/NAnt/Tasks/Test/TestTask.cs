using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using Parent = SoftwareNinjas.NAnt.Tasks;

namespace SoftwareNinjas.NAnt.Tasks.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.TestTask"/>.
    /// </summary>
    [TestFixture]
    public class TestTask
    {
        /// <summary>
        /// Tests the <see cref="Parent.TestTask.IsTestClassFile(string)"/> method.
        /// </summary>
        [Test]
        public void IsTestClassFile()
        {
            Assert.IsTrue(Parent.TestTask.IsTestClassFile("GenerateTestsActionTest.class"));

            Assert.IsFalse(Parent.TestTask.IsTestClassFile(""));
            Assert.IsFalse(Parent.TestTask.IsTestClassFile("GenerateTestsActionTest.java"));
            Assert.IsFalse(Parent.TestTask.IsTestClassFile("TestingClass.class"));
            Assert.IsFalse(Parent.TestTask.IsTestClassFile("AbstractTestCase.class"));
        }

        /// <summary>
        /// Tests the <see cref="Parent.TestTask.ConvertClassFilenameToClassName(string)"/> method.
        /// </summary>
        [Test]
        public void ConvertClassFilenameToClassName()
        {
            Assert.AreEqual("Test", Parent.TestTask.ConvertClassFilenameToClassName("Test.class"));
            Assert.AreEqual(
                "org.dyndns.opendemogroup.todd.ui.actions.GenerateTestsActionTest", 
                Parent.TestTask.ConvertClassFilenameToClassName(
                "org/dyndns/opendemogroup/todd/ui/actions/GenerateTestsActionTest.class"));
            Assert.AreEqual("", Parent.TestTask.ConvertClassFilenameToClassName(""));
        }
    }
}
