using System.Xml.Serialization;

namespace CarDealer.DTO.ExportDTO
{
    [XmlType("car")]
    public class CarSaleOutputMpdel
    {

        [XmlAttribute("make")]
        public string Make { get; set; }

        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("travelled-distance")]
        public long TravellDistance { get; set; }
    }
}
