using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Trucks.Data.Models;

namespace Trucks.DataProcessor.ImportDto
{
    public class ClientsJsonImportModel
    {

        [Required]
        [StringLength(40, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [StringLength(40, MinimumLength = 2)]
        public string Nationality { get; set; }

        [Required]
        public string Type { get; set; }

        public List<int> Trucks { get; set; }


        //•	Id – integer, Primary Key
        //•	Name – text with length[3, 40] (required)
        //•	Nationality – text with length[2, 40] (required)
        //•	Type – text(required)
        //•	ClientsTrucks – collection of type ClientTruck
    }

    //public class ClientTruckInputModel
    //{
     
    //    [Required]
    //    public int TruckId { get; set; }
    //}
}
