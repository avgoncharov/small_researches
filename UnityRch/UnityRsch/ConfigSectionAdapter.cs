using System.IO;
using System.Xml;
using Microsoft.Practices.Unity.Configuration;

namespace UnityRsch
{
	/// <summary>
	/// Adapter for createing section from xml.
	/// </summary>
	public class ConfigSectionAdapter : UnityConfigurationSection
	{
		public void LoadFromString(string str)
		{
			using (var r = new StringReader(File.ReadAllText("AddToContainer.xml")))
			{

				var xmlr = XmlReader.Create(r);
				DeserializeSection(xmlr);
			}
		}
	}
}
