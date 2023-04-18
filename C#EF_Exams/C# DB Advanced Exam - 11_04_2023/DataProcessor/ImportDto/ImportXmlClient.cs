using Invoices.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType("Client")]
    public class ImportXmlClient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [XmlElement("Name")]
        [StringLength(25, MinimumLength = 10)]
        public string Name { get; set; }

        [Required]
        [XmlElement("NumberVat")]
        [StringLength(15, MinimumLength = 10)]
        public string NumberVat { get; set; }

        [XmlArray("Addresses")]
        public AddressXlmInputModel[] Addresses { get; set; }
    }


    //    •	Id – integer, Primary Key
    //•	Name – text with length[10…25] (required)
    //•	NumberVat – text with length[10…15] (required)
    //•	Invoices – collection of type Invoicе
    //•	Addresses – collection of type Address
    //•	ProductsClients – collection of type ProductClient

    [XmlType("Address")]
    public class AddressXlmInputModel
    {


        [Required]
        [XmlElement("StreetName")]
        [StringLength(20, MinimumLength = 10)]
        public string StreetName { get; set; }

        [Required]
        [XmlElement("StreetNumber")]
        public int StreetNumber { get; set; }

        [Required]
        [XmlElement("PostCode")]
        public string PostCode { get; set; }

        [Required]
        [XmlElement("City")]
        [StringLength(15, MinimumLength = 5)]
        public string City { get; set; }

        [Required]
        [XmlElement("Country")]
        [StringLength(15, MinimumLength = 5)]
        public string Country { get; set; }

    }


//    •	Id – integer, Primary Key
//•	StreetName – text with length[10…20] (required)
//•	StreetNumber – integer(required)
//•	PostCode – text(required)
//•	City – text with length[5…15] (required)
//•	Country – text with length[5…15] (required)
//•	ClientId – integer, foreign key(required)
//•	Client – Client

}
