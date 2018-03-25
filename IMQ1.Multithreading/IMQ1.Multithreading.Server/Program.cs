using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using IMQ1.Multithreading.Helpers;
using IMQ1.Multithreading.Helpers.Extensions;

namespace IMQ1.Multithreading.Server
{
    public class Program
    {

        public static void Main()
        {
            var server = new Server();

            server.Start();
        }

        
    }
}
