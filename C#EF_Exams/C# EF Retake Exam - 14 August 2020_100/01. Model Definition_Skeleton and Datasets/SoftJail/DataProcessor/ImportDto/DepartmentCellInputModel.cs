using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class DepartmentCellInputModel
    {     //•	Id – integer, Primary Key
        //•	CellNumber – integer in the range[1, 1000] (required)
        //•	HasWindow – bool (required)
        //•	DepartmentId - integer, foreign key(required)
        //•	Department – the cell's department (required)
        //•	Prisoners - collection of type Prisoner

        [Required]
        [StringLength(25, MinimumLength = 3)]
        public string Name { get; set; }

        public List<CellInputModel> Cells { get; set; }  //IEnumerable<CellInputModel>, CellinputModel[]
    }

    public class CellInputModel
    {
        [Range(1,1000)]
        public int CellNumber { get; set; }

        public bool HasWindow { get; set; }
    }

    //       {
    //         "Name": "CSS",
    //         "Cells": [
    //           {
    //             "CellNumber": 0,
    //             "HasWindow": true
    //           },
    //           {
    //         "CellNumber": 202,
    //             "HasWindow": false
    //           }
    //         ]
    //       }
}
