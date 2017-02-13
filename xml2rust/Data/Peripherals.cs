using System.Collections.Generic;
using System.Xml.Serialization;

namespace xml2rust.Data
{
	[XmlRoot(ElementName = "peripherals")]
	public class Peripherals
	{
		[XmlElement(ElementName = "peripheral")]
		public List<Peripheral> Peripheral { get; set; }
	}
}