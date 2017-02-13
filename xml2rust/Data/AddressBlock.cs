/* 
 Licensed under the Apache License, Version 2.0

 http://www.apache.org/licenses/LICENSE-2.0
 */

using System.Xml.Serialization;

namespace xml2rust.Data
{
	[XmlRoot(ElementName = "addressBlock")]
	public class AddressBlock
	{
		[XmlElement(ElementName = "offset")]
		public string Offset { get; set; }
		[XmlElement(ElementName = "size")]
		public string Size { get; set; }
		[XmlElement(ElementName = "usage")]
		public string Usage { get; set; }
	}
}
