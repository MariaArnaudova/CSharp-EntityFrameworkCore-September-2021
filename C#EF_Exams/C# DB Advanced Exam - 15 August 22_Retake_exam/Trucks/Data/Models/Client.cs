using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Trucks.Data.Models
{
    public class Client
    {
        public Client()
        {
            this.ClientsTrucks = new HashSet<ClientTruck>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Nationality { get; set; }

        [Required]
        public string Type { get; set; }

        public ICollection<ClientTruck> ClientsTrucks { get; set; }

        //•	Id – integer, Primary Key
        //•	Name – text with length[3, 40] (required)
        //•	Nationality – text with length[2, 40] (required)
        //•	Type – text(required)
        //•	ClientsTrucks – collection of type ClientTruck

    }
}
