namespace Invoices.DataProcessor
{
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.DataProcessor.ExportDto;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Internal;
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.Linq;
    using Theatre.DataProcessor;

    public class Serializer
    {
        public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
        {
            var clients = context.Clients
                .Include(c => c.Invoices)
                .ToArray()
                .Where(c => c.Invoices.Any(i => i.IssueDate >= date))
                .Select(c => new ClientsXmlExportModelWithTheirInvoices
                {
                    InvoicesCount = c.Invoices.Count(),
                    VatNumber = c.NumberVat,
                    ClientName = c.Name,
                    Invoices = c.Invoices
                    .OrderBy(i => i.IssueDate)
                    .ThenByDescending(i => i.DueDate)
                    .Select(i => new XmlExportInvoices
                    {
                        InvoiceNumber = i.Number.ToString(),
                        InvoiceAmount = i.Amount,
                        //DueDate = DateTime.ParseExact(i.DueDate.ToString(), "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture) .ToString("MM/dd/yyyy"),
                        DueDate = i.DueDate.ToString("MM/dd/yyyy"),
                        Currency = i.CurrencyType.ToString(),
                    })
                    .ToArray()
                })
                .OrderByDescending(c => c.InvoicesCount)
                .ThenBy(c=> c.ClientName)
                .ToArray();

            var xml = XmlConverter.Serialize(clients, "Clients");
            return xml;
            //throw new NotImplementedException();
        }

        public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
        {

            var products = context.Products
        .Include(p => p.ProductsClients)
        .ThenInclude(p => p.Client)
        .ToArray()
        .Where(p => p.ProductsClients.Any(c => c.Client.Name.Length >= nameLength))
        //.Where(p => p.ProductsClients.Any(c => c.Client.Name.Length >= p.ProductsClients.Select(p => p.ProductId).Count()))
        .Select(p => new
        {
            Name = p.Name,
            Price = p.Price,
            Category = p.CategoryType.ToString(),
            Clients = p.ProductsClients
            .Where(c => c.Client.Name.Length >= nameLength)
            .Select(c => new
            {
                Name = c.Client.Name,
                NumberVat = c.Client.NumberVat,
            })
           .ToArray()
            .OrderBy(c => c.Name)

        })
        .OrderByDescending(p => p.Clients.Count())
                            .ThenBy(p => p.Name)
                            .Take(5)
                            .ToArray();

            string jsonResult = JsonConvert.SerializeObject(products, Formatting.Indented);

            return jsonResult;
        }
    }
}