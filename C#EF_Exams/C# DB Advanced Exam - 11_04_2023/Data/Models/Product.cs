using Invoices.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Invoices.Data.Models
{
    public class Product
    {
        public Product()
        {
            this.ProductsClients = new HashSet<ProductClient>();
        }

        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public CategoryType CategoryType { get; set; }
        public ICollection<ProductClient> ProductsClients { get; set; }

    }



    //        •	Id – integer, Primary Key
    //•	Name – text with length[9…30] (required)
    //•	Price – decimal in range[5.00…1000.00] (required)
    //•	CategoryType – enumeration of type CategoryType, with possible values(ADR, Filters, Lights, Others, Tyres) (required)
    //•	ProductsClients – collection of type ProductClient
}
