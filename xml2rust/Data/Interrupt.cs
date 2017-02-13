using System.Xml.Serialization;

namespace xml2rust.Data
{
	[XmlRoot(ElementName = "interrupt")]
	public class Interrupt
	{
		[XmlElement(ElementName = "description")]
		public string Description { get; set; }
		[XmlElement(ElementName = "name")]
		public string Name { get; set; }
		[XmlElement(ElementName = "value")]
		public string Value { get; set; }
	}
}