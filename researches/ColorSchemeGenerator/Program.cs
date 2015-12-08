using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace ColorSchemeGenerator
{
	class Program
	{
		static void Main()
		{
			var sb = new StringBuilder();
			sb.Append("<html><body><table>");
						
			var buf = new List<string>();
			
			//Direct(sb);
			FillBySin(buf);
			
			foreach (var itr in Shuffle(buf))
			{
				sb.Append(itr);
			}
						
			sb.Append("</table></body></html>");

			File.WriteAllText("test.html", sb.ToString());

		}

		private static IEnumerable<string> Shuffle(IList<string> buf)
		{
			var result = new List<string>();
			var rnd = new Random();
			while (buf.Count > 0)
			{
				int index = rnd.Next(buf.Count);
				result.Add(buf[index]);
				buf.RemoveAt(index);
			}

			return result;
		}

		private static void FillBySin(ICollection<string> sb)
		{
			const double step = 1e-4;

			int rAmp = 120;
			double rPh = 10;
			double rFr = 5 * Math.PI * step;

			int gAmp = 100;
			double gPh = 2.5;
			double gFr = 2 * Math.PI * step;

			int bAmp = 90;
			double bPh = 0;
			double bFr = 10 * Math.PI * step;


			for (int i = 0; i < 10000; ++i)
			{
				var sR = Convert.ToInt32(Math.Round(rAmp * Math.Sin(rPh + rFr * i)));
				var sG = Convert.ToInt32(Math.Round(gAmp * Math.Sin(gPh + gFr * i)));
				var sB = Convert.ToInt32(Math.Round(bAmp * Math.Sin(bPh + bFr * i)));

				var c = Color.FromArgb((255 - rAmp) - sR, (255 - gAmp) - sG, (255 - bAmp) - sB);
				sb.Add(String.Format(ROW_TML, c.R, c.G, c.B));
			}			
		}

		private static void Direct(ICollection<string> sb)
		{
			for (var i = 0; i < 255; i += 10)
			{
				for (var k = 0; k < 255; k += 10)
				{
					for (var z = 0; z < 255; z += 10)
					{
						var c = Color.FromArgb(i, k, z);
						sb.Add(String.Format(ROW_TML, c.R, c.G, c.B));
					}
				}

			}
		}

		private const string ROW_TML = "<tr><td style='background: rgb({0}, {1}, {2})'>rgb({0}{1}{2})</td></tr>";

	}
}
