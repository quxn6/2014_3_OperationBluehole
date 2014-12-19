using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Server.Simulation
{
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using OperationBluehole.Content;
    using LitJson;

    static class TaskManager
    {
        public static void Run( ConnectionFactory factory)
        {
            using ( var connection = factory.CreateConnection() )
            {
                using ( var channel = connection.CreateModel() )
                {
                    channel.QueueDeclare( "task_queue", true, false, false, null );

                    channel.BasicQos( 0, 1, false );
                    var consumer = new QueueingBasicConsumer( channel );
                    channel.BasicConsume( "task_queue", false, consumer );

                    Console.WriteLine( " [*] Waiting for messages. " + "To exit press CTRL+C" );

                    while ( true )
                    {
                        var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString( body );

                        // 보내준 파티 정보를 사용해서 시뮬레이션 실행
                        var party = JsonMapper.ToObject<Party>( message );
                        SimulationManager.Simulation( party );

                        channel.BasicAck( ea.DeliveryTag, false );
                    }
                }
            }
        }
    }
}
