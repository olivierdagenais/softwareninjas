using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;
using SoftwareNinjas.Core.Test;
using Parent = SoftwareNinjas.NAnt.Tasks;

namespace SoftwareNinjas.NAnt.Tasks.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.AbstractProjectTask"/>.
    /// </summary>
    [TestFixture]
    public class AbstractProjectTask
    {

        /// <summary>
        /// Tests <see cref="Parent.AbstractProjectTask.ParseProjects(string)"/>.
        /// </summary>
        [Test]
        public void ParseProjects_WithSpaces()
        {
            EnumerableExtensions.EnumerateSame(
                new string[] {"One", "Two"},
                Parent.AbstractProjectTask.ParseProjects("One, Two") );
        }
    }
}
