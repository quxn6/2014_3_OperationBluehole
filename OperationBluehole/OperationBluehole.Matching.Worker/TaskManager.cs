using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Matching.Worker
{
	using RabbitMQ.Client;
	using RabbitMQ.Client.Events;
	using OperationBluehole.Content;
	using LitJson;

	static class TaskManager
	{
		public static long workerNum = System.Diagnostics.Stopwatch.GetTimestamp() % 10000000;
		static Matching matching;
		static ConnectionFactory factory = new ConnectionFactory();

		public static void Run( ConnectionFactory factory, Matching matching, string lobbyAddr )
		{
			TaskManager.matching = matching;
			factory.HostName = lobbyAddr;

			using ( var connection = factory.CreateConnection() )
			{
				using ( var channel = connection.CreateModel() )
				{
					channel.QueueDeclare( "matching_queue_" + workerNum, true, false, false, null );

					channel.BasicQos( 0, 1, false );
					var consumer = new QueueingBasicConsumer( channel );
					channel.BasicConsume( "matching_queue_" + workerNum, false, consumer );

					// 로비에 등록
					{
						var newDic = new Dictionary<string, object>();
						newDic.Add( "host", "localhost" ); // 조심해!! 지금은 그냥 localhost지만 만약 다른 서버에서 돌아간다면 제대로된 주소가 들어가야한다.
						newDic.Add( "queueNum", workerNum );
						newDic.Add( "minLev", matching.minLev );
						newDic.Add( "maxLev", matching.maxLev );

						SendToLobby( "SET", newDic );
					}

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
			// 설정 변경
			if ( msg.BasicProperties.Type == "SET" )
			{
				var body = msg.Body;
				var message = Encoding.UTF8.GetString( body );
				var data = JsonMapper.ToObject<Dictionary<string, object>>( message );

				int minLev = (int)data["minLev"];
				int maxLev = (int)data["maxLev"];

				matching.minLev = (ushort)minLev;
				Program.form.UpdateMinLev( matching.minLev );
				matching.maxLev = (ushort)maxLev;
				Program.form.UpdateMaxLev( matching.maxLev );
				matching.Reset();

				return true;
			}

			// 등록
			else if ( msg.BasicProperties.Type == "REG" )
			{
				var body = msg.Body;
				var message = Encoding.UTF8.GetString( body );
				var data = JsonMapper.ToObject<Dictionary<string, object>>( message );
				
				Console.WriteLine( "register : " + message );
				matching.RegisterPlayer( (string)data["playerId"], (int)data["difficulty"] );
				return true;
			}

			// 등록 해제
			else if ( msg.BasicProperties.Type == "DEREG" )
			{
				var body = msg.Body;
				var message = Encoding.UTF8.GetString( body );
				var data = JsonMapper.ToObject<Dictionary<string, object>>( message );

				Console.WriteLine( "deregister : " + message );
				matching.DeregisterPlayer( (string)data["playerId"] );
				return true;
			}

			return false;
		}

		public static void SendToLobby( string type, Dictionary<string, object> data )
		{
			using ( var connection = factory.CreateConnection() )
			{
				using ( var channel = connection.CreateModel() )
				{
					channel.QueueDeclare( "matching_lobby_queue", true, false, false, null );

					var message = JsonMapper.ToJson( data );
					var body = Encoding.UTF8.GetBytes( message );

					var properties = channel.CreateBasicProperties();
					properties.SetPersistent( true );
					properties.Type = type;
					channel.BasicPublish( "", "matching_lobby_queue", properties, body );
				}
			}
		}
	}
}
