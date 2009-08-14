using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareNinjas.Core.Process.Test
{
    /// <summary>
    /// An implementation of <see cref="ICapturedProcess"/> that simulates the execution of a sub-process and
    /// feeds to the standard out and standard error handlers pre-arranged strings.
    /// </summary>
    public class SimulatedCapturedProcess : ICapturedProcess
    {
        private static readonly string[] emptyStringArray = new string[] { };
        private static readonly string[] splittingStrings = new string[] { Environment.NewLine };
        private readonly IEnumerable<string> _stdOut;
        private readonly IEnumerable<string> _stdErr;
        private readonly int _exitCode;

        /// <summary>
        /// Initializes a new instance of a <see cref="SimulatedCapturedProcess"/> with the specified
        /// <paramref name="exitCode"/> and optional <paramref name="stdOut"/> and <paramref name="stdErr"/>.
        /// </summary>
        /// 
        /// <param name="exitCode">
        /// The exit code that <see cref="ICapturedProcess.Run()"/> will return.
        /// </param>
        /// 
        /// <param name="stdOut">
        /// An optional multi-line string that will be fed to the standard out handler one line at a time.
        /// </param>
        /// 
        /// <param name="stdErr">
        /// An optional multi-line string that will be fed to the standard error handler one line at a time.
        /// </param>
        public SimulatedCapturedProcess(int exitCode, string stdOut, string stdErr)
        {
            _exitCode = exitCode;

            _stdOut = ToLines(stdOut);
            _stdErr = ToLines(stdErr);
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="SimulatedCapturedProcess"/> with the specified
        /// <paramref name="exitCode"/> and optional <paramref name="stdOut"/> and <paramref name="stdErr"/>.
        /// </summary>
        /// 
        /// <param name="exitCode">
        /// The exit code that <see cref="ICapturedProcess.Run()"/> will return.
        /// </param>
        /// 
        /// <param name="stdOut">
        /// An optional <see cref="IEnumerable{T}"/> of <see cref="String"/> that will be fed to the standard out
        /// handler one at a time.
        /// </param>
        /// 
        /// <param name="stdErr">
        /// An optional <see cref="IEnumerable{T}"/> of <see cref="String"/> that will be fed to the standard error
        /// handler one at a time.
        /// </param>
        public SimulatedCapturedProcess(int exitCode, IEnumerable<string> stdOut, IEnumerable<string> stdErr)
        {
            _exitCode = exitCode;

            _stdOut = null == stdOut ? emptyStringArray : stdOut;
            _stdErr = null == stdErr ? emptyStringArray : stdErr;
        }

        internal static IEnumerable<string> ToLines(string input)
        {
            if (null == input)
            {
                return emptyStringArray;
            }
            else
            {
                return input.Split(splittingStrings, StringSplitOptions.None);
            }
        }

        /// <summary>
        /// The configured <see cref="Action{T}"/> to invoke for each line of standard out.
        /// </summary>
        public Action<string> StandardOutHandler
        {
            get;
            set;
        }

        /// <summary>
        /// The configured <see cref="Action{T}"/> to invoke for each line of standard error.
        /// </summary>
        public Action<string> StandardErrorHandler
        {
            get;
            set;
        }

        private IEnumerable<object> _arguments = null;
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
            set
            {
                _arguments = value;
                if (null == _arguments)
                {
                    _argumentString = null;
                }
                else
                {
                    _argumentString = _arguments.QuoteForShell();
                }
            }
        }

        private string _argumentString = null;
        /// <summary>
        /// The string of command-line parameters that will be provided to the program, if applicable.
        /// </summary>
        public string ArgumentString
        {
            get
            {
                return _argumentString;
            }
            set
            {
                _argumentString = value;
            }
        }

        /// <summary>
        /// The name and optional location of the program to use when creating the sub-process.
        /// </summary>
        public string PathToExecutable
        {
            get;
            set;
        }

        #region ICapturedProcess Members

        int ICapturedProcess.Run()
        {
            foreach (string line in _stdOut)
            {
                if (StandardOutHandler != null)
                {
                    StandardOutHandler(line);
                }
            }
            foreach (string line in _stdErr)
            {
                if (StandardErrorHandler != null)
                {
                    StandardErrorHandler(line);
                }
            }
            return _exitCode;
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            // do nothing
        }

        #endregion
    }
}
