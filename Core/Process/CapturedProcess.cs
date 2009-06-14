using System;
using System.Collections.Generic;
using System.Diagnostics;
using LegacyProcess = System.Diagnostics.Process;
using System.IO;

using SoftwareNinjas.Core;

namespace SoftwareNinjas.Core.Process
{
    /// <summary>
    /// A façade to the <see cref="LegacyProcess"/> class for the special case of automatically (and safely) capturing
    /// and optionally relaying the contents of standard out and standard error.
    /// </summary>
    public class CapturedProcess : ICapturedProcess
    {
        private readonly LegacyProcess m_associatedProcess;
        private readonly Action<string> m_stdOutHandler;
        private readonly Action<string> m_stdErrHandler;

        /// <summary>
        /// Creates an instance of the <see cref="CapturedProcess"/> class with the specified
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
        public CapturedProcess(string pathToExecutable, IEnumerable<object> arguments,
                                Action<string> standardOutHandler, Action<string> standardErrorHandler)
        {
            m_associatedProcess = new LegacyProcess();
            var startInfo = m_associatedProcess.StartInfo;
            startInfo.FileName = pathToExecutable;
            if (arguments != null)
            {
                startInfo.Arguments = arguments.QuoteForShell();
            }

            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;

            startInfo.RedirectStandardOutput = true;
            m_associatedProcess.OutputDataReceived += OutputDataReceived;

            startInfo.RedirectStandardError = true;
            m_associatedProcess.ErrorDataReceived += ErrorDataReceived;

            m_stdOutHandler = standardOutHandler;
            m_stdErrHandler = standardErrorHandler;
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
        public int Run()
        {
            m_associatedProcess.Start();
            m_associatedProcess.BeginOutputReadLine();
            m_associatedProcess.BeginErrorReadLine();
            m_associatedProcess.WaitForExit();
            int exitCode = m_associatedProcess.ExitCode;
            return exitCode;
        }

        void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (m_stdErrHandler != null && e.Data != null)
            {
                m_stdErrHandler(e.Data);
            }
        }

        void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (m_stdOutHandler != null && e.Data != null)
            {
                m_stdOutHandler(e.Data);
            }
        }

        #region IDisposable Members
        void IDisposable.Dispose()
        {
            m_associatedProcess.OutputDataReceived -= OutputDataReceived;
            m_associatedProcess.ErrorDataReceived -= ErrorDataReceived;
            m_associatedProcess.Dispose();
        }
        #endregion
    }
}
