using System;
using System.Collections.Generic;

namespace SoftwareNinjas.Core.Process
{
    /// <summary>
    /// A façade to sub-process launching, for the special case of automatically (and safely) capturing
    /// and optionally relaying the contents of standard out and standard error.
    /// </summary>
    public interface ICapturedProcess : IDisposable
    {
        /// <summary>
        /// The command-line parameters that will be processed into a single string to be provided to the program, if
        /// applicable.
        /// </summary>
        IEnumerable<object> Arguments
        {
            get;
        }

        /// <summary>
        /// The string of command-line parameters that will be provided to the program, if applicable.
        /// </summary>
        string ArgumentString
        {
            get;
        }

        /// <summary>
        /// The name and optional location of the program to use when creating the sub-process.
        /// </summary>
        string PathToExecutable
        {
            get;
        }

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
