using System.Collections.Generic;
using System.Xml.Serialization;

namespace xml2rust.Data
{
	[XmlRoot(ElementName = "registers")]
	public class Registers
	{
		[XmlElement(ElementName = "register")]
		public List<Register> Register { get; set; }
	}
}