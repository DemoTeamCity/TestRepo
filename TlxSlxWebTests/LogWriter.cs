using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace TlxSlxWebTests
{
    public static class LogWriter
    {
        public static void Write(string message)
        {
            Console.WriteLine("\t" + message);
        }

        public static void PrintTestDetails()
        {
            //Console.WriteLine(Convert.ToString(TestContext.CurrentContext.Test.Name));
        }
    }
}
