using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMqRsch
{
	/// <summary>
	/// Actor which uses a pull-strategy.
	/// </summary>
	public class PullActor:IDisposable
	{
		public PullActor(PullConsumer consumer, CancellationToken token,int id)
		{
			_token = token;
			_id = id;
			_consumer = consumer;

			_task = Task.Factory.StartNew(Processing, TaskCreationOptions.LongRunning);
		}

		private void Processing()
		{
			while (!_token.IsCancellationRequested)
			{
				using (var context = _consumer.Receive<Message>())
				{
					try
					{
						Console.WriteLine("[PULL_ACTOR] Id: {0} msg: {1}.", _id, context.Message.Content);
						context.Ack();
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
					}
				}
			}
		}

		public void Dispose()
		{
			if (_consumer != null)
			{
				try
				{
					_consumer.Dispose();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}

				_consumer = null;
			}

			if (_task != null)
			{
				try
				{
					_task.Dispose();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}

				_task = null;
			}
		}

		private readonly CancellationToken _token;
		private Task _task;
		private readonly int _id;
		private PullConsumer _consumer;
	}
}
