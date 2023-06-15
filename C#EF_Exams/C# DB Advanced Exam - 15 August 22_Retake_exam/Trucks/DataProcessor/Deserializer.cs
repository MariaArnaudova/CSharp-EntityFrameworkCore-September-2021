namespace Trucks.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using Theatre.DataProcessor;
    using Trucks.Data.Models;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var validDispatchers = new List<Despatcher>();

            var dispatchers = XmlConverter.Deserializer<DespatchersXmlImportModel>(xmlString, "Despatchers");

            foreach (var currentDispatcher in dispatchers)
            {
                if (!IsValid(currentDispatcher))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                if (currentDispatcher.Position == null || currentDispatcher.Position == "")
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                var despatcher = new Despatcher
                {
                    Name = currentDispatcher.Name,
                    Position = currentDispatcher.Position,
                };

                foreach (var currTruck in currentDispatcher.Trucks)
                {
                    if (!IsValid(currTruck))
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }

                    var truck = new Truck
                    {
                        RegistrationNumber = currTruck.RegistrationNumber,
                        VinNumber = currTruck.VinNumber,
                        CargoCapacity = (int)currTruck.CargoCapacity,
                        TankCapacity = (int)currTruck.TankCapacity,
                        CategoryType = Enum.Parse<CategoryType>(currTruck.CategoryType),
                        MakeType = Enum.Parse<MakeType>(currTruck.MakeType)
                    };

                    despatcher.Trucks.Add(truck);
                }
                    validDispatchers.Add(despatcher);
                    sb.AppendLine($"Successfully imported despatcher - {despatcher.Name} with {despatcher.Trucks.Count} trucks.");
            }

            context.AddRange(validDispatchers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var validClients = new List<Client>();

            var clients = JsonConvert.
                DeserializeObject<IEnumerable<ClientsJsonImportModel>>(jsonString);

            foreach (var currentClient in clients)
            {
                if (!IsValid(currentClient))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                if (currentClient.Type == "usual")
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                var client = new Client
                {
                    Name = currentClient.Name,
                    Nationality = currentClient.Nationality,
                    Type = currentClient.Type,
                };
                context.Add(client);
                context.SaveChanges();

                var uniqueTucksId = currentClient.Trucks.Distinct();

                foreach (var currTruck in uniqueTucksId)
                {
                    var isContainsCurrenrTuck = context.Trucks.Any(t => t.Id == currTruck);

                    if (!isContainsCurrenrTuck)
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }

                    var clientId = client.Id;

                    client.ClientsTrucks.Add(new ClientTruck
                    {
                        ClientId = clientId,
                        TruckId = currTruck
                    });

                    context.SaveChanges();
                };

                sb.AppendLine($"Successfully imported client - {client.Name} with {client.ClientsTrucks.Count} trucks.");
            }

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
