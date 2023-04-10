
namespace Artillery.DataProcessor
{
    using Artillery.Data;
    using Artillery.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using Theatre.DataProcessor;

    public class Serializer
    {
        public static string ExportShells(ArtilleryContext context, double shellWeight)
        {
            var shells = context.Shells
                .ToArray()
                .Where(s => s.ShellWeight > shellWeight)
                .Select(s => new
                {
                    ShellWeight = s.ShellWeight,
                    Caliber = s.Caliber,
                    Guns = s.Guns.Where(g => g.GunType.ToString() == "AntiAircraftGun").Select(g => new
                    {
                        GunType = g.GunType.ToString(),
                        GunWeight = g.GunWeight,
                        BarrelLength = g.BarrelLength,
                        Range = g.Range > 3000 ? "Long-range" : "Regular range"

                    })
                    .OrderByDescending(g => g.GunWeight)
                    .ToArray()
                })
                .OrderBy(s => s.ShellWeight)
                .ToArray();


            string jsonResult = JsonConvert.SerializeObject(shells, Formatting.Indented);

            return jsonResult;
        }

        public static string ExportGuns(ArtilleryContext context, string manufacturer)
        {
            var guns = context.Guns
                .Where(g => g.Manufacturer.ManufacturerName == manufacturer)
                .OrderBy(g => g.BarrelLength)
                .Select(g => new GunXmlExportModel
                {
                    BarrelLength = g.BarrelLength.ToString(),
                    GunType = g.GunType.ToString(),
                    GunWeight = g.GunWeight.ToString(),
                    Range = g.Range.ToString(),
                    Manufacturer = g.Manufacturer.ManufacturerName.ToString(),
                    Countries = g.CountriesGuns.Where(g => g.Country.ArmySize > 4500000)
                    .Select(c => new CountryXmlExportModel
                    {
                        ArmySize = c.Country.ArmySize.ToString(),
                        Country = c.Country.CountryName.ToString()
                    })
                    .OrderBy(c => c.ArmySize)
                    .ToArray()
                })
                .ToArray();

            var xml = XmlConverter.Serialize(guns, "Guns");
            return xml;
        }
    }
}
