using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cobos.Core.Log
{
    public static class Logger
    {
        private static LogWriter _instance;

        public static LogWriter Instance
        {
            get
            {
                if ( _instance == null )
                {
                    _instance = new LogWriter();
                }
                return _instance;
            }
        }
    }
}
