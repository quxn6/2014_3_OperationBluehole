using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Matching.Lobby
{
	using RabbitMQ.Client;
	using RabbitMQ.Client.Events;
	using OperationBluehole.Content;
	using LitJson;

	static class TaskManager
	{
		public static long workerNum = System.Diagnostics.Stopwatch.GetTimestamp();
		static MatchingManager matchingManager;

		public static void Run( ConnectionFactory factory, MatchingManager matchingManager )
		{
			TaskManager.matchingManager = matchingManager;

			using ( var connection = factory.CreateConnection() )
			{
				using ( var channel = connection.CreateModel() )
				{
					channel.QueueDeclare( "matching_lobby_queue", true, false, false, null );

					channel.BasicQos( 0, 1, false );
					var consumer = new QueueingBasicConsumer( channel );
					channel.BasicConsume( "matching_lobby_queue", false, consumer );

					Console.WriteLine( " [*] Waiting for messages. " + "To exit press CTRL+C" );

					while ( true )
					{
						var msg = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

						if ( !HandleMessage( msg ) )
							Console.WriteLine( "Fail to handle a message." );

						channel.BasicAck( msg.DeliveryTag, false );
					}
				}
			}
		}
		static bool HandleMessage( BasicDeliverEventArgs msg )
		{
			var body = msg.Body;
			var message = Encoding.UTF8.GetString( body );
			var data = JsonMapper.ToObject<Dictionary<string, object>>( message );

			if ( msg.BasicProperties.Type == "SET" )
			{
				var host = (string)data["host"];
				var queueNum = (int)data["queueNum"];
				var minLev = (int)data["minLev"];
				var maxLev = (int)data["maxLev"];

				matchingManager.SetWorker( host, queueNum, (ushort)minLev, (ushort)maxLev );

				return true;
			}
			else if ( msg.BasicProperties.Type == "REG" )
			{
				// 등록
				Console.WriteLine( "register : " + message );
				matchingManager.RegisterPlayer( (string)data["playerId"], (int)data["difficulty"] );

				return true;
			}
			else if ( msg.BasicProperties.Type == "DEREG" )
			{
				// 등록 해제
				Console.WriteLine( "deregister : " + message );
				matchingManager.DeregisterPlayer( (string)data["playerId"] );

				return true;
			}

			return false;
		}

		public static void SendToWorker( WorkerData wData, string type, Dictionary<string, object> data )
		{
			var channel = wData.factory.CreateConnection().CreateModel();
			channel.QueueDeclare( "matching_queue_" + wData.queueNum, true, false, false, null );

			var message = JsonMapper.ToJson( data );
			var body = Encoding.UTF8.GetBytes( message );

			var properties = channel.CreateBasicProperties();
			properties.SetPersistent( true );
			properties.Type = type;
			channel.BasicPublish( "", "matching_queue_" + wData.queueNum, properties, body );
		}
	}
}
