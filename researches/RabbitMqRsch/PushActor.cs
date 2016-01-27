using System;
using System.Threading;

namespace RabbitMqRsch
{
	/// <summary>
	/// Simple actor which uses push-strategy.
	/// </summary>
	public class PushActor
	{
		public void Process(Message msg, Context context, int id)
		{
			Console.WriteLine("[PUSH_ACTOR] ActorId: {0} msg: {1}.", id, msg.Content);
			Thread.Sleep(5000);
			context.Ack();

		}
	}
}
