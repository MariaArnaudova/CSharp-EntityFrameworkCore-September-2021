namespace Invoices.DataProcessor
{
    using Invoices.Data;
    using Invoices.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System;
    using System.Linq;

    public class Serializer
    {
        public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
        {
            var products = context.Products
                .Include(p => p.ProductsClients)
                .ThenInclude(p => p.Client)
                .ToArray()
                .Where(p => p.ProductsClients.Any(c => c.Client.Name.Length >= 5))
                .Select(p => new
                {
                    Name = p.Name,
                    Price = p.Price,
                    Category = p.CategoryType.ToString(),
                    Clients = p.ProductsClients
                    .Where(c => c.Client.Name.Length >= 5)
                    .Select(c => new
                    {
                        Name = c.Client.Name,
                        NumberVat = c.Client.NumberVat,
                    })
                    .OrderBy(c => c.Name)
                   .ToList()

                })
                .OrderByDescending(p => p.Clients.Count())
                                    .ThenBy(p => p.Name)
                                    .ToList()
                                    .Take(5);

            string jsonResult = JsonConvert.SerializeObject(products, Formatting.Indented);

            return jsonResult;
        }

        public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
        {

            throw new NotImplementedException();
        }
    }
}