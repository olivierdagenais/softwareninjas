using System;

using Parent = SoftwareNinjas.Core.Process;
using NUnit.Framework;
using System.Collections.Generic;

namespace SoftwareNinjas.Core.Process.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.CapturedProcess"/>.
    /// </summary>
    [TestFixture]
    public class CapturedProcess
    {
        const string stdOut = "stdOut is here";
        const string stdErr = "stdErr is here";
        const string pathToShell = "cmd.exe";
        const string pathToEcho = "..\\..\\..\\Tools\\echo.exe";

        /// <summary>
        /// Tests <see cref="Parent.CapturedProcess.Run()"/> with a command that will output to both streams.
        /// </summary>
        [Test]
        public void Run_BothStreams()
        {
            Run(true, true, pathToShell, "/c", pathToEcho, stdOut, "&&", pathToEcho, stdErr, ">&2");
        }

        /// <summary>
        /// Tests <see cref="Parent.CapturedProcess.Run()"/> with a command that will output to only standard out.
        /// </summary>
        [Test]
        public void Run_OnlyStandardOut()
        {
            Run(true, false, pathToShell, "/c", pathToEcho, stdOut);
        }

        /// <summary>
        /// Tests <see cref="Parent.CapturedProcess.Run()"/> with a command that will output to only standard error.
        /// </summary>
        [Test]
        public void Run_OnlyStandardError()
        {
            Run(false, true, pathToShell, "/c", pathToEcho, stdErr, ">&2");
        }

        private static void Run(bool hasStdOut, bool hasStdErr, string pathToProgram, params object[] parameters)
        {
            List<string> actualOut = new List<string>();
            List<string> actualErr = new List<string>();
            Action<string> outProcessor = (outLine) =>
            {
                actualOut.Add(outLine);
            };
            Action<string> errProcessor = (errLine) =>
            {
                actualErr.Add(errLine);
            };
            int actualExitCode;
            using (var cp = new Parent.CapturedProcess(pathToProgram, parameters, outProcessor, errProcessor))
            {
                actualExitCode = cp.Run();
            }
            Assert.AreEqual(0, actualExitCode);

            if (hasStdOut)
            {
                Assert.AreEqual(1, actualOut.Count);
                Assert.AreEqual(stdOut, actualOut[0]);
            }
            else
            {
                Assert.AreEqual(0, actualOut.Count);
            }

            if (hasStdErr)
            {
                Assert.AreEqual(1, actualErr.Count);
                Assert.AreEqual(stdErr, actualErr[0]);
            }
            else
            {
                Assert.AreEqual(0, actualErr.Count);
            }
        }
    }
}
