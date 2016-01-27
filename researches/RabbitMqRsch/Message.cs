using System;
using System.Runtime.Serialization;

namespace RabbitMqRsch
{
	[DataContract]
	public class Message
	{
		[DataMember]
		public string Content { get; set; }

		public override string ToString()
		{
			return String.Format("Message: {{ Content: '{0}' }}", Content);
		}
	}

	[DataContract]
	public class MessageWrapper<T> where T : Message
	{
		[DataMember]
		public Guid Guid { get; set; }

		[DataMember]
		public T Message { get; set; }


	}

	[DataContract]
	public class Message2 : Message
	{
		[DataMember]
		public int Count { get; set; }

		public override string ToString()
		{
			return String.Format("Message2: {{ Content: '{0}'; Count: {1} }}", Content, Count);
		}
	}
}
