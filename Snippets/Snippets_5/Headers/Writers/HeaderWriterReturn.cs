﻿using System;
using System.Collections.Generic;
using System.Threading;
using NServiceBus;
using NServiceBus.MessageMutator;
using NUnit.Framework;
using Operations.Msmq;

[TestFixture]
public class HeaderWriterReturn
{
    public static ManualResetEvent ManualResetEvent;
    static IBus Bus;
    string endpointName = "HeaderWriterReturnV5";

    [SetUp]
    [TearDown]
    public void Setup()
    {
        QueueCreation.DeleteQueuesForEndpoint(endpointName);
    }

    [Test]
    public void Write()
    {
        ManualResetEvent = new ManualResetEvent(false);
        BusConfiguration config = new BusConfiguration();
        config.EndpointName(endpointName);
        IEnumerable<Type> typesToScan = TypeScanner.NestedTypes<HeaderWriterReturn>(typeof(ConfigErrorQueue));
        config.TypesToScan(typesToScan);
        config.EnableInstallers();
        config.UsePersistence<InMemoryPersistence>();
        config.RegisterComponents(c => c.ConfigureComponent<Mutator>(DependencyLifecycle.InstancePerCall));
        using (IStartableBus startableBus = NServiceBus.Bus.Create(config))
        using (Bus = startableBus.Start())
        {
            Bus.SendLocal(new MessageToSend());
            ManualResetEvent.WaitOne();
        }
    }

    class MessageToSend : IMessage
    {
    }

    class MessageHandler : IHandleMessages<MessageToSend>
    {

        public void Handle(MessageToSend message)
        {
            Bus.Return(100);
        }
    }

    class Mutator : IMutateIncomingTransportMessages
    {
        public void MutateIncoming(TransportMessage transportMessage)
        {
            if (transportMessage.IsMessageOfTye<MessageToSend>())
            {
                string sendingText = HeaderWriter.ToFriendlyString < HeaderWriterReturn>(transportMessage.Headers);
                SnippetLogger.Write(text: sendingText, suffix: "Sending");
            }
            else
            {
                string returnText = HeaderWriter.ToFriendlyString<HeaderWriterReturn>(transportMessage.Headers);
                SnippetLogger.Write(text: returnText, suffix: "Returning");
                ManualResetEvent.Set();
            }

        }

    }
}