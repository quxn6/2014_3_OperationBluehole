namespace OperationBluehole.Server
{
    using System;
    using Nancy.Hosting.Self;
    using OperationBluehole.Content;

    class Program
    {
        static void Main(string[] args)
        {
            var uri =
                new Uri("http://localhost:3579");

            HostConfiguration config = new HostConfiguration();
            config.UrlReservations.CreateAutomatically = true;

            using (var host = new NancyHost(config, uri))
            {
                host.Start();

                Console.WriteLine("Your application is running on " + uri);
                Console.WriteLine("Press any [Enter] to close the host.");
                Console.ReadLine();
            }
        }
    }
}
