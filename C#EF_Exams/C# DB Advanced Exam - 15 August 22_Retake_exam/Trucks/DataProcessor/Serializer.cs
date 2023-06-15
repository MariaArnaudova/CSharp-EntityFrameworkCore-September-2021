namespace Trucks.DataProcessor
{
    using System;
    using System.Linq;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Theatre.DataProcessor;
    using Trucks.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportDespatchersWithTheirTrucks(TrucksContext context)
        {
            var despatchers = context.Despatchers.ToArray()
                .Where(d => d.Trucks.Count() >= 1)
                .Select(d => new DespatcherXmlModel
                {
                    DespatcherName = d.Name,
                    TrucksCount = d.Trucks.Count().ToString(),
                    Trucks = d.Trucks.Select(t => new TrucksViewModel
                    {
                        RegistrationNumber = t.RegistrationNumber,
                        Make = t.MakeType.ToString()
                    })
                    .OrderBy(t => t.RegistrationNumber)
                    .ToArray()
                })
                .OrderByDescending(d => d.Trucks.Count())
                .ThenBy(d => d.DespatcherName)
                .ToArray();


            var xml = XmlConverter.Serialize(despatchers, "Despatchers");
            return xml;
        }

        public static string ExportClientsWithMostTrucks(TrucksContext context, int capacity)
        {
            var clients = context.Clients
             .ToArray()
            // .Where( c => c.ClientsTrucks.All(t => t.Truck.TankCapacity >= capacity ))
            .Where(c => c.ClientsTrucks.Any(t => t.Truck.TankCapacity >= capacity))
            .Select(c => new
            {
                Name = c.Name,
                Trucks = c.ClientsTrucks.Where(t => t.Truck.TankCapacity >= capacity)
                .Select(t => new
                {
                    TruckRegistrationNumber = t.Truck.RegistrationNumber,
                    VinNumber = t.Truck.VinNumber,
                    TankCapacity = t.Truck.TankCapacity,
                    CargoCapacity = t.Truck.CargoCapacity,
                    CategoryType = t.Truck.CategoryType.ToString(),
                    MakeType = t.Truck.MakeType.ToString()
                })
                  .OrderBy(t => t.MakeType)
                  .ThenByDescending(t => t.CargoCapacity)
                  .ToArray()
            })
              .OrderByDescending(c => c.Trucks.Count())
              .ThenBy(c => c.Name)
              .ToArray()
              .Take(10);

            string jsonResult = JsonConvert.SerializeObject(clients, Formatting.Indented);

            return jsonResult;
        }
    }
}
