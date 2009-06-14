using System;
using System.Collections.Generic;

namespace SoftwareNinjas.Core.Process
{
    /// <summary>
    /// A factory for <see cref="ICapturedProcess"/> implementations, thus allowing code that uses sub-processes to be
    /// tested without launching too many sub-processes and thus simplifying and speeding up unit tests.
    /// </summary>
    public interface ICapturedProcessFactory
    {
        /// <summary>
        /// Initializes an instance of an implementation of <see cref="ICapturedProcess"/> with the specified
        /// <paramref name="pathToExecutable"/> as well as optional <paramref name="arguments"/>,
        /// <paramref name="standardOutHandler"/> and <paramref name="standardErrorHandler"/>.
        /// </summary>
        /// 
        /// <param name="pathToExecutable">
        /// The path to the executable program from which a process will be created.
        /// </param>
        /// 
        /// <param name="arguments">
        /// The command-line arguments to supply to the program.  The arguments will be quoted automatically.
        /// </param>
        /// 
        /// <param name="standardOutHandler">
        /// An <see cref="Action{String}"/> to execute for each line of text sent to the standard output stream by the
        /// sub-process.
        /// </param>
        /// 
        /// <param name="standardErrorHandler">
        /// An <see cref="Action{String}"/> to execute for each line of text sent to the standard error stream by the
        /// sub-process.
        /// </param>
        /// 
        /// <returns>
        /// An instance of the associated <see cref="ICapturedProcess"/> implementation.
        /// </returns>
        ICapturedProcess Create(string pathToExecutable, IEnumerable<object> arguments,
                                Action<string> standardOutHandler, Action<string> standardErrorHandler);
    }
}
