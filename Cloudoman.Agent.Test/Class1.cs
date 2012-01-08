using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cloudoman;
using Cloudoman.Agent;
using Microsoft.ServiceBus;
using NUnit.Framework;
using Microsoft.ServiceBus.Messaging;


namespace WebCmdRunner.Test
{

    [TestFixture]
    public class Class1
    {
        readonly string _issuer = Environment.GetEnvironmentVariable("issuer",EnvironmentVariableTarget.Machine);
        readonly string _key = Environment.GetEnvironmentVariable("key",EnvironmentVariableTarget.Machine);
        string _queueName = Environment.GetEnvironmentVariable("key",EnvironmentVariableTarget.Machine);
        private string _sbNamespace = "testameer";


        [Test]
        public void Hello()
        {
            var cmdList = new[] {
                    new RemoteCommand {Url ="https://github.com/writeameer/WebcmdRunner/raw/master/scriptrepo/DumpApp.exe"},
                    new RemoteCommand {Url ="https://github.com/writeameer/WebcmdRunner/raw/master/scriptrepo/test.bat"},
                    new RemoteCommand {Url ="https://github.com/writeameer/WebcmdRunner/raw/master/scriptrepo/test.ps1"}
                };

            foreach (var output in cmdList.Select(cmd => cmd.Execute()))
                Console.WriteLine(output);

        

        }

        [Test]
        public void ReceiveMessage()
        {
            // URI address and token for our "HowToSample" namespace
            var tP = TokenProvider.CreateSharedSecretTokenProvider(_issuer, _key);
            var uri = ServiceBusEnvironment.CreateServiceUri("sb",_sbNamespace, string.Empty);
            var factory = MessagingFactory.Create(uri, tP);

            // Receive the cow
            var messageReceiver = factory.CreateMessageReceiver(_queueName);
            for (var i = 0; i <= 10; i++)
            {

                var receivedMessage = messageReceiver.Receive();
                var urlList = receivedMessage.GetBody<List<string>>();

                foreach (var url in urlList)
                {
                    Console.WriteLine(url);
                }
            }

        }
    }
}
