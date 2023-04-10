namespace Artillery.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using Artillery.Data;
    using Artillery.Data.Models;
    using Artillery.DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using Artillery.Data.Models.Enums;

    public class Deserializer
    {
        private const string ErrorMessage =
                "Invalid data.";
        private const string SuccessfulImportCountry =
            "Successfully import {0} with {1} army personnel.";
        private const string SuccessfulImportManufacturer =
            "Successfully import manufacturer {0} founded in {1}.";
        private const string SuccessfulImportShell =
            "Successfully import shell caliber #{0} weight {1} kg.";
        private const string SuccessfulImportGun =
            "Successfully import gun {0} with a total weight of {1} kg. and barrel length of {2} m.";

        public static string ImportCountries(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var validCounties = new List<Country>();

            //var countries = XmlConverter.Deserializer<CountriesXmlImportModel>(xmlString, "Countries");
            var xmlSerializer = new XmlSerializer(
                typeof(CountriesXmlImportModel[]),
                new XmlRootAttribute("Countries"));
            var countries = (CountriesXmlImportModel[])xmlSerializer.Deserialize(
                new StringReader(xmlString));

            foreach (var currCountry in countries)
            {
                if (!IsValid(currCountry))
                {
                    sb.AppendLine($"Invalid data.");
                    continue;
                }

                Country country = new Country
                {
                    ArmySize = currCountry.ArmySize,
                    CountryName = currCountry.CountryName,
                };

                validCounties.Add(country);
                sb.AppendLine($"Successfully import {country.CountryName} with {country.ArmySize} army personnel.");
            }

            context.Countries.AddRange(validCounties);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
            // return "TODO";
        }

        public static string ImportManufacturers(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var validManufacturers = new List<Manufacturer>();

            //var countries = XmlConverter.Deserializer<CountriesXmlImportModel>(xmlString, "Countries");
            var xmlSerializer = new XmlSerializer(
                typeof(ManufacturerXmlImportModel[]),
                new XmlRootAttribute("Manufacturers"));
            var manufacrurers = (ManufacturerXmlImportModel[])xmlSerializer.Deserialize(
                new StringReader(xmlString));

            foreach (var currManufacturers in manufacrurers)
            {
                if (!IsValid(currManufacturers))
                {
                    sb.AppendLine($"Invalid data.");
                    continue;
                }

                if (context.Manufacturers.Any(m => m.ManufacturerName == currManufacturers.ManufacturerName)
                    || validManufacturers.Any(m => m.ManufacturerName == currManufacturers.ManufacturerName))
                {
                    sb.AppendLine($"Invalid data.");
                    continue;
                }

                Manufacturer manufacturer = new Manufacturer
                {
                    Founded = currManufacturers.Founded,
                    ManufacturerName = currManufacturers.ManufacturerName
                };

                //var manNames = String.Join(", ", manufacturer.Founded.Split(", ")
                //    .Where(t => !t.Any(char.IsDigit)));
                var manNames = manufacturer.Founded.Split(", ")
                 .Where(t => !t.Any(char.IsDigit)).ToArray();

                var country = manNames[manNames.Length - 1];
                var town = manNames[manNames.Length - 2];

                validManufacturers.Add(manufacturer);
                sb.AppendLine($"Successfully import manufacturer {manufacturer.ManufacturerName} founded in {town}, {country}.");
            }

            context.Manufacturers.AddRange(validManufacturers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
            // return "TODO";
        }

        public static string ImportShells(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var validShells = new List<Shell>();

            var xmlSerializer = new XmlSerializer(
                typeof(ShellXmlImportModel[]),
                new XmlRootAttribute("Shells"));
            var shells = (ShellXmlImportModel[])xmlSerializer.Deserialize(
                new StringReader(xmlString));

            foreach (var shellsItem in shells)
            {
                if (!IsValid(shellsItem))
                {
                    sb.AppendLine($"Invalid data.");
                    continue;
                }

                var shell = new Shell
                {
                    Caliber = shellsItem.Caliber,
                    ShellWeight = (double)shellsItem.ShellWeight,
                };

                validShells.Add(shell);

                sb.AppendLine($"Successfully import shell caliber #{shell.Caliber} weight {shell.ShellWeight} kg.");
            }

            context.Shells.AddRange(validShells);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
            // return "TODO";
        }

        public static string ImportGuns(ArtilleryContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var validGuns = new List<Gun>();

            var guns = JsonConvert.
                DeserializeObject<IEnumerable<GunJsonImortModel>>(jsonString);

            foreach (var currentGun in guns)
            {
                if (!IsValid(currentGun))
                {
                    sb.AppendLine($"Invalid data.");
                    continue;
                }

                var gun = new Gun
                {
                    GunWeight = currentGun.GunWeight,
                    ManufacturerId = currentGun.ManufacturerId,
                    Range = currentGun.Range,
                    BarrelLength = currentGun.BarrelLength,
                    GunType = Enum.Parse<GunType>(currentGun.GunType),
                    NumberBuild = currentGun.NumberBuild,
                    ShellId = currentGun.ShellId,
                    CountriesGuns = currentGun.Countries.Select(c => new CountryGun
                    {
                        CountryId = c.Id,
                    })
                    .ToArray()
                };

                validGuns.Add(gun);
                sb.AppendLine($"Successfully import gun {gun.GunType} with a total weight of {gun.GunWeight} kg. and barrel length of {gun.BarrelLength} m.");
            }
            context.Guns.AddRange(validGuns);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }
        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
