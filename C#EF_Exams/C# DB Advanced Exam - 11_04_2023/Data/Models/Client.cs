using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Invoices.Data.Models
{
    public class Client
    {
        public Client()
        {
            this.ProductsClients = new HashSet<ProductClient>();
            this.Invoices = new HashSet<Invoice>();
            this.Addresses = new HashSet<Address>();
        }


        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string NumberVat { get; set; }
        public ICollection<Invoice> Invoices { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<ProductClient> ProductsClients { get; set; }

    }

    //    •	Id – integer, Primary Key
    //•	Name – text with length[10…25] (required)
    //•	NumberVat – text with length[10…15] (required)
    //•	Invoices – collection of type Invoicе
    //•	Addresses – collection of type Address
    //•	ProductsClients – collection of type ProductClient

}
