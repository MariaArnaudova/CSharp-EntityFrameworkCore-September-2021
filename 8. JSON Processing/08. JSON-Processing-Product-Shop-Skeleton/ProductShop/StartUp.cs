using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DataTransferObjects;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        static IMapper mapper;
        public static void Main(string[] args)
        {

            var productShopContext = new ProductShopContext();

            //productShopContext.Database.EnsureDeleted();
            //productShopContext.Database.EnsureCreated();


            //string userJason = File.ReadAllText("../../../Datasets/users.json");
            //string productJason = File.ReadAllText("../../../Datasets/products.json");
            //string categoryJason = File.ReadAllText("../../../Datasets/categories.json");
            //string categoryProductJason = File.ReadAllText("../../../Datasets/categories-products.json");
            //var importUsers = ImportUsers(productShopContext, userJason);
            //var importProducts = ImportProducts(productShopContext, productJason);
            //var importCategories = ImportCategories(productShopContext, categoryJason);
            //var importCategoriesProducts = ImportCategoryProducts(productShopContext, categoryProductJason);
            //var exportProductInRange = GetProductsInRange(productShopContext);
            //var soldProducts = GetSoldProducts(productShopContext);
            //var categoriesByProductsCount = GetCategoriesByProductsCount(productShopContext);
            var usersWithProducts = GetUsersWithProducts(productShopContext);
            Console.WriteLine(usersWithProducts);
        }

        //1. Import Users
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            InitializeAutomapper();

            var dtoUsers = JsonConvert.DeserializeObject<IEnumerable<UserInputModel>>(inputJson);
            var users = mapper.Map<IEnumerable<User>>(dtoUsers);
            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        // 2. Import Products 
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            InitializeAutomapper();
            var dtoProducts = JsonConvert.DeserializeObject<IEnumerable<ProductInputModel>>(inputJson);
            var products = mapper.Map<IEnumerable<Product>>(dtoProducts);
            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        // 3.  Import Categories 
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            InitializeAutomapper();
            var dtoCategories = JsonConvert.DeserializeObject<IEnumerable<CategoryInputModel>>(inputJson);
            var categories = mapper.Map<IEnumerable<Category>>(dtoCategories)
                .Where(c => c.Name != null)
                .ToList();
            context.Categories.AddRange(categories);
            context.SaveChanges();
            return $"Successfully imported {categories.Count()}";

        }

        // 04. Import Categories and Products 
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            InitializeAutomapper();
            var dtoCategoriesProducts = JsonConvert.DeserializeObject<IEnumerable<CategoryProductInputModel>>(inputJson);
            var categoryProducts = mapper.Map<IEnumerable<CategoryProduct>>(dtoCategoriesProducts);
            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();
            return $"Successfully imported {categoryProducts.Count()}";

        }

        // 05. Export Products In Range 
        public static string GetProductsInRange(ProductShopContext context)
        {

            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    seller = p.Seller.FirstName + ' ' + p.Seller.LastName
                })
                .OrderBy(p => p.price)
                .ToList();

            var result = JsonConvert.SerializeObject(products, Formatting.Indented);
            return result;
        }

        // 06. Export Sold Products 
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(ps => ps.BuyerId != null))
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold
                                    .Where(pb => pb.BuyerId != null)
                                    .Select(pb => new
                                    {
                                        name = pb.Name,
                                        price = pb.Price,
                                        buyerFirstName = pb.Buyer.FirstName,
                                        buyerLastName = pb.Buyer.LastName
                                    })
                                    .ToList()
                })
                .OrderBy(u => u.lastName)
                .ThenBy(u => u.firstName)
                .ToList();

            var result = JsonConvert.SerializeObject(users, Formatting.Indented);

            return result;
        }

        // 07. Export Categories By Products Count 
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoryProducts.Where(cp => cp.CategoryId == c.Id).Count(),
                    averagePrice = (c.CategoryProducts.Sum(cp => cp.Product.Price) / c.CategoryProducts.Count()).ToString("F2"),
                    totalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price).ToString("F2")

                })
                .OrderByDescending(c => c.productsCount)
                .ToArray();

            var result = JsonConvert.SerializeObject(categories, Formatting.Indented);
            return result;

        }

        // 08. Export Users and Products 

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersWithProducts = context.Users
                .Include(p => p.ProductsSold)
                .ToList()
               .Where(u => u.ProductsSold.Any(ps => ps.BuyerId != null))
               .Select(u => new
               {

                   firstName = u.FirstName,
                   lastName = u.LastName,
                   age = u.Age,
                   soldProducts = new
                   {
                       count = u.ProductsSold.Where(x => x.BuyerId != null).Count(),
                       products = u.ProductsSold
                                   .Where(p => p.BuyerId != null)
                                   .Select(p => new
                                   {
                                       name = p.Name,
                                       price = p.Price
                                   })
                   }
               })
               .OrderByDescending(u => u.soldProducts.count)
               .ToList();

            var resultObject = new
            {
                usersCount = context.Users.Where(u => u.ProductsSold.Any(ps => ps.BuyerId != null)).Count(),
                users = usersWithProducts
            };

            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var resultJson = JsonConvert.SerializeObject(resultObject, Formatting.Indented, jsonSettings);

            return resultJson;

        }
        private static void InitializeAutomapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            mapper = config.CreateMapper();
        }
    }
}