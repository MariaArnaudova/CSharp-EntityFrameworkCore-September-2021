using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType("Shell")]
    public class ShellXmlImportModel
    {

        [Required]
        [Range(2,1_680)]
        [XmlElement("ShellWeight")]
        public double ShellWeight { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 4)]
        [XmlElement("Caliber")]
        public string Caliber { get; set; }

        //•	Id – integer, Primary Key
        //•	ShellWeight – double in range[2…1_680] (required)
        //•	Caliber – text with length[4…30] (required)
        //•	Guns – a collection of Gun
    }
}
