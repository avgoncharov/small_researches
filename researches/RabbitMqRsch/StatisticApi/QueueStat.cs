using System;

namespace RabbitMqRsch.StatisticApi
{
	public class QueueStat
	{
		public int messages { get; set; }
		public int messages_ready { get; set; }
		public int messages_unacknowledged { get; set; }
		public MessageStatus message_stats { get; set; }

		public override string ToString()
		{
			return String.Format(
				"{{\n\t" +
				"Messages:                {0};\n\t" +
				"Messages ready:          {1};\n\t" +
				"Messages unacknowledged: {2};\n\t" +
				"Message stats: {3}}}\n}}",
				messages,
				messages_ready,
				messages_unacknowledged,
				message_stats);
		}
	}

	public class MessageStatus
	{
		public double ack { get; set; }
		public double get { get; set; }
		public double get_no_ack { get; set; }
		public double deliver { get; set; }
		public double deliver_get { get; set; }
		public double publish { get; set; }
		public double redeliver { get; set; }

		public override string ToString()
		{
			return String.Format(
				"{{\n\t\t" +
				"Ack:         {0};\n\t\t" +
				"Get:         {1};\n\t\t" +
				"Get no ack:  {2};\n\t\t" +
				"Deliver:     {3};\n\t\t" +
				"Deliver get: {4};\n\t\t" +
				"Publish:     {5};\n\t\t" +
				"Redeliver:   {6};}}",
				ack,
				get,
				get_no_ack,
				deliver,
				deliver_get,
				publish,
				redeliver);
		}
	}
}
