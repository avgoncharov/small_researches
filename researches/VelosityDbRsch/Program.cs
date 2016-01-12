using System;
using System.Diagnostics;
using System.Linq;
using VelocityDb.Session;

namespace VelosityDbRsch
{
	public class Program
	{
		public static void Main(string[] args)
		{
			ClearDb();

			var sw = new Stopwatch();
			sw.Start();
			var dt = DateTime.UtcNow;
			FillDb();
			sw.Stop();
			Console.WriteLine("Time for filling: {0}", sw.ElapsedMilliseconds);
			sw.Restart();
			ReadDb(dt.AddSeconds(1));
			sw.Stop();
			Console.WriteLine("Time for filter: {0}", sw.ElapsedMilliseconds);
			Console.WriteLine("Completed.");
			Console.ReadLine();
		}

		private static void ReadDb(DateTime date)
		{
			using (var session = new ServerClientSession(@"K:\av_goncharov\tmp\VelocityDb"))
			{
				session.BeginRead();

				var objs = session.AllObjects<ModelObject>().Where(o => o.LastCheckeDateTime < date).ToArray();
				Console.WriteLine("Date: {0:yyy-MM-dd HH:mm:ss.fff}.", date);
				Console.WriteLine("[Before] First content: {0}; Date: {1:yyy-MM-dd HH:mm:ss.fff}.", objs.First().Content, objs.First().LastCheckeDateTime);
				Console.WriteLine("[Before] Last content: {0}; Date: {1:yyy-MM-dd HH:mm:ss.fff}.", objs.Last().Content, objs.Last().LastCheckeDateTime);
				Console.WriteLine("[Before] Count: {0}", objs.Length);
				
				//objs = session.AllObjects<ModelObject>().Where(o => o.LastCheckeDateTime >= date).ToArray();
				//Console.WriteLine("[After] First content: {0}; Date: {1:yyy-MM-dd HH:mm:ss.fff}.", objs.First().Content, objs.First().LastCheckeDateTime);
				//Console.WriteLine("[After] Last content: {0}; Date: {1:yyy-MM-dd HH:mm:ss.fff}.", objs.Last().Content, objs.Last().LastCheckeDateTime);
				//Console.WriteLine("[After] Count: {0}", objs.Length);


				session.Commit();
			}
		}

		private static void FillDb()
		{
			using (var session = new ServerClientSession(@"tmp\VelocityDb"))
			{
				try
				{
					session.BeginUpdate();

					for (var i = 0; i < 10000000; ++i)
					{
						var buf = new ModelObject{Content = "A_" + i.ToString(), LastCheckeDateTime = DateTime.UtcNow};
						session.Persist(buf);
					}

					session.Commit();
				}
				catch 
				{
					session.Abort();
					throw;
				}
			}
		}

		private static void ClearDb()
		{
			using (var session = new ServerClientSession(@"tmp\VelocityDb"))
			{
				try
				{
					session.BeginUpdate();
					var dbs = session.OpenAllDatabases();
					foreach (var itr in dbs.Where(itr => itr.Name.Contains("ModelObject")))
					{
						Console.WriteLine("{0} was deleted.", itr.Name);
						session.DeleteDatabase(itr);
					}
					

					session.Commit();
				}
				catch 
				{
					session.Abort();
					throw;
				}
			}
		}
	}
}
