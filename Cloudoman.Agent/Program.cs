using System;
using Topshelf;

namespace Cloudoman.Agent
{
    public class AwsAgent
    {
        // Func to create Time stamp based on RFC 822
        public static Func<string,string> Ec2Metadata = (x) =>{
            var webClient = new System.Net.WebClient();
            return webClient.DownloadString("http://169.254.169.254/latest/user-data/");
        };


        public static void Main(string[] args)
        {
            var queueName = Ec2Metadata("queueName");
            var issuer = Ec2Metadata("issuer");
            var key = Ec2Metadata("key");

            var messageProcessor = new MessageProcessor(
                queueName:queueName, 
                serviceNameSpace:"testameer",
                issuer:issuer,
                key:key);

            HostFactory.Run(x =>
            {
                x.Service<MessageProcessor>(s =>
                {
                    s.SetServiceName("TownCrier");
                    s.ConstructUsing(name => new MessageProcessor(queueName,"testameer",issuer,key));
                    s.WhenStarted(mp => mp.Start());
                    s.WhenStopped(mp => mp.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("Cloudoman AWS Agent");
                x.SetDisplayName("Cloudoman Agent");
                x.SetServiceName("CloudomanAgent");
            });                                                  
        }

    }
}
