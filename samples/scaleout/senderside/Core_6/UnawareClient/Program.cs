﻿using System;
using System.Threading.Tasks;
using NServiceBus;

class Program
{
    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        Console.Title = "Samples.SenderSideScaleOut.UnawareClient";
        var endpointConfiguration = new EndpointConfiguration("Samples.SenderSideScaleOut.UnawareClient");
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        endpointConfiguration.SendFailedMessagesTo("error");

        var routing = endpointConfiguration.Routing();
        routing.RouteToEndpoint(typeof(DoSomething), "Samples.SenderSideScaleOut.Server");

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        Console.WriteLine("Press enter to send a message");
        Console.WriteLine("Press any key to exit");
        var sequenceId = 0;
        while (true)
        {
            var key = Console.ReadKey();
            if (key.Key != ConsoleKey.Enter)
            {
                break;
            }
            var command = new DoSomething { SequenceId = ++sequenceId };
            await endpointInstance.Send(command)
                .ConfigureAwait(false);
            Console.WriteLine($"Message {command.SequenceId} Sent");
        }
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}