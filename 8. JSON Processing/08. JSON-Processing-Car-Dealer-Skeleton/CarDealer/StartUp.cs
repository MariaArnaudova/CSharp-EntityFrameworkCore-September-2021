using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            CarDealerContext carDealerContext = new CarDealerContext();

            //carDealerContext.Database.EnsureDeleted();
            //carDealerContext.Database.EnsureCreated();

            //string suppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");
            //string partsJson = File.ReadAllText("../../../Datasets/parts.json");
            //string carsJson = File.ReadAllText("../../../Datasets/cars.json");
            //string customersJson = File.ReadAllText("../../../Datasets/customers.json");
            //string salesJson = File.ReadAllText("../../../Datasets/sales.json");

            //var inputSuppliers = ImportSuppliers(carDealerContext, suppliersJson);
            //var inputParts = ImportParts(carDealerContext, partsJson);
            //var inputCars = ImportCars(carDealerContext, carsJson);
            //var inputCustomers = ImportCustomers(carDealerContext, customersJson);
            //var inputSales = ImportSales(carDealerContext, salesJson);
            // Console.WriteLine(inputSuppliers);

            // var orderedCustomers = GetOrderedCustomers(carDealerContext);
            // var carsFromMakeToyota = GetCarsFromMakeToyota(carDealerContext);
            // var localSuppliers = GetLocalSuppliers(carDealerContext);
            // var carsWithTheirListOfParts = GetCarsWithTheirListOfParts(carDealerContext);
            // var totalSalesByCustomer = GetTotalSalesByCustomer(carDealerContext);
            var salesWithAppliedDiscount = GetSalesWithAppliedDiscount(carDealerContext);
            Console.WriteLine(salesWithAppliedDiscount);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var importSuppliers = JsonConvert.DeserializeObject<IEnumerable<SuppliersInputModel>>(inputJson);

            var suppliers = importSuppliers.Select(s => new Supplier
            {
                Name = s.Name,
                IsImporter = s.IsImporter
            })
             .ToList();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}.";
        }

        // 10. Import Parts 
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            //var importParts = JsonConvert.DeserializeObject<IEnumerable<PartInputModel>>(inputJson);

            var suppliersId = context.Suppliers
                                     .Select(s => s.Id)
                                     .ToArray();

            var validParts = JsonConvert.DeserializeObject<IEnumerable<PartInputModel>>(inputJson)
                                        .Where(p => suppliersId.Contains(p.SupplierId))
                                        .ToList();


            var parts = validParts.Select(p => new Part
            {

                Name = p.Name,
                Price = p.Price,
                Quantity = p.Guantity,
                SupplierId = p.SupplierId

            })
            .ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {parts.Count}.";

        }

        // 11. Import Cars 
        public static string ImportCars(CarDealerContext context, string inputJson)
        {

            var carsJson = JsonConvert.DeserializeObject<IEnumerable<CarInputModel>>(inputJson);

            var listOfCars = new List<Car>();

            foreach (var carsDto in carsJson)
            {
                Car currentCar = new Car
                {
                    Make = carsDto.Make,
                    Model = carsDto.Model,
                    TravelledDistance = carsDto.TravelledDistance
                };

                foreach (var partId in carsDto?.PartsId.Distinct())
                {
                    currentCar.PartCars.Add(new PartCar
                    {
                        PartId = partId

                    });
                }
                listOfCars.Add(currentCar);
            }

            context.Cars.AddRange(listOfCars);

            context.SaveChanges();

            return $"Successfully imported {listOfCars.Count()}.";
        }

        //12. Import Customers 
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customersJson = JsonConvert.DeserializeObject<IEnumerable<CustomersInputModel>>(inputJson);

            var customers = customersJson.Select(c => new Customer
            {
                Name = c.Name,
                BirthDate = c.BirthDate,
                IsYoungDriver = c.IsYoungDriver
            })
                .ToList();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        //13. Import Sales 
        public static string ImportSales(CarDealerContext context, string inputJson)
        {

            var salesJson = JsonConvert.DeserializeObject<IEnumerable<SalesInputModel>>(inputJson);

            var sales = salesJson.Select(s => new Sale
            {

                CarId = s.CarId,
                CustomerId = s.CustomerId,
                Discount = s.Discount
            })
                .ToList();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        // 14. Export Ordered Customers 

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate.Date)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToList();

            var result = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return result;
        }

        // 15. Export Cars From Make Toyota 
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {


            var carsToyota = context.Cars
               .Where(c => c.Make == "Toyota")
               .Select(c => new
               {
                   Id = c.Id,
                   Make = c.Make,
                   Model = c.Model,
                   TravelledDistance = c.TravelledDistance

               })
               .OrderBy(c => c.Model)
               .ThenByDescending(c => c.TravelledDistance)
               .ToList();

            var result = JsonConvert.SerializeObject(carsToyota, Formatting.Indented);
            return result;
        }

        // 16. Export Local Suppliers 
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count()
                })
                .ToArray();

            var result = JsonConvert.SerializeObject(suppliers, Formatting.Indented);
            return result;
        }

        // 17. Export Cars With Their List Of Parts 
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TravelledDistance = c.TravelledDistance
                    },
                    parts = c.PartCars.Select(pc => new
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price.ToString("F2")

                    })
                    .ToArray()
                })
                .ToArray();

            var result = JsonConvert.SerializeObject(cars, Formatting.Indented);
            return result;
        }

        // 18. Export Total Sales By Customer 
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count >= 1)
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count(),
                    spentMoney = c.Sales.SelectMany(s => s.Car.PartCars.Select(pc => pc.Part.Price)).Sum()         //.Sum(x => x.Car)
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenBy(c => c.boughtCars)
                .ToArray();

            var result = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return result;
        }

        // 19. Export Sales With Applied Discount // 
        //public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        //{

        //    var sales = context.Sales
               
        //        .Select(s => new
        //        {
        //            car = new
        //            {
        //                Make = s.Car.Make,
        //                Model = s.Car.Model,
        //                TravelledDistance = s.Car.TravelledDistance
        //            },

        //            customerName = s.Customer.Name,
        //            Discount = s.Discount.ToString("f2"), //////!!!!!
        //            price = (s.Car.PartCars.Sum(p => p.Part.Price)).ToString("f2"),
        //            priceWithDiscount = (s.Car.PartCars.Sum(p => p.Part.Price) - s.Car.PartCars.Sum(p => p.Part.Price) * (s.Discount / 100)).ToString("f2")

        //        })
        //         .Take(10)
        //     .ToArray();

        //    var result = JsonConvert.SerializeObject(sales, Formatting.Indented);
        //    return result;
        //}

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {

            var sales = context.Sales

                .Select(s => new
                {
                    car = new
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },

                    customerName = s.Customer.Name,
                    Discount = s.Discount.ToString("f2"), //////!!!!!
                    price = (s.Car.PartCars.Select(p => p.Part.Price).Sum()).ToString("f2"),
                    priceWithDiscount = (s.Car.PartCars.Select(p => p.Part.Price).Sum() - s.Car.PartCars.Select(p => p.Part.Price).Sum() * (s.Discount / 100)).ToString("f2")

                })
                 .Take(10)
             .ToArray();

            var result = JsonConvert.SerializeObject(sales, Formatting.Indented);
            return result;
        }
    }
}



