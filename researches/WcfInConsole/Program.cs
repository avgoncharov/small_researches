using System;
using System.ServiceModel;

namespace WcfInConsole
{
	class Program
	{
		static void Main()
		{
			using (var host = new ServiceHost(typeof(WCF.TestService)))
			{
				host.Open();
			
				Console.WriteLine("Press <Enter> to stop the service.");
				Console.ReadLine();

				host.Close();
			}
		}
	}
}
