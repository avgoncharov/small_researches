using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace RabbitMqRsch.StatisticApi
{
	internal static class RabbitMqStatisticGetter
	{
		public static QueueStat GetStatisticForQueuer(string queueName)
		{
			var c = WebRequest.CreateHttp("http://localhost:15672/api/queues/%2f/" + queueName);
			c.Credentials = new NetworkCredential("guest", "guest");

			var r = c.GetResponse();
			var sr = new StreamReader(r.GetResponseStream());
			var s = sr.ReadToEnd();

			var jr = JsonSerializer.CreateDefault();
			var result = jr.Deserialize<QueueStat>(new JsonTextReader(new StringReader(s)));

			return result;
		}
	}
}
