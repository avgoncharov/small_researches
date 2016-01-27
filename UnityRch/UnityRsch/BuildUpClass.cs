using System;

namespace UnityRsch
{
	public interface IResource
	{
		void Print();
	}

	public class Resource : IResource
	{
		public void Print()
		{
			Console.WriteLine("Hello from Resource.");
		}
	}

	public class BuildUpClass
	{
		public IResource Resource { get; set; }

		public BuildUpClass(IResource resource)
		{
			Resource = resource;
		}
	}
}
