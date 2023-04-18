using Invoices.Data.Models.Enums;
using Invoices.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportProductsJson
    {

        [Required]
        [StringLength(30, MinimumLength = 9)]
        public string Name { get; set; }

        [Required]
        [Range(typeof(decimal), "5.00", "1000.00")]
        public decimal Price { get; set; }

        [Required]
        public CategoryType CategoryType { get; set; }

        public List<Client> Clients { get; set; }

    }

//    •	Id – integer, Primary Key
//•	Name – text with length[9…30] (required)
//•	Price – decimal in range[5.00…1000.00] (required)
//•	CategoryType – enumeration of type CategoryType, with possible values(ADR, Filters, Lights, Others, Tyres) (required)
//•	ProductsClients – collection of type ProductClient

}
