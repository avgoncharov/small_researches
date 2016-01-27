using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadsAndTaskRsch
{
	public class Program
	{
		private static long _num;

		public static void Main()
		{
			var sw = new Stopwatch();
			_num = 100;
			sw.Start();
			for(int i = 0; i< 100; ++i)
			{
				Task.Factory.StartNew(()=>
				{
					Thread.Sleep(TimeSpan.FromSeconds(20));
					Interlocked.Decrement(ref _num);
				}, TaskCreationOptions.LongRunning);
			}

			while (Interlocked.Read(ref _num) != 0)
			{
				Thread.Sleep(TimeSpan.FromMilliseconds(20));
			}

			sw.Stop();

			Console.WriteLine(sw.ElapsedMilliseconds / 1000);
			Console.ReadLine();
		}
	}
}
