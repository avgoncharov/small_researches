using System;
using System.Globalization;

namespace DateFormatting
{
	class Program
	{
		static void Main()
		{
			var curDate = DateTime.Now;

			//US
			Console.WriteLine("{0}", curDate.ToString("MMMM dd, yyyy hh:mm tt", CultureInfo.InvariantCulture));
			Console.WriteLine("{0}", curDate.ToString(CultureInfo.CreateSpecificCulture("en-US")));
			
			//RU
			Console.WriteLine("{0}", curDate.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture));
			Console.WriteLine("{0}", curDate.ToString(CultureInfo.CreateSpecificCulture("ru-RU")));

			Console.ReadKey();
		}
	}
}

