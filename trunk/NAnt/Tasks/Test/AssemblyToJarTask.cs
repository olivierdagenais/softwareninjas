using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Parent = SoftwareNinjas.NAnt.Tasks;
using SoftwareNinjas.Core.Process;
using SoftwareNinjas.Core.Process.Test;
using System.Text.RegularExpressions;

namespace SoftwareNinjas.NAnt.Tasks.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.AssemblyToJarTask"/>.
    /// </summary>
    [TestFixture]
    public class AssemblyToJarTask
    {
        /// <summary>
        /// Configures the task to simulate all processing and then asserts on properties of the command-line arguments.
        /// </summary>
        [Test]
        public void Execute_Simulated()
        {
            SimulatedCapturedProcess simulated = new SimulatedCapturedProcess(0, (string) null, (string) null);
            ICapturedProcessFactory factory = new SimulatedCapturedProcessFactory(simulated);
            Parent.AssemblyToJarTask victim = new Parent.AssemblyToJarTask(false, factory);
            victim.AssemblyPaths = "Dependencies/Organization.Product.dll";

            victim.ExecuteForTest();

            Assert.IsTrue(simulated.PathToExecutable.EndsWith("Converter.exe"));
            Regex re = new Regex(@"^
(""?.+Dependencies[/\\]Organization\.Product.dll""?)\s
(""?/lib:.+[/\\]v2\.0\.50727;.+Dependencies""?)\s
(""?/out:.+Dependencies[/\\]Organization\.Product.jar""?)
$", RegexOptions.IgnorePatternWhitespace);
            Assert.IsTrue(re.IsMatch(simulated.ArgumentString));
        }
    }
}
