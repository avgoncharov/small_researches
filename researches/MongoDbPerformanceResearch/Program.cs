using System;
using System.Configuration;
using MongoDbPerformanceResearch.Mongo;
using MongoDB.Driver;

namespace MongoDbPerformanceResearch
{
	public class Program
	{
		static void Main()
		{
			var mongoUrl = MongoUrl.Create(ConfigurationManager.ConnectionStrings["TestDb"].ConnectionString);
			var cln = new MongoClient(mongoUrl);
			var db = cln.GetDatabase(mongoUrl.DatabaseName);

			var rep = new TaskRepository(db);
//			var rnd = new Random();
//			for (int i = 1; i < 100001; ++i)
//			{
//				rep.CreateTask(new Task
//				{
//					Id = i,
//					LastProcessedDateTime = DateTime.UtcNow,
//					Timeout = rnd.Next(100),
//					Preority = rnd.Next(10),
//					TaskType = rnd.Next(1000) > 600 ? "Feed" : "Page"
//				});
//			}


			var task = rep.GetTaskById(2);
			task.Content = "UnLock";
			rep.SaveTask(task);

			rep.GetAndLockTask(2);


			//for (int i = 1; i < 10000; ++i)
			//{
			//	var task = rep.GetTaskById(i);
			//	task.Content = "hello_" + i.ToString();
			//	rep.SaveTask(task);
			//}
			//var tasks = rep.GetTaskByTimeout();
			//Console.WriteLine("Timeout expired for {0} tasks", tasks.Count);

			//foreach (var itr in tasks)
			//{
			//	Console.WriteLine(itr);
			//}
			Console.WriteLine("Completed.");

			Console.ReadLine();
		}
	}
}
