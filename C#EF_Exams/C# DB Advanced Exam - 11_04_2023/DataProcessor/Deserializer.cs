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

                //var currDueData = DateTime.ParseExact(invoice.DueDate, "yyyyMMddTHH:mm:ss", CultureInfo.InvariantCulture);
                var currDueData = DateTime.TryParse(invoice.DueDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out resultData);
                var currIssueData = DateTime.TryParse(invoice.IssueDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out resultIssue);
                var validType = Enum.TryParse<CurrencyType>( invoice.CurrencyType.ToString(), true, out currType);

                if (!currDueData || !currIssueData ||!validType)
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
                    IssueDate = resultData,
                    Number = invoice.Number,
                    ClientId = invoice.ClientId,
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

                var uniqueClients = product.Clients.Distinct().ToList();

                var newPorduct = new Product
                {
                    CategoryType = product.CategoryType,
                    Name = product.Name,
                    Price = (decimal)product.Price,
                };

                var cout = 0;
                foreach (var client in uniqueClients)
                {

                    if (!context.Clients.Any(c => c.Id == client.Id))
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }
                    cout++;

                    newPorduct.ProductsClients.Add(new ProductClient
                    {
                        ClientId = client.Id,
                    });

                    validProducts.Add(newPorduct);

                };
                sb.AppendLine($"Successfully imported product - {product.Name} with {cout} clients.");

                context.SaveChanges();

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
