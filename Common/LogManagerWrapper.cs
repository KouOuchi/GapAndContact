using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;

namespace Common
{
    //// TODO: Implement the additional GetLogger method signatures and log4net.LogManager methods that are not seen below.
    public static class LogManagerWrapper
    {
        public static ILog GetLogger(Type type)
        {
            // If no loggers have been created, load our own.
            if (LogManager.GetCurrentLoggers().Length == 0)
            {
                LoadConfig();
            }
            return LogManager.GetLogger(type);
        }

        private static void LoadConfig()
        {
            string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            XmlConfigurator.ConfigureAndWatch(new FileInfo(dir + Path.DirectorySeparatorChar + "log4net.config"));
        }
    }
}
