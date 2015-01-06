using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Matching.Worker
{
	using System.Diagnostics;
	using System.Collections.Concurrent;

	static class LogRecord
	{
		static System.IO.StreamWriter file;
		static BlockingCollection<string> inputStrs;

		static LogRecord()
		{
			file = new System.IO.StreamWriter( "Matching" + TaskManager.workerNum + "_" + DateTime.Now.ToString( "yyyy.MM.dd-hhmmss" ) + ".txt", true );
			file.AutoFlush = true;
			inputStrs = new BlockingCollection<string>();

			Task.Factory.StartNew( Recording );
		}

		public static void Write( string inputStr )
		{
			inputStrs.Add( inputStr );
		}

		static void Recording()
		{
			foreach ( var str in inputStrs.GetConsumingEnumerable() )
			{
				file.WriteLine( str );
			}
		}
	}
}
