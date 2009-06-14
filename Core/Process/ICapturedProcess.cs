using System;

namespace SoftwareNinjas.Core.Process
{
    /// <summary>
    /// A façade to sub-process launching, for the special case of automatically (and safely) capturing
    /// and optionally relaying the contents of standard out and standard error.
    /// </summary>
    public interface ICapturedProcess : IDisposable
    {
        /// <summary>
        /// Starts the process and waits for it to exit while capturing and, as necessary, relaying the contents of
        /// standard out and standard error.  Returns the value that the associated process specified when it
        /// terminated.
        /// </summary>
        /// 
        /// <returns>
        /// The code that the associated process specified when it terminated.  The convention is to use <c>0</c>
        /// to indicate success and positive integers to indicate failure.
        /// </returns>
        int Run();
    }
}
