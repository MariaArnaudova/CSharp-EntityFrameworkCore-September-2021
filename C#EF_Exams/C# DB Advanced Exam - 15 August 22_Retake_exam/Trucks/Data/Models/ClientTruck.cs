using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Trucks.Data.Models
{
    public class ClientTruck
    {
        [Required]
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public Client Client { get; set; }

        [Required]
        [ForeignKey("Truck")]
        public int TruckId { get; set; }
        public Truck Truck { get; set; }
        // •	ClientId – integer, Primary Key, foreign key(required)
        //•	Client – Client
        //•	TruckId – integer, Primary Key, foreign key(required)
        //•	Truck – Truck

    }
}
