namespace Invoices.DataProcessor
{
    using Invoices.Data;
    using Invoices.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Internal;
    using Newtonsoft.Json;
    using System;
    using System.Linq;

    public class Serializer
    {
        public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
        {


            throw new NotImplementedException();
        }

        public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
        {

            var products = context.Products
        .Include(p => p.ProductsClients)
        .ThenInclude(p => p.Client)
        .ToArray()
        //.Where(p => p.ProductsClients.Select(p => p.ProductId).Count() >= 1)

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