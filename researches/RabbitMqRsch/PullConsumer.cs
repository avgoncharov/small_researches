using System;
using System.IO;
using RabbitMQ.Client;

namespace RabbitMqRsch
{
	/// <summary>
	/// Consumer which is used for realization of pull-strategy. RabbitMq adapter.
	/// </summary>
	public class PullConsumer : IDisposable
	{
		public PullConsumer(IModel channel, string queueName)
		{
			_channel = channel;

			_channel.BasicQos(0, 1, false);
			_consumer = new QueueingBasicConsumer(_channel);
			_channel.BasicConsume(queue: queueName, noAck: false, consumer: _consumer);
		}

		public PullDeliveryContext<T> Receive<T>() where T : Message
		{
			try
			{
				var ea = _consumer.Queue.Dequeue();
				return new PullDeliveryContext<T>(_channel, ea);
			}
			catch (EndOfStreamException ex)
			{
				Console.WriteLine(ex);
				return new PullDeliveryContext<T>(_channel, null);
			}
		}

		public void Dispose()
		{
			if (_channel != null)
			{
				try
				{
					_channel.Close();
					_channel.Dispose();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}

				_channel = null;
			}
		}

		private IModel _channel;
		private readonly QueueingBasicConsumer _consumer;
	}
}
