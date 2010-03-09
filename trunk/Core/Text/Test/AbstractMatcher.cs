using System;
using System.Collections.Generic;

using NUnit.Framework;
using Parent = SoftwareNinjas.Core.Text;

namespace SoftwareNinjas.Core.Text.Test
{
    /// <summary>
    /// A class to test implementations of <see cref="IMatcher{T}"/>.
    /// </summary>
    public abstract class AbstractMatcher
    {
        #region Agriculture
        static readonly string[] Agriculture = new[]
        {
            "Animal", 
            "Barn",
            "BarnFactory",
            "Bovine",
            "CombineHarvester",
            "CombineHarvesterFactory",
            "ConventionalHarvester",
            "ConventionalHarvesterFactory",
            "Farm",
            "Farmer",
            "FarmFactory",
        };
        #endregion

        /// <summary>
        /// When implemented by subclasses, creates a new instance of the class under test, initialized with the
        /// provided <paramref name="items"/>.
        /// </summary>
        /// 
        /// <typeparam name="T">
        /// The type of <see cref="IMatcher{T}.Items"/> to search for by their <see cref="String"/> representation.
        /// </typeparam>
        /// 
        /// <param name="items">
        /// A sequence of instances of <typeparamref name="T"/> that will be searched for by their string
        /// representation.
        /// </param>
        /// 
        /// <returns>
        /// A new instance on each call.
        /// </returns>
        abstract protected IMatcher<T> CreateImplementation<T>(IEnumerable<T> items);

        /// <summary>
        /// Tests the <see cref="IMatcher{T}.MatchAnywhere(string)" /> method with
        /// the <see cref="Agriculture"/> set.
        /// </summary>
        [Test]
        public void MatchAnywhere_Agriculture()
        {
            Core.Test.EnumerableExtensions.EnumerateSame(
                new[] { "CombineHarvester", "CombineHarvesterFactory" },
                CreateImplementation(Agriculture).MatchAnywhere("Com"));

            Core.Test.EnumerableExtensions.EnumerateSame(
                new[] { "Bovine", "CombineHarvester", "CombineHarvesterFactory" },
                CreateImplementation(Agriculture).MatchAnywhere("in"));

            Core.Test.EnumerableExtensions.EnumerateSame(
                new[] { "BarnFactory", "CombineHarvesterFactory", "ConventionalHarvesterFactory", "FarmFactory" },
                CreateImplementation(Agriculture).MatchAnywhere("ory"));

            Core.Test.EnumerableExtensions.EnumerateSame(
                new string[] { },
                CreateImplementation(Agriculture).MatchAnywhere("chicken"));
        }

        /// <summary>
        /// Tests the <see cref="IMatcher{T}.MatchFromStartThenAnywhere(string)" /> method with
        /// the <see cref="Agriculture"/> set.
        /// </summary>
        [Test]
        public void MatchFromStartThenAnywhere_Agriculture()
        {
            Core.Test.EnumerableExtensions.EnumerateSame(
                new[] { "Farm", "Farmer", "FarmFactory", "BarnFactory", 
                        "CombineHarvesterFactory", "ConventionalHarvesterFactory" },
                CreateImplementation(Agriculture).MatchFromStartThenAnywhere("F"));

            Core.Test.EnumerableExtensions.EnumerateSame(
                new[] { "Bovine", "CombineHarvester", "CombineHarvesterFactory" },
                CreateImplementation(Agriculture).MatchFromStartThenAnywhere("in"));

            Core.Test.EnumerableExtensions.EnumerateSame(
                new[] { "BarnFactory", "CombineHarvesterFactory", "ConventionalHarvesterFactory", "FarmFactory" },
                CreateImplementation(Agriculture).MatchFromStartThenAnywhere("ory"));

            Core.Test.EnumerableExtensions.EnumerateSame(
                new string[] { },
                CreateImplementation(Agriculture).MatchFromStartThenAnywhere("chicken"));
        }

        /// <summary>
        /// Tests the <see cref="IMatcher{T}.MatchFromStart(string)" /> method with
        /// the <see cref="Agriculture"/> set.
        /// </summary>
        [Test]
        public void MatchFromStart_Agriculture()
        {
            Core.Test.EnumerableExtensions.EnumerateSame(
                new[] { "Farm", "Farmer", "FarmFactory" },
                CreateImplementation(Agriculture).MatchFromStart("F"));

            Core.Test.EnumerableExtensions.EnumerateSame(
                new[] { "CombineHarvester", "CombineHarvesterFactory" },
                CreateImplementation(Agriculture).MatchFromStart("Com"));

            Core.Test.EnumerableExtensions.EnumerateSame(
                new string[] { },
                CreateImplementation(Agriculture).MatchFromStart("Factory"));
        }

        /// <summary>
        /// Tests the <see cref="IMatcher{T}.MatchCamelCasingHumps(string)" /> method with
        /// the <see cref="Agriculture"/> set.
        /// </summary>
        [Test]
        public void MatchCamelCasingHumps_Agriculture()
        {
            Core.Test.EnumerableExtensions.EnumerateSame(
                new[] { "CombineHarvester", "CombineHarvesterFactory" },
                CreateImplementation(Agriculture).MatchCamelCasingHumps("Com"));

            Core.Test.EnumerableExtensions.EnumerateSame(
                new[] { "CombineHarvesterFactory", "ConventionalHarvesterFactory" },
                CreateImplementation(Agriculture).MatchCamelCasingHumps("CHF"));

            Core.Test.EnumerableExtensions.EnumerateSame(
                new[] { "CombineHarvesterFactory" },
                CreateImplementation(Agriculture).MatchCamelCasingHumps("ComHF"));

            Core.Test.EnumerableExtensions.EnumerateSame(
                new string[] { },
                CreateImplementation(Agriculture).MatchCamelCasingHumps("chicken"));
        }
    }
}
