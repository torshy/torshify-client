using System;

using log4net;

using Microsoft.Practices.Prism.Logging;

namespace Torshify.Client.Log
{
    public class Log4NetFacade : ILoggerFacade
    {
        #region Fields

        private static readonly ILog _log = LogManager.GetLogger("Torshify");

        #endregion Fields

        #region Methods

        public void Log(string message, Category category, Priority priority)
        {
            message = string.Format("[{0}] {1}", priority, message);

            switch (category)
            {
                case Category.Debug:
                    _log.Debug(message);
                    break;
                case Category.Exception:
                    LogError(message, priority);
                    break;
                case Category.Info:
                    _log.Info(message);
                    break;
                case Category.Warn:
                    _log.Warn(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("category");
            }
        }

        private void LogError(string message, Priority priority)
        {
            switch (priority)
            {
                case Priority.High:
                    _log.Fatal(message);
                    break;
                default:
                    _log.Error(message);
                    break;
            }
        }

        #endregion Methods
    }
}