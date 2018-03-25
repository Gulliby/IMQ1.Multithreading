using System;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;
using IMQ1.Multithreading.Client.Constants;
using IMQ1.Multithreading.Helpers.Extensions;

namespace IMQ1.Multithreading.Client
{
    public class Client
    {
        private const int MaxNumberOfMessages = 10;
        private const int MaxTimeToWait = 1000;

        private readonly int _numberOfMessagesToSend;

        public Client()
        {
            _numberOfMessagesToSend = new Random().Next(MaxNumberOfMessages);
        }

        public void StartClient()
        {
            var pipeClient = new NamedPipeClientStream(
                Helpers.Constants.ServerName,
                Helpers.Constants.PipeName,
                PipeDirection.InOut,
                PipeOptions.None,
                TokenImpersonationLevel.Identification,
                System.IO.HandleInheritability.None);


            var randomTimeRand = new Random();
            var messagePositionRand = new Random();

            Console.WriteLine("Connecting to server...");

            pipeClient.Connect();

            pipeClient.ReadMode = PipeTransmissionMode.Message;

            Console.WriteLine("Connected.");

            for (var i = 0; i < _numberOfMessagesToSend; i++)
            {
                var messagePosition = messagePositionRand.Next(Messages.CustomMessages.Length - 1);
                var message = Messages.CustomMessages[messagePosition];

                pipeClient.WriteString(message);

                Console.WriteLine("Message: ({0}) was sent.", message);

                var serverMessage = GetMessageIfExist(pipeClient);

                if (serverMessage.Equals(Helpers.Constants.ServerCommand.ServerFinished, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                Thread.Sleep(randomTimeRand.Next(MaxTimeToWait));
            }

            pipeClient.Close();
        }

        private static string GetMessageIfExist(PipeStream clientStream)
        {
            return !clientStream.IsMessageComplete ? clientStream.ReadString() : string.Empty;
        }
    }
}
