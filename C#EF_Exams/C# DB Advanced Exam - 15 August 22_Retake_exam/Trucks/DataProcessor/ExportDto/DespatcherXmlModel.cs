using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Trucks.DataProcessor.ExportDto
{
    [XmlType("Despatcher")]
    public class DespatcherXmlModel
    {
        [Required]
        [XmlElement("DespatcherName")]
        public string DespatcherName { get; set; }

        [XmlAttribute("TrucksCount")]
        public string TrucksCount { get; set; }

        [XmlArray("Trucks")]
        public TrucksViewModel[] Trucks { get; set; }
    }

    [XmlType("Truck")]
    public class TrucksViewModel
    {
 
        [XmlElement("RegistrationNumber")]
        public string RegistrationNumber { get; set; }

        [XmlElement("Make")]
        public string Make { get; set; }

    }
}
