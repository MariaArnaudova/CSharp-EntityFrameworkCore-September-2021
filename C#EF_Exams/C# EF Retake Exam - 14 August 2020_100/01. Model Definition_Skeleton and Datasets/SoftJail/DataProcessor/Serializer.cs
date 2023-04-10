namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Globalization;
    using System.Linq;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context.Prisoners
                .Where(p => ids.Contains(p.Id))
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers.Select(o => new
                    {
                        OfficerName = o.Officer.FullName,
                        Department = o.Officer.Department.Name
                    })
                    .OrderBy(p => p.OfficerName)
                    .ToList(),
                    TotalOfficerSalary = decimal.Parse(p.PrisonerOfficers.Sum(po => po.Officer.Salary).ToString("F2"))
                })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToList();

            string result = JsonConvert.SerializeObject(prisoners, Formatting.Indented);
            return result;

        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            // Melanie Simonich,Diana Ebbs,Binni Cornhill
            var names = prisonersNames.Split(',',
                StringSplitOptions.RemoveEmptyEntries);

            var result = context.Prisoners
                .Where(x => names.Contains(x.FullName))
                .Select(x => new PrisonerViewModel
                {
                    Id = x.Id,
                    Name = x.FullName,
                    IncarcerationDate = x.IncarcerationDate.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture),
                    EncryptedMessages = x.Mails.Select(m => new EncryptedMessagesViewModel
                    {
                        Description = string.Join("", m.Description.Reverse())
                    })
                    .ToArray()

                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToList();

            var resultXml = XmlConverter.Serialize(result, "Prisoners");

            return resultXml;
        }
    }
}