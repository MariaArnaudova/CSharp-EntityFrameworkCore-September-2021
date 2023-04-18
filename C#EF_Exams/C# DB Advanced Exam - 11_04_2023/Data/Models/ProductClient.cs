using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

namespace Invoices.Data.Models
{
    public class ProductClient
    {
        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public Client Client { get; set; }
    }

//    •	ProductId – integer, Primary Key, foreign key(required)
//•	Product – Product
//•	ClientId – integer, Primary Key, foreign key(required)
//•	Client – Client

}
