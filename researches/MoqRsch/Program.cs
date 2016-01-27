using System;
using Moq;

namespace MoqRsch
{
	public interface IM<T>
	{
		void Exec(T msg);
	}

	public interface IProd
	{
		void DoProd<T>(T prod) where T : Message;
	}

	public interface IContext
	{
		void Do<T>(T x) where T : Message;
	}

	public class Message
	{
		public string Content { get; set; }
		public override string ToString()
		{
			return String.Format("{{Content: {0}}}", Content);
		}
	}

	public class DerivedMessage: Message
	{
		public int Id { get; set; }
		public override string ToString()
		{
			return String.Format("{{Id: {0}; Content: {1};}}", Id, Content);
		}
	}

	public class DerivedMessage2 : Message
	{
		public string Name { get; set; }
		public override string ToString()
		{
			return String.Format("{{Name: {0}; Content: {1};}}", Name, Content);
		}
	}

	public class Program
	{
		public static void Main()
		{
			var context = CreateIContextMok<Message>();

			context.Do(new DerivedMessage { Id = 314, Content = "zzz" });
			context.Do(new DerivedMessage2 {Name = "DM2",Content = "xyz" });
			context.Do(new Message { Content = "yyy" });

			Console.WriteLine("--------------------------------------------");
			context = CreateIContextMok<DerivedMessage2>();

			context.Do(new DerivedMessage { Id = 314, Content = "zzz" });
			context.Do(new DerivedMessage2 { Name = "DM2", Content = "xyz" });

			Console.WriteLine("--------------------------------------------");

			// Моки можно делать только на все публичное. 
			// Для непубличного нужны доп телодвижения.
			var m = new Mock<IM<Message>>();
			var x = m.Object;
			Console.WriteLine(x);

			Console.ReadLine();
		}
		
		private static IProd CreateIProdMok<T>() where T : Message
		{
			// Если мы хотим специфицировать шаблонный метод по типу, то 
			// в случае типизации была по дочерниму классу, то уточнить мы можем только для параллельной ветке.
			// В данном случае, если T был Message - то тонкая типизация не работает.
			// Она работает если T находится в параллельной ветке наследования, относительно DerivedMessage.
			// Порядок сетапов не важен.
			var prod = new Mock<IProd>();
			prod.Setup(k => k.DoProd(It.IsAny<T>())).Callback<T>(v => Console.WriteLine("From Prod M: {0}.", v.ToString()));
			prod.Setup(k => k.DoProd(It.IsAny<DerivedMessage>())).Callback<DerivedMessage>(v => Console.WriteLine("DDD: {0}", v.ToString()));
			
			return prod.Object;
		}

		private static IContext CreateIContextMok<T>() where T : Message
		{
			var prod = CreateIProdMok<T>();
			var cntx = new Mock<IContext>();
			
			cntx.Setup(k => k.Do(It.IsAny<T>())).Callback<T>(v =>
			{
				prod.DoProd(v);
			});

			cntx.Setup(k => k.Do(It.IsAny<DerivedMessage>())).Callback<DerivedMessage>(v =>
			{
				Console.WriteLine("From cntx direct. This is DM: {0}.", v.ToString());
			});
			return cntx.Object;
		}
	}
}
