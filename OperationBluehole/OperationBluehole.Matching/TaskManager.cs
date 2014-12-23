using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Matching
{
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using OperationBluehole.Content;
    using LitJson;

    static class TaskManager
    {
        public static void Run( ConnectionFactory factory )
        {
            using ( var connection = factory.CreateConnection() )
            {
                using ( var channel = connection.CreateModel() )
                {
                    channel.QueueDeclare( "matching_queue", true, false, false, null );

                    channel.BasicQos( 0, 1, false );
                    var consumer = new QueueingBasicConsumer( channel );
                    channel.BasicConsume( "matching_queue", false, consumer );

                    Console.WriteLine( " [*] Waiting for messages. " + "To exit press CTRL+C" );

                    while ( true )
                    {
                        var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString( body );

                        // 등록
                        Console.WriteLine( "register : " + message );
                        var matchingData = JsonMapper.ToObject<Dictionary<string, object>>( message );
                        MatchingManager.RegisterPlayer( (string)matchingData["playerId"], (int)matchingData["difficulty"] );

                        // 매칭
//                         Console.WriteLine("matching");
//                         MatchingManager.MatchPlayer();

                        channel.BasicAck( ea.DeliveryTag, false );
                    }
                }
            }
        }
    }
}
