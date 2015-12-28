using System;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDbPerformanceResearch.Mongo.Model
{
	public class Task
	{
		[BsonId]
		public int Id { get; set; }
		public int Preority { get; set; }
		public DateTime LastProcessedDateTime { get; set; }
		public string TaskType { get; set; }
		public int Timeout { get; set; }
		public string Content { get; set; }

		public override string ToString()
		{
			return String.Format("Task {{ Id:{0}; Preority: {1}; TaskType: {2}; Timeout: {3}; LastProcDateTime:{4:dd-MM-yyyy HH:mm:ss.fff} }}", Id, Preority, TaskType, Timeout, LastProcessedDateTime);
		}
	}
}
