using System.Collections.Generic;
using System.Xml.Serialization;

namespace xml2rust.Data
{
	[XmlRoot(ElementName = "fields")]
	public class Fields
	{
		[XmlElement(ElementName = "field")]
		public List<Field> Field { get; set; }
	}
}