using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationBluehole.Matching
{
	using System.Diagnostics;
	using System.Collections.Concurrent;

	static class LogRecord
	{
		static System.IO.StreamWriter file;
		static BlockingCollection<string> inputStrs;
		public static Int64 tickPerMillisecond;

		static LogRecord()
		{
			file = new System.IO.StreamWriter( "Matching_"+Stopwatch.GetTimestamp()+".txt", true );
			file.AutoFlush = true;
			tickPerMillisecond = Stopwatch.Frequency / 1000;
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
