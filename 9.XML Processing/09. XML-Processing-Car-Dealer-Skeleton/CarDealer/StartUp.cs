using CarDealer.Data;
using CarDealer.DTO.ExportDTO;
using CarDealer.DTO.ImportDTO;
using CarDealer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            CarDealerContext carDealerContext = new CarDealerContext();

            //carDealerContext.Database.EnsureDeleted();
            //carDealerContext.Database.EnsureCreated();
            //Console.WriteLine("CarContext reset succses.");

            //string inputXmlSupplier = File.ReadAllText("../../../Datasets/suppliers.xml");
            //string inputXmlParts = File.ReadAllText("../../../Datasets/parts.xml");
            //string inputXmlCars = File.ReadAllText("../../../Datasets/cars.xml");
            //string inputXmlCustomers = File.ReadAllText("../../../Datasets/customers.xml");
            //string inputXmlSales = File.ReadAllText("../../../Datasets/sales.xml");
            //string resultSuppliers = ImportSuppliers(carDealerContext, inputXmlSupplier);
            //string resultParts = ImportParts(carDealerContext, inputXmlParts);
            //string resultCars = ImportCars(carDealerContext, inputXmlCars);
            //string resultCustomers = ImportCustomers(carDealerContext, inputXmlCustomers);
            //string resultSales = ImportSales(carDealerContext, inputXmlSales);

            //Console.WriteLine(resultSuppliers);
            //Console.WriteLine(resultParts);
            //Console.WriteLine(resultCars);
            //Console.WriteLine(resultCustomers);
            //Console.WriteLine(resultSales);

            //string resultGetCarsWithDistance = GetCarsWithDistance(carDealerContext);
            // string resultCarsFromMakeBmw = GetCarsFromMakeBmw(carDealerContext);
            // string resultSuppliers = GetLocalSuppliers(carDealerContext);
            // string reaultcarsWithTheirListOfParts = GetCarsWithTheirListOfParts(carDealerContext);
            //string resulttotalSalesByCustomer = GetTotalSalesByCustomer(carDealerContext);
            string resulltSalesWithAppliedDiscount = GetSalesWithAppliedDiscount(carDealerContext);

             Console.WriteLine(resulltSalesWithAppliedDiscount);
        }
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Suppliers");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSupplierDTO[]), xmlRoot);

            using StringReader stringReader = new StringReader(inputXml);

            ImportSupplierDTO[] dtos = xmlSerializer.Deserialize(stringReader) as ImportSupplierDTO[];
            //ImportSupplierDTO[] dtos = (ImportSupplierDTO[]) xmlSerializer.Deserialize(stringReader);

            ICollection<Supplier> suppliers = new HashSet<Supplier>();

            foreach (ImportSupplierDTO supplierDto in dtos)
            {

                Supplier supl = new Supplier
                {
                    Name = supplierDto.Name,
                    IsImporter = bool.Parse(supplierDto.IsImporter)
                };

                suppliers.Add(supl);
            }

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }
        // 10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Parts");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPartDTO[]), xmlRoot);

            using StringReader stringReader = new StringReader(inputXml);

            ImportPartDTO[] partsDTO = (ImportPartDTO[])xmlSerializer.Deserialize(stringReader);

            ICollection<Part> parts = new HashSet<Part>();


            foreach (ImportPartDTO partDTO in partsDTO)
            {

                Supplier supplier = context
                    .Suppliers
                    .Find(partDTO.SupplierId);

                if (supplier == null)
                {
                    continue;
                }

                Part part = new Part
                {

                    Name = partDTO.Name,
                    Price = decimal.Parse(partDTO.Price),
                    Quantity = partDTO.Quantity,
                    SupplierId = partDTO.SupplierId

                };

                parts.Add(part);
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        // 11. Import Cars 
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Cars");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCarDTO[]), xmlRoot);

            using StringReader stringReader = new StringReader(inputXml);

            ImportCarDTO[] carsDto = (ImportCarDTO[])xmlSerializer.Deserialize(stringReader);

            ICollection<Car> cars = new HashSet<Car>();
            ICollection<PartCar> currentPartCars = new HashSet<PartCar>();

            foreach (ImportCarDTO carDto in carsDto)
            {

                var distinctedParts = carDto.Parts.Select(p => p.Id).Distinct();

                Car car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TraveledDistance,
                };

                // ICollection<PartCar> currentPartCars = new HashSet<PartCar>();

                foreach (int partId in distinctedParts)
                {
                    Part part = context
                        .Parts
                        .Find(partId);

                    if (part == null)
                    {
                        continue;
                    }

                    PartCar partCar = new PartCar
                    {
                        Car = car,
                        PartId = partId
                    };

                    currentPartCars.Add(partCar);
                }

                cars.Add(car);
            }
            context.PartCars.AddRange(currentPartCars);
            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        // 12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Customers");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomersDto[]), xmlRoot);

            using StringReader stringReader = new StringReader(inputXml);

            ImportCustomersDto[] customersDTO = (ImportCustomersDto[])xmlSerializer.Deserialize(stringReader);

            ICollection<Customer> customers = new HashSet<Customer>();

            foreach (var customer in customersDTO)
            {
                Customer cust = new Customer
                {

                    Name = customer.Name,
                    BirthDate = customer.BirthDate,
                    IsYoungDriver = customer.IsYoungDriver

                };
                customers.Add(cust);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        // Query 13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Sales");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSalesDto[]), xmlRoot);

            using StringReader stringReader = new StringReader(inputXml);

            ImportSalesDto[] salesDTO = (ImportSalesDto[])xmlSerializer.Deserialize(stringReader);

            var carsId = context.Cars.Select(c => c.Id).ToList();

            var sales = salesDTO
              .Where(s => carsId.Contains(s.CarId))
              .Select(s => new Sale
              {
                  CarId = s.CarId,
                  CustomerId = s.CustomerId,
                  Discount = s.Discount
              })
              .ToList();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        // Query 14. Cars With Distance
        public static string GetCarsWithDistance(CarDealerContext context)
        {

            ExportCarWithDistance[] cars = context.Cars
                .Where(c => c.TravelledDistance > 2_000_000)
                .Select(c => new ExportCarWithDistance
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ToArray();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("cars");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCarWithDistance[]), xmlRoot);

            XmlSerializerNamespaces nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add(string.Empty, string.Empty);

            using StringWriter textWriter = new StringWriter();

            xmlSerializer.Serialize(textWriter, cars, nameSpaces);

            string result = textWriter.ToString();
            return result;
        }

        // Query 15. Cars from make BMW
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            
            var carsBmv = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new ExportBMVOutputModel
                {
                    Model = c.Model,
                    Id = c.Id,
                    TravelledDistance = c.TravelledDistance
                })
                .ToArray();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("cars");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportBMVOutputModel[]), xmlRoot);

            XmlSerializerNamespaces nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add(string.Empty, string.Empty);

            using StringWriter textWriter = new StringWriter();

            xmlSerializer.Serialize(textWriter, carsBmv, nameSpaces);

            string result = textWriter.ToString();
            return result;
        }

        // 16. Export Local Suppliers 
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            // Get all suppliers that do not import parts from abroad. Get their id,
            //name and the number of parts they can offer to supply. 

            var suppliers = context.Suppliers
                .Where(s => s.IsImporter != true)
                .Select(s => new ExportSuppliers
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count()
                })
                .ToArray();


            XmlRootAttribute xmlRoot = new XmlRootAttribute("suppliers");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportSuppliers[]), xmlRoot);

            XmlSerializerNamespaces nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add(string.Empty, string.Empty);

            using StringWriter textWriter = new StringWriter();

            xmlSerializer.Serialize(textWriter, suppliers, nameSpaces);

            string result = textWriter.ToString();
            return result;
        }

        //17. Export Cars With Their List Of Parts 
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new ExportCarPartOutput
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars.Select(pc => new PartOutputModel
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price
                    })
                    .OrderByDescending(p => p.Price)
                    .ToArray()
                })
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ToArray();


            XmlRootAttribute xmlRoot = new XmlRootAttribute("cars");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCarPartOutput[]), xmlRoot);

            XmlSerializerNamespaces nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add(string.Empty, string.Empty);

            using StringWriter textWriter = new StringWriter();

            xmlSerializer.Serialize(textWriter, cars, nameSpaces);

            string result = textWriter.ToString();
            return result;
        }

        // Query 18. Total Sales by Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count >= 1)
                .Select(c => new ExportCustomers
                {

                    FullName = c.Name,
                    BoughtCars = c.Sales.Count(),
                    SpentMoney = c.Sales.SelectMany(c => c.Car.PartCars)
                    .Sum(c => c.Part.Price)
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();


            XmlRootAttribute xmlRoot = new XmlRootAttribute("customers");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCustomers[]), xmlRoot);

            XmlSerializerNamespaces nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add(string.Empty, string.Empty);

            using StringWriter textWriter = new StringWriter();

            xmlSerializer.Serialize(textWriter, customers, nameSpaces);

            string result = textWriter.ToString();
            return result;
        }


        // 19. Export Sales With Applied Discount 

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {

            var sales = context.Sales
                .Select(s => new ExportSalesDto
                {
                    CarSale = new CarSaleOutputMpdel
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravellDistance = s.Car.TravelledDistance
                    },

                    Discount = s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartCars.Select(c => c.Part.Price).Sum(),
                    PriceWithDiscount = s.Car.PartCars.Select(c => c.Part.Price).Sum() -
                                       s.Car.PartCars.Select(c => c.Part.Price).Sum() * s.Discount / 100
                })
                .ToArray();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("sales");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportSalesDto[]), xmlRoot);

            XmlSerializerNamespaces nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add(string.Empty, string.Empty);

            using StringWriter textWriter = new StringWriter();

            xmlSerializer.Serialize(textWriter, sales, nameSpaces);

            string result = textWriter.ToString();
            return result;

        }
        private static XmlSerializer GenerateXmlSerializer(string rootName, Type dtoType)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);

            XmlSerializer xmlSerializer = new XmlSerializer(dtoType, rootName);

            return xmlSerializer;

        }
    }

}