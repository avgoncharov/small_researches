using System;
using System.Collections.Generic;
using MongoDbPerformanceResearch.Mongo.Model;
using MongoDB.Driver;

namespace MongoDbPerformanceResearch.Mongo
{
	public class TaskRepository
	{
		public TaskRepository(IMongoDatabase database)
		{
			_tasks = database.GetCollection<Task>("Tasks");
			_tasks.Indexes.CreateOneAsync(
				Builders<Task>.IndexKeys.Ascending(u => u.Id),
				new CreateIndexOptions { Unique = true, });
		}

		//public IReadOnlyCollection<Task> GetTaskByTimeout()
		//{
		//	return _tasks.Find(Builders<Task>.Filter.Where(itr => itr.LastProcessedDateTime.Second+ itr.Timeout < DateTime.UtcNow.Second)).ToList();
		//}

		public IReadOnlyCollection<Task> GetAllTask()
		{
			return _tasks.Find(itr=>true).ToList();
		}




		public Task GetTaskById(int id)
		{
			return _tasks.Find(Builders<Task>.Filter.Where(itr => itr.Id == id)).FirstOrDefault();
		}

		public void CreateTask(Task task)
		{
			_tasks.InsertOne(task);
		}

		public void SaveTask(Task task)
		{
			_tasks.UpdateOne(
				Builders<Task>.Filter.Where(itr => itr.Id == task.Id),
				Builders<Task>.Update.
					Set("Content", task.Content).
					Set("LastProcessedDateTime", DateTime.UtcNow));
		}


		public Task GetAndLockTask(int id)
		{
			var task = GetTaskById(id);

			var result = _tasks.UpdateOne(
				Builders<Task>.Filter.Where(itr => itr.Id == task.Id && itr.Content != "Lock"),
				Builders<Task>.Update.
					Set("Content", "Lock").
					Set("LastProcessedDateTime", DateTime.UtcNow));
			
			return result.MatchedCount > 0 ?GetTaskById(id):null;
		}



		private readonly IMongoCollection<Task> _tasks;
	}
}
