using Invoices.Data.Models.Enums;
using Invoices.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportInvoicesJson
    {


        [Required]
        [Range(1_000_000_000, 1_500_000_000)]
        public int Number { get; set; }

        [Required]
        public string IssueDate { get; set; }

        [Required]
        public string DueDate { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public CurrencyType CurrencyType { get; set; }

        [Required]
        public int ClientId { get; set; }

    }

    //    •	Id – integer, Primary Key
    //•	Number – integer in range[1, 000, 000, 000…1, 500, 000, 000] (required)
    //•	IssueDate – DateTime(required)
    //•	DueDate – DateTime(required)
    //•	Amount – decimal (required)
    //•	CurrencyType – enumeration of type CurrencyType, with possible values(BGN, EUR, USD) (required)
    //•	ClientId – integer, foreign key(required)
    //•	Client – Client

}
