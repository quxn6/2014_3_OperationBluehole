using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Simulation
{
    using RabbitMQ.Client;

    class Program
    {
        const int WORKER_THREAD_NUM = 1;

        static void Main( string[] args )
        {
            SimulationManager.Init();
            var factory = new ConnectionFactory() { HostName = Config.SIMULATION_QUEUE_ADDRESS };

            for ( int i = 0; i < WORKER_THREAD_NUM; ++i )
                Task.Factory.StartNew( () => TaskManager.Run( factory ) );
        }
    }
}
