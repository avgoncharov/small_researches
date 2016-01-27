using System;
using InterfaceForUnity;

namespace ClassForUnity
{
	public class X1
	{
		public string Host { get; set; }
		public int Port { get; set; }

		public override string ToString()
		{
			return String.Format("{{Host: {0}; Port: {1} }}", Host, Port);
		}
	}

	public class Y1
	{
		public string Name { get; set; }
		public string LastName { get; set; }

		public override string ToString()
		{
			return String.Format("{{Name: {0}; LastName: {1} }}", Name, LastName);
		}
	}

	public class ClassForUnityRsch: IForUnityTest
	{
		public ClassForUnityRsch(X1 x, Y1 y)
		{
			_x = x;
			_y = y;
		}

		public int CountOfStars()
		{
			return Int32.MaxValue;
		}

		public override string ToString()
		{
			return String.Format("{{X: {0}; Y: {1} }}", _x, _y);
		}


		private readonly X1 _x;
		private readonly Y1 _y;
	}
}
