using NAnt.Core;

namespace SoftwareNinjas.NAnt.Tasks
{
    /// <summary>
    /// A subclass of <see cref="Task"/> with a few extra features that make it easier to unit test the functionality.
    /// </summary>
    public abstract class TestableTask : Task
    {
        private readonly bool _logging;

        /// <summary>
        /// Creates a new instance with the specified <paramref name="logging"/>.
        /// </summary>
        /// 
        /// <param name="logging">
        /// Whether logging is enabled or not.
        /// </param>
        protected TestableTask(bool logging)
        {
            _logging = logging;
        }

        /// <summary>
        /// Logs a message with the given priority.
        /// </summary>
        /// 
        /// <param name="messageLevel">
        /// The message priority at which the specified message is to be logged.
        /// </param>
        /// 
        /// <param name="message">
        /// The message to be logged.
        /// </param>
        /// 
        /// <remarks>
        /// <para>
        /// The actual logging will only take place if logging was enabled during construction.
        /// </para>
        /// </remarks>
        /// 
        /// <seealso cref="Task.Log(Level,string)"/>
        public override void Log(Level messageLevel, string message)
        {
            if (_logging)
            {
                base.Log(messageLevel, message);
            }
        }

        /// <summary>
        /// Logs a formatted message with the given priority.
        /// </summary>
        /// 
        /// <param name="messageLevel">
        /// The message priority at which the specified message is to be logged.
        /// </param>
        /// 
        /// <param name="message">
        /// The message to log, containing zero or more format items.
        /// </param>
        /// 
        /// <param name="args">
        /// An <see cref="object" /> array containing zero or more objects to format.
        /// </param>
        /// 
        /// <remarks>
        /// <para>
        /// The actual logging will only take place if logging was enabled during construction.
        /// </para>
        /// </remarks>
        /// 
        /// <seealso cref="Task.Log(Level,string,object[])"/>
        public override void Log(Level messageLevel, string message, params object[] args)
        {
            if (_logging)
            {
                base.Log(messageLevel, message, args);
            }
        }

        /// <summary>
        /// Simply calls <see cref="Task.ExecuteTask()"/> without any prelude or fanfare.
        /// </summary>
        public void ExecuteForTest()
        {
            ExecuteTask();
        }

    }
}
