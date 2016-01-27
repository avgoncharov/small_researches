using System;
using System.Threading;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace RabbitMqRsch
{
	/// <summary>
	/// Delivery context for push-strategy.
	/// IDisposable for auto-rejection.
	/// </summary>
	public class Context: IDisposable
	{
		public Context(IModel channel, BasicDeliverEventArgs ea)
		{
			_channel = channel;
			_ea = ea;
			_done = 0;
		}

		public void Ack()
		{
			if (Interlocked.CompareExchange(ref _done, 1, 0) == 0)
			{
				_channel.BasicAck(_ea.DeliveryTag, false);
			}
		}

		public void Reject()
		{
			if (Interlocked.CompareExchange(ref _done, 1, 0) == 0)
			{
				_channel.BasicReject(_ea.DeliveryTag, true);
			}
		}

		public void Dispose()
		{
			Reject();
		}

		private int _done; 
		private readonly IModel _channel;
		private readonly BasicDeliverEventArgs _ea;

		
	}
}
