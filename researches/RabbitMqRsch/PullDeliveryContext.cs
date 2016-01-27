using System;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqRsch
{
	/// <summary>
	/// Context of delivery, IDisposable for automatic rejection.
	/// </summary>
	/// <typeparam name="T">Message type.</typeparam>
	public class PullDeliveryContext<T>: IDisposable where T:Message
	{
		public PullDeliveryContext(IModel channel, BasicDeliverEventArgs ea)
		{
			_done = 0;
			_channel = channel;
			_ea = ea;

			if(ea != null)
				Message = SimpleSerializer.Deserialize<T>(ea.Body);
		}

		public void Ack()
		{
			if (_ea == null)
				return;

			if (Interlocked.CompareExchange(ref _done, 1, 0) == 0)
			{
				_channel.BasicAck(_ea.DeliveryTag, false);
			}
		}

		public void Reject()
		{
			if (_ea == null)
				return;

			if (Interlocked.CompareExchange(ref _done, 1, 0) == 0)
			{
				_channel.BasicReject(_ea.DeliveryTag, true);
			}
		}

		public void Dispose()
		{
			Reject();
		}

		public T Message { get; private set; }
		private readonly IModel _channel;
		private readonly BasicDeliverEventArgs _ea;
		private int _done;

		
	}
}
