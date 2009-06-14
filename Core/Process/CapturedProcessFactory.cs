using System;
using System.Collections.Generic;

namespace SoftwareNinjas.Core.Process
{
    /// <summary>
    /// An implementation of <see cref="ICapturedProcessFactory"/> that creates instances of
    /// <see cref="CapturedProcess"/>.
    /// </summary>
    public class CapturedProcessFactory : ICapturedProcessFactory
    {
        #region ICapturedProcessFactory Members

        ICapturedProcess ICapturedProcessFactory.Create(string pathToExecutable, IEnumerable<object> arguments, 
            Action<string> standardOutHandler, Action<string> standardErrorHandler)
        {
            return new CapturedProcess(pathToExecutable, arguments, standardOutHandler, standardErrorHandler);
        }

        #endregion
    }
}
