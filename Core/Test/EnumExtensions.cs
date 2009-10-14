using System;

using NUnit.Framework;
using Parent = SoftwareNinjas.Core;

namespace SoftwareNinjas.Core.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.EnumExtensions"/>
    /// </summary>
    [TestFixture]
    public class EnumExtensions
    {
        [Flags]
        private enum PrimaryColours
        {
            Red   = 1,
            Green = 2,
            Blue  = 4,
        }

        /// <summary>
        /// Tests <see cref="Parent.EnumExtensions.HasFlag{T}"/> with
        /// the typical case:  an enum decorated with the <see cref="FlagsAttribute"/>.
        /// </summary>
        [Test]
        public void HasFlag_Typical()
        {
            PrimaryColours cyan = PrimaryColours.Green | PrimaryColours.Blue;

            Assert.IsTrue(( cyan & PrimaryColours.Blue ) == PrimaryColours.Blue);
            Assert.IsTrue(cyan.HasFlag(PrimaryColours.Blue));

            Assert.IsTrue(( cyan & PrimaryColours.Green ) == PrimaryColours.Green);
            Assert.IsTrue(cyan.HasFlag(PrimaryColours.Green));
        }

        /// <summary>
        /// Tests <see cref="Parent.EnumExtensions.HasFlag{T}"/> with
        /// the base case that a single flag should be identified as "set".
        /// </summary>
        [Test]
        public void HasFlag_Identity()
        {
            Assert.IsTrue(PrimaryColours.Red.HasFlag(PrimaryColours.Red));
            Assert.IsTrue(PrimaryColours.Green.HasFlag(PrimaryColours.Green));
            Assert.IsTrue(PrimaryColours.Blue.HasFlag(PrimaryColours.Blue));
        }

        /// <summary>
        /// Tests <see cref="Parent.EnumExtensions.HasFlag{T}"/> with
        /// the base case that if a flag isn't set, it should not register.
        /// </summary>
        [Test]
        public void HasFlag_NotSet()
        {
            PrimaryColours cyan = PrimaryColours.Green | PrimaryColours.Blue;
            Assert.IsFalse(cyan.HasFlag(PrimaryColours.Red));
            Assert.IsFalse(PrimaryColours.Red.HasFlag(PrimaryColours.Blue));
        }

        /// <summary>
        /// Tests <see cref="Parent.EnumExtensions.HasFlag{T}"/> with
        /// the less likely (but still possible) case of integers.
        /// </summary>
        [Test]
        public void HasFlag_Integers()
        {
            int flags = 1 + 2;
            Assert.IsTrue(flags.HasFlag(1));
        }

        /// <summary>
        /// Tests <see cref="Parent.EnumExtensions.HasFlag{T}"/> with
        /// the even less likely (but still possible!) case of strings representing integers.
        /// </summary>
        [Test]
        public void HasFlag_Strings()
        {
            Assert.IsTrue("3".HasFlag("1"));
        }
    }
}
