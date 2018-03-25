using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using IMQ1.Multithreading.Helpers;
using IMQ1.Multithreading.Helpers.Extensions;

namespace IMQ1.Multithreading.Server
{
    public class Server
    {
        private const int MaxNumberOfClients = 4;
        private readonly List<NamedPipeServerStream> _serverStreams;

        public Server()
        {
            _serverStreams = new List<NamedPipeServerStream>();
        }

        public void Start()
        {
            int i;
            var servers = new Thread[MaxNumberOfClients];

            Console.WriteLine("Waiting for client connect...\n");
            for (i = 0; i < MaxNumberOfClients; i++)
            {
                servers[i] = new Thread(ServerThread);
                servers[i].Start();
            }
            Thread.Sleep(250);

            while (i > 0)
            {
                for (var j = 0; j < MaxNumberOfClients; j++)
                {
                    if (servers[j] != null)
                    {
                        if (servers[j].Join(250))
                        {
                            Console.WriteLine("Server thread[{0}] finished.", servers[j].ManagedThreadId);
                            servers[j] = null;
                            i--;
                        }
                    }
                }
            }
            Console.WriteLine("\nServer threads exhausted, exiting.");
        }
        private void ServerThread(object data)
        {
            var pipeServer = new NamedPipeServerStream(Constants.PipeName, PipeDirection.InOut, MaxNumberOfClients, PipeTransmissionMode.Message);

            pipeServer.WaitForConnection();

            _serverStreams.Add(pipeServer);

            var threadId = Thread.CurrentThread.ManagedThreadId;

            Console.WriteLine("Thread[{0}]. User: {1}", threadId, pipeServer.GetImpersonationUserName());
            try
            {
                while (pipeServer.IsConnected)
                {
                    var message = pipeServer.ReadString();

                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        Console.WriteLine("Thread[{0}]. User: {1}. Message: {2}", threadId, pipeServer.GetImpersonationUserName(), message);
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

            pipeServer.Close();

            _serverStreams.Remove(pipeServer);
        }

    }
}
