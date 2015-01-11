using log4net;
using log4net.Config;

namespace Ajubaa.Common
{
    /// <summary>
    /// see info in the document "logging with log4net.xps" in this folder
    /// taken from http://shiman.wordpress.com/2008/07/09/how-to-log-in-c-net-with-log4net-a-tutorial/
    /// </summary>
    public class Logger
    {
        protected static ILog Log4NetLogger;
        public Logger(string logFilePath)
        {
            XmlConfigurator.Configure();
            GlobalContext.Properties["LogFilePath"] = logFilePath;
            Log4NetLogger = LogManager.GetLogger(typeof(Logger));
        }
        public void Log(string msg)
        {
            Log4NetLogger.Debug(msg);
        }
    }
}
