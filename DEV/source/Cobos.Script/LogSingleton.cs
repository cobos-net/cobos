
namespace Cobos.Script
{
    using NLog;

    public static class LogSingleton
    {
        /// <summary>
        /// The static log instance
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Get the log instance.
        /// </summary>
        public static Logger Instance
        {
            get
            {
                return logger;
            }
        }

    }
}
