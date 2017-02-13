using System.Xml.Serialization;

namespace xml2rust.Data
{
	[XmlRoot(ElementName = "field")]
	public class Field
	{
		[XmlElement(ElementName = "access")]
		public string Access { get; set; }
		[XmlElement(ElementName = "bitOffset")]
		public string BitOffset { get; set; }
		[XmlElement(ElementName = "bitWidth")]
		public string BitWidth { get; set; }
		[XmlElement(ElementName = "description")]
		public string Description { get; set; }
		[XmlElement(ElementName = "name")]
		public string Name { get; set; }
	}
}