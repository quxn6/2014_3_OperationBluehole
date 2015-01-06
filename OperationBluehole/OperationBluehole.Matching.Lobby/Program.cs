using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Matching.Lobby
{
	class Program
	{
		static void Main( string[] args )
		{
			var factory = new RabbitMQ.Client.ConnectionFactory() { HostName = "localhost" };
			TaskManager.Run( factory, new MatchingManager() );
		}
	}
}
