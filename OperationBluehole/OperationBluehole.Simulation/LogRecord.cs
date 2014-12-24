using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Simulation
{
	using System.Diagnostics;
	using System.Collections.Concurrent;

	static class LogRecord
	{
		static System.IO.StreamWriter file;
		static BlockingCollection<string> inputStrs;

		static LogRecord()
		{
			file = new System.IO.StreamWriter( "Simulation_" + Stopwatch.GetTimestamp() + ".txt", true );
			file.AutoFlush = true;
			inputStrs = new BlockingCollection<string>();

			file.WriteLine( "[Tick Per Second : " + Stopwatch.Frequency + "]" );

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
