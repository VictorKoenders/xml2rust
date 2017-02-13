using System.Xml;
using System.Xml.Serialization;
using xml2rust.Data;

namespace xml2rust
{
	public class Parser
	{
		public Parser(string file)
		{
			using (XmlReader reader = XmlReader.Create(file))
			{
				Device = (Device)new XmlSerializer(typeof(Device)).Deserialize(reader);
			}
		}

		public readonly Device Device;
	}
}