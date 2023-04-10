using Artillery.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType("Country")]
    public class CountriesXmlImportModel
    {
      
        [Required]
        [XmlElement("CountryName")]
        [StringLength(60, MinimumLength = 4)]
        public string CountryName { get; set; }

        [Required]
        [XmlElement("ArmySize")]
        [Range(50_000, 10_000_000)]
        public int ArmySize { get; set; }

        //•	Id – integer, Primary Key
        //•	CountryName – text with length[4, 60] (required)
        //•	ArmySize – integer in the range[50_000….10_000_000] (required)
        //•	CountriesGuns – a collection of CountryGun

    }
}
