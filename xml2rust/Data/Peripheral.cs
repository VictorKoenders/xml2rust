using System.Collections.Generic;
using System.Xml.Serialization;

namespace xml2rust.Data
{
	[XmlRoot(ElementName = "peripheral")]
	public class Peripheral
	{
		[XmlElement(ElementName = "addressBlock")]
		public AddressBlock AddressBlock { get; set; }
		[XmlElement(ElementName = "baseAddress")]
		public string BaseAddress { get; set; }
		[XmlAttribute(AttributeName = "derivedFrom")]
		public string DerivedFrom { get; set; }
		[XmlElement(ElementName = "description")]
		public string Description { get; set; }
		[XmlElement(ElementName = "groupName")]
		public string GroupName { get; set; }
		[XmlElement(ElementName = "interrupt")]
		public List<Interrupt> Interrupt { get; set; }
		[XmlElement(ElementName = "name")]
		public string Name { get; set; }
		[XmlElement(ElementName = "registers")]
		public Registers Registers { get; set; }
	}
}