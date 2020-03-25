using log4net;
using log4net.Config;
using System.Diagnostics;

[assembly: XmlConfigurator(Watch = true)]
namespace Infonet.CStoreCommander.Logging
{
    public static class LoggerManager
    {
        static LoggerManager()
        {
            BasicConfigurator.Configure();
        }

        public static ILog PerformanceLogger => LogManager.GetLogger("PerformanceLogger");


        public static ILog PumpLogger => LogManager.GetLogger("PumpLogger");


        //public static ILog CustLogger => LogManager.GetLogger("CustLogger");
    }
}
