using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareNinjas.Core.Process.Test
{
    /// <summary>
    /// An implementation of <see cref="ICapturedProcessFactory"/> that configures a single instance of
    /// <see cref="SimulatedCapturedProcess"/> for testing purposes.  This class is not thread-safe.
    /// </summary>
    public class SimulatedCapturedProcessFactory : ICapturedProcessFactory
    {
        private SimulatedCapturedProcess _instance;

        /// <summary>
        /// Initializes an instance of the <see cref="SimulatedCapturedProcessFactory"/> with the specified
        /// <paramref name="instance"/>.
        /// </summary>
        /// 
        /// <param name="instance">
        /// A single instance of <see cref="SimulatedCapturedProcess"/> that will be configured with the standard
        /// handlers on each call to
        /// <see cref="ICapturedProcessFactory.Create(string, IEnumerable{object}, Action{string}, Action{string})"/>.
        /// </param>
        public SimulatedCapturedProcessFactory(SimulatedCapturedProcess instance)
        {
            _instance = instance;
        }

        #region ICapturedProcessFactory Members

        ICapturedProcess ICapturedProcessFactory.Create(string pathToExecutable, 
            IEnumerable<object> arguments, Action<string> standardOutHandler, Action<string> standardErrorHandler)
        {
            _instance.StandardOutHandler = standardOutHandler;
            _instance.StandardErrorHandler = standardErrorHandler;
            return _instance;
        }

        #endregion
    }
}
