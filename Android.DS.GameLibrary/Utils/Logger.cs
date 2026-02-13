using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLibrary.Utils
{
    public static class Logger
    {
        public static void Log(object logMessage)
        {
            System.Diagnostics.Debug.WriteLine(logMessage.ToString());
        }
    }
}
