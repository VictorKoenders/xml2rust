using System.Xml.Serialization;

namespace xml2rust.Data
{
	[XmlRoot(ElementName = "device")]
	public class Device
	{
		[XmlElement(ElementName = "addressUnitBits")]
		public string AddressUnitBits { get; set; }
		[XmlElement(ElementName = "description")]
		public string Description { get; set; }
		[XmlElement(ElementName = "name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName = "noNamespaceSchemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
		public string NoNamespaceSchemaLocation { get; set; }
		[XmlElement(ElementName = "peripherals")]
		public Peripherals Peripherals { get; set; }
		[XmlElement(ElementName = "resetMask")]
		public string ResetMask { get; set; }
		[XmlElement(ElementName = "resetValue")]
		public string ResetValue { get; set; }
		[XmlAttribute(AttributeName = "schemaVersion")]
		public string SchemaVersion { get; set; }
		[XmlElement(ElementName = "size")]
		public string Size { get; set; }
		[XmlElement(ElementName = "version")]
		public string Version { get; set; }
		[XmlElement(ElementName = "width")]
		public string Width { get; set; }
		[XmlAttribute(AttributeName = "xs", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Xs { get; set; }
	}
}