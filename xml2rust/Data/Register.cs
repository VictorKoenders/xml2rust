using System.Xml.Serialization;

namespace xml2rust.Data
{
	[XmlRoot(ElementName = "register")]
	public class Register
	{
		[XmlElement(ElementName = "access")]
		public string Access { get; set; }
		[XmlElement(ElementName = "addressOffset")]
		public string AddressOffset { get; set; }
		[XmlElement(ElementName = "alternateRegister")]
		public string AlternateRegister { get; set; }
		[XmlElement(ElementName = "description")]
		public string Description { get; set; }
		[XmlElement(ElementName = "displayName")]
		public string DisplayName { get; set; }
		[XmlElement(ElementName = "fields")]
		public Fields Fields { get; set; }
		[XmlElement(ElementName = "name")]
		public string Name { get; set; }
		[XmlElement(ElementName = "resetValue")]
		public string ResetValue { get; set; }
		[XmlElement(ElementName = "size")]
		public string Size { get; set; }
	}
}