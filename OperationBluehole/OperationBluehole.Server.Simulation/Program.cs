using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Server.Simulation
{
    using RabbitMQ.Client;

    class Program
    {
        const int WORKER_THREAD_NUM = 2;

        static void Main( string[] args )
        {
            SimulationManager.Init();
            var factory = new ConnectionFactory() { HostName = "localhost" };
            
            for ( int i = 0; i < WORKER_THREAD_NUM; ++i )
                Task.Factory.StartNew( () => TaskManager.Run( factory ) );
        }
    }
}
