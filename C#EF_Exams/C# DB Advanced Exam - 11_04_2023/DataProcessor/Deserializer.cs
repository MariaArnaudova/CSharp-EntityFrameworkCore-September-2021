namespace Invoices.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.Data.Models.Enums;
    using Invoices.DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using Theatre.DataProcessor;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedClients
            = "Successfully imported client {0}.";

        private const string SuccessfullyImportedInvoices
            = "Successfully imported invoice with number {0}.";

        private const string SuccessfullyImportedProducts
            = "Successfully imported product - {0} with {1} clients.";


        public static string ImportClients(InvoicesContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var validClient = new List<Client>();
            var validAddress = new List<Address>();
            var clients = XmlConverter.Deserializer<ImportXmlClient>(xmlString, "Clients");

            foreach (var currClient in clients)
            {
                if (!IsValid(currClient))
                {
                    sb.AppendLine("Invalid data!");

                    continue;
                }

                var client = new Client
                {
                    Name = currClient.Name,
                    NumberVat = currClient.NumberVat,
                };

                foreach (var currAddress in currClient.Addresses)
                {
                    var address = new Address
                    {
                        City = currAddress.City,
                        Country = currAddress.Country,
                        StreetName = currAddress.StreetName,
                        PostCode = currAddress.PostCode,
                        StreetNumber = currAddress.StreetNumber,
                        Client = client,
                        ClientId = client.Id

                    };

                    if (!IsValid(currAddress))
                    {
                        sb.AppendLine("Invalid data!");
                        validAddress.Add(address);
                        continue;
                    }

                    client.Addresses.Add(address);

                    validClient.Add(client);

                }

                sb.AppendLine($"Successfully imported client {currClient.Name}.");
            }

            context.AddRange(validAddress);
            context.SaveChanges();
            context.AddRange(validClient);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }


        public static string ImportInvoices(InvoicesContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var validInvoices = new List<Invoice>();

            var invoices = JsonConvert.
                DeserializeObject<IEnumerable<ImportInvoicesJson>>(jsonString);

            foreach (var invoice in invoices)
            {

                if (!IsValid(invoice))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                DateTime resultData;
                DateTime resultIssue;
                CurrencyType currType;

                var currDueData = DateTime.TryParseExact(invoice.DueDate, "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out resultData);
                var currIssueData = DateTime.TryParseExact(invoice.IssueDate, "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out resultIssue);
                //var currDueData = DateTime.TryParse(invoice.DueDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out resultData);
                //var currIssueData = DateTime.TryParse(invoice.IssueDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out resultIssue);
                var validType = Enum.TryParse<CurrencyType>(invoice.CurrencyType.ToString(), true, out currType);

                if(!context.Clients.Any(c => c.Id == invoice.ClientId))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                if (!currDueData || !currIssueData || !validType)
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                if (resultData <= resultIssue)
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                var importInvoice = new Invoice
                {
                    Amount = invoice.Amount,
                    CurrencyType = currType,
                    DueDate = resultData,
                    IssueDate = resultIssue,
                    Number = invoice.Number,
                    ClientId = (int)invoice.ClientId,
                };

                validInvoices.Add(importInvoice);

                sb.AppendLine($"Successfully imported invoice with number {importInvoice.Number}.");
            }

            context.AddRange(validInvoices);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportProducts(InvoicesContext context, string jsonString)
        {

            var sb = new StringBuilder();
            var validProducts = new List<Product>();

            var products = JsonConvert.
                DeserializeObject<IEnumerable<ImportProductsJson>>(jsonString);


            foreach (var product in products)
            {
                if (!IsValid(product))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                if (!IsValid(product.Clients))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                var uniqueClients = product.Clients.Distinct().ToList();

                CategoryType categoryType;
                var validType = Enum.TryParse<CategoryType>(product.CategoryType.ToString(), true, out categoryType);

                if (!validType)
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                var newPorduct = new Product
                {
                    CategoryType = categoryType,
                    Name = product.Name,
                    Price = (decimal)product.Price,
                };

                var count = 0;
                foreach (var client in uniqueClients)
                {

                    if (!context.Clients.Any(c => c.Id == client))
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }
                    count++;

                    newPorduct.ProductsClients.Add(new ProductClient
                    {
                        ClientId = client,
                    });


                };

                validProducts.Add(newPorduct);
                sb.AppendLine($"Successfully imported product - {product.Name} with {count} clients.");

            }
            context.AddRange(validProducts);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
