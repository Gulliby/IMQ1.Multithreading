using System;

namespace IMQ1.Multithreading.Client
{
    public class Program
    {
        public static void Main()
        {
            var clinet = new Client();

            clinet.StartClient();

            Console.ReadKey();
        }
    }
}
