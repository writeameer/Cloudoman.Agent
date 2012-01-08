using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace Cloudoman.Agent
{
    class MessageProcessor
    {

        private string _queueName;
        private MessageReceiver _messageReceiver;
        private MessageSender _messageSender;
        private MessagingFactory _factory;

        public MessageProcessor(string queueName, string serviceNameSpace, string issuer, string key)
        {
            // Create Token Provider and Service Uri
            var tokenProvider = TokenProvider.CreateSharedSecretTokenProvider(issuer, key);
            var serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceNameSpace, string.Empty);

            // Create a messaging receiver
            _factory = MessagingFactory.Create(serviceUri, tokenProvider);
            _messageReceiver = _factory.CreateMessageReceiver(queueName);
            //_messageSender = _factory.CreateMessageSender(queueName);
        }

        public void Start()
        {
            
            while (true)
            {
                  try
                  {
                      // Wait for message from queue
                      var receivedMessage = _messageReceiver.Receive();

                      // Read message
                      var urlList = receivedMessage.GetBody<List<string>>();

                      // For each Url in message, execute script
                      foreach (var url in urlList)
                      {
                          var cmd = new RemoteCommand {Url = url};
                          cmd.Execute();
                      }
                  }   
                  catch(Exception e)
                  {
                      // Log errors if any and continue
                      Console.WriteLine(e.Message + ":" + e.StackTrace);
                  }
            }
        }

        public void Stop()
        {
            _messageReceiver.Close();
            _factory.Close();
        }
    }
}
