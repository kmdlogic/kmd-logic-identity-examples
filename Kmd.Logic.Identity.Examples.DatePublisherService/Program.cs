using System;
using System.IO;
using Topshelf;
using Microsoft.Extensions.Configuration;

namespace Kmd.Logic.Identity.Examples.DatePublisherService
{
    public class Program
    {
        private static IConfiguration _config;

        public static void Main()
        {
            LoadConfiguration();
            var clientCredentialsConfig = new ClientCredentialsConfig();
            _config.Bind("ClientCredentials", clientCredentialsConfig);

            var rc = HostFactory.Run(x =>
            {
                x.Service<DatePublisher>(s =>
                {
                    s.ConstructUsing(name => new DatePublisher(
                        10000, 
                        "https://localhost:44327/api/dates", 
                        clientCredentialsConfig));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Publishes dates to the DatesApi on a timed schedule");
                x.SetDisplayName("Date Publisher Service");
                x.SetServiceName("Date Publisher Service");
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }

        private static void LoadConfiguration()
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
        }
    }
}
