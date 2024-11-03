using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.DTO.ImportDTO
{
    [XmlType("Part")]
    public class ImportPartDTO
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public string Price { get; set; }

        [XmlElement("quantity")]
        public int Quantity { get; set; }

        [XmlElement("supplierId")]
        public int SupplierId { get; set; }


        // <name>Unexposed bumper</name>
        //<price>1003.34</price>
        //<quantity>10</quantity>
        //<supplierId>12</supplierId>
    }
}
