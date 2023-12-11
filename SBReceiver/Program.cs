
using Microsoft.Azure.ServiceBus;
using Microsoft.IdentityModel.Tokens;
using SBReceiver;
using SBShared.Models;
using System.Text;
using System.Text.Json;


namespace SBReceiver
{
    internal class Program
    {

        static IQueueClient queueClient;
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            const string connectionString = "Endpoint=sb://rukshan.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=KMDqV1LIAnfsePz8EjZEx3FaWplLgktc2+ASbGoPqXU=";
            const string queueName = "personqueue";
           


            queueClient = new QueueClient(connectionString, queueName);

            var messageHandlerOptios = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false

            };

            queueClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptios);

            Console.ReadLine();

            await queueClient.CloseAsync();

        }

        public static async Task ProcessMessageAsync(Message message, CancellationToken token)
        {
            var jasonString = Encoding.UTF8.GetString(message.Body);
            PersonModel person = JsonSerializer.Deserialize<PersonModel>(jasonString);
            Console.WriteLine($"Person Received: { person.FirstName } {person.LastName}");

            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
            //throw new NotImplementedException();
        }

        public static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            Console.WriteLine($"Message handler exception: { arg.Exception }");
            return Task.CompletedTask;
            //throw new NotImplementedException();

        }



    }
}
