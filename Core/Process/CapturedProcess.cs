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
        private readonly LegacyProcess _associatedProcess;
        private readonly Action<string> _stdOutHandler;
        private readonly Action<string> _stdErrHandler;
        private readonly IEnumerable<object> _arguments;
        private readonly string _pathToExecutable;

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
            _associatedProcess = new LegacyProcess();
            var startInfo = _associatedProcess.StartInfo;
            _pathToExecutable = pathToExecutable;
            startInfo.FileName = _pathToExecutable;

            _arguments = arguments;
            if (_arguments != null)
            {
                startInfo.Arguments = _arguments.QuoteForShell();
            }

            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;

            startInfo.RedirectStandardOutput = true;
            _associatedProcess.OutputDataReceived += OutputDataReceived;

            startInfo.RedirectStandardError = true;
            _associatedProcess.ErrorDataReceived += ErrorDataReceived;

            _stdOutHandler = standardOutHandler;
            _stdErrHandler = standardErrorHandler;
        }

        #region ICapturedProcess Members

        /// <summary>
        /// The command-line parameters that will be processed into a single string to be provided to the program, if
        /// applicable.
        /// </summary>
        public IEnumerable<object> Arguments
        {
            get
            {
                return _arguments;
            }
        }

        /// <summary>
        /// The string of command-line parameters that will be provided to the program, if applicable.
        /// </summary>
        public string ArgumentString
        {
            get
            {
                return _associatedProcess.StartInfo.Arguments;
            }
            set
            {
                _associatedProcess.StartInfo.Arguments = value;
            }
        }

        /// <summary>
        /// The name and optional location of the program to use when creating the sub-process.
        /// </summary>
        public string PathToExecutable
        {
            get
            {
                return _pathToExecutable;
            }
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
            _associatedProcess.Start();
            _associatedProcess.BeginOutputReadLine();
            _associatedProcess.BeginErrorReadLine();
            _associatedProcess.WaitForExit();
            int exitCode = _associatedProcess.ExitCode;
            return exitCode;
        }

        #endregion

        void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (_stdErrHandler != null && e.Data != null)
            {
                _stdErrHandler(e.Data);
            }
        }

        void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (_stdOutHandler != null && e.Data != null)
            {
                _stdOutHandler(e.Data);
            }
        }

        #region IDisposable Members
        void IDisposable.Dispose()
        {
            _associatedProcess.OutputDataReceived -= OutputDataReceived;
            _associatedProcess.ErrorDataReceived -= ErrorDataReceived;
            _associatedProcess.Dispose();
        }
        #endregion
    }
}
