using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using ProductShop.XmlHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ProductShop
{
 
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext productShopContext = new ProductShopContext();

            //productShopContext.Database.EnsureDeleted();
            //productShopContext.Database.EnsureCreated();
            //Console.WriteLine("ShopContext reset succses.");


            //string importUsersXml = File.ReadAllText("../../../Datasets/users.xml");
            //string importProductsXml = File.ReadAllText("../../../Datasets/products.xml");
            //string importCategoryXml = File.ReadAllText("../../../Datasets/categories.xml");
            //string importCategoryProductsXml = File.ReadAllText("../../../Datasets/categories-products.xml");

            //Console.WriteLine(ImportUsers(productShopContext, importUsersXml));
            //Console.WriteLine(ImportProducts(productShopContext, importProductsXml));
            //Console.WriteLine(ImportCategories(productShopContext, importCategoryXml));
            //Console.WriteLine(ImportCategoryProducts(productShopContext, importCategoryProductsXml));

            //string resultExportProducts = GetProductsInRange(productShopContext);
            //string resultUsersWithSoldProducts = GetSoldProducts(productShopContext);
            //string resultCategories = GetCategoriesByProductsCount(productShopContext);
            string resultUsersWithProducts = GetUsersWithProducts(productShopContext);

            Console.WriteLine(resultUsersWithProducts);

        }

        //Query 1. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute rootAttribute = new XmlRootAttribute("Users");

            XmlSerializer serializer = new XmlSerializer(typeof(ImportUsersModel[]), rootAttribute);

            StringReader stringReader = new StringReader(inputXml);

            ImportUsersModel[] usersDto = serializer.Deserialize(stringReader) as ImportUsersModel[];

            ICollection<User> users = new HashSet<User>();

            foreach (var currentUser in usersDto)
            {
                User user = new User
                {
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    Age = currentUser.Age
                };

                users.Add(user);

            }

            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        // Query 2. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute rootAttribute = new XmlRootAttribute("Products");

            XmlSerializer serializer = new XmlSerializer(typeof(ImportProductModel[]), rootAttribute);

            StringReader stringReader = new StringReader(inputXml);

            ImportProductModel[] productsDto = serializer.Deserialize(stringReader) as ImportProductModel[];

            ICollection<Product> products = new HashSet<Product>();


            foreach (var currentProduct in productsDto)
            {
                Product product = new Product
                {
                    Name = currentProduct.Name,
                    Price = currentProduct.Price,
                    SellerId = currentProduct.SellerId,
                    BuyerId = currentProduct.BuyerId

                };

                products.Add(product);

            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        // 03. Import Categories 
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute rootAttribute = new XmlRootAttribute("Categories");

            XmlSerializer serializer = new XmlSerializer(typeof(ImportCategoryModel[]), rootAttribute);

            StringReader stringReader = new StringReader(inputXml);

            ImportCategoryModel[] categoriesDto = serializer.Deserialize(stringReader) as ImportCategoryModel[];

            ICollection<Category> categories = new HashSet<Category>();


            foreach (var currentCategory in categoriesDto)
            {
                Category category = new Category
                {
                    Name = currentCategory.Name

                };

                categories.Add(category);
            }

            context.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        // 04. Import Categories and Products 
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute rootAttribute = new XmlRootAttribute("CategoryProducts");

            XmlSerializer serializer = new XmlSerializer(typeof(ImportCatergoryProductModel[]), rootAttribute);

            StringReader stringReader = new StringReader(inputXml);

            ImportCatergoryProductModel[] categoryProductsDto = serializer.Deserialize(stringReader) as ImportCatergoryProductModel[];

            ICollection<CategoryProduct> categoryProducts = new HashSet<CategoryProduct>();

            foreach (var catProd in categoryProductsDto)
            {
                CategoryProduct catergoryProduct = new CategoryProduct
                {
                    CategoryId = catProd.CategoryId,
                    ProductId = catProd.ProductId
                };

                categoryProducts.Add(catergoryProduct);

            }

            context.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        // 05. Export Products In Range 
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new ExportProductModel
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = p.Buyer.FirstName + ' ' + p.Buyer.LastName
                })
                .OrderBy(p => p.Price)
                .Take(10)
                .ToArray();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Products");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportProductModel[]), xmlRoot);

            XmlSerializerNamespaces nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add(string.Empty, string.Empty);

            StringWriter textWriter = new StringWriter();

            xmlSerializer.Serialize(textWriter, products, nameSpaces);

            string result = textWriter.ToString();
            return result;

        }

        //6. Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Count() >= 1)
                .Select(u => new ExportUserWithSoldProducts
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    ProductsSold = u.ProductsSold.Select(ps => new ExportProduct
                    {
                        Name = ps.Name,
                        Price = ps.Price
                    })
                    .ToArray()

                })
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ToArray();


            XmlRootAttribute xmlRoot = new XmlRootAttribute("Users");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportUserWithSoldProducts[]), xmlRoot);

            XmlSerializerNamespaces nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add(string.Empty, string.Empty);

            StringWriter textWriter = new StringWriter();

            xmlSerializer.Serialize(textWriter, users, nameSpaces);

            string result = textWriter.ToString();
            return result;

        }

        //Query 7. Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new ExportCategory
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count(),
                    AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = c.CategoryProducts.Select(cp => cp.Product.Price).Sum()
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Categories");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCategory[]), xmlRoot);

            XmlSerializerNamespaces nameSpaces = new XmlSerializerNamespaces();
            nameSpaces.Add(string.Empty, string.Empty);

            StringWriter textWriter = new StringWriter();

            xmlSerializer.Serialize(textWriter, categories, nameSpaces);

            string result = textWriter.ToString();
            return result;
        }

        // 08. Export Users and Products 
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .ToArray()
                .Where(u => u.ProductsSold.Any())
                .Select(u => new ExportUserModel
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProduct = new SoldProductCountDto
                    {
                        Count = u.ProductsSold.Count(),
                        Products = u.ProductsSold.Select(p => new ExportProductDto
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                    }
                })
                .OrderByDescending(x => x.SoldProduct.Count)
                .Take(10)
                .ToArray();

            var resultDto = new ExportUserCountDto
            {
                Count = context.Users.Count(p => p.ProductsSold.Any()),
                Users = users
            };

            //XmlRootAttribute xmlRoot = new XmlRootAttribute("Users");

            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportUserModel[]), xmlRoot);

            //XmlSerializerNamespaces nameSpaces = new XmlSerializerNamespaces();
            //nameSpaces.Add(string.Empty, string.Empty);

            //StringWriter textWriter = new StringWriter();

            //xmlSerializer.Serialize(textWriter, resultDto, nameSpaces);
            //string result = textWriter.ToString();
            //return result;


            var result = XMLConverter.Serialize(resultDto, "Users");
            return result;

        }

        //public static string GetUsersWithProducts(ProductShopContext context)
        //{
        //    var usersAndproducts = context
        //        .Users
        //        .ToArray()
        //        .Where(p => p.ProductsSold.Any())
        //        .Select(u => new ExportUserModel
        //        {
        //            FirstName = u.FirstName,
        //            LastName = u.LastName,
        //            Age = u.Age,
        //            SoldProduct = new SoldProductCountDto
        //            {
        //                Count = u.ProductsSold.Count,
        //                Products = u.ProductsSold.Select(p => new ExportProductDto
        //                {
        //                    Name = p.Name,
        //                    Price = p.Price
        //                })
        //                    .OrderByDescending(p => p.Price)
        //                    .ToArray()
        //            }
        //        })
        //        .OrderByDescending(x => x.SoldProduct.Count)
        //        .Take(10)
        //        .ToArray();

        //    var resultDto = new ExportUserCountDto
        //    {
        //        Count = context.Users.Count(p => p.ProductsSold.Any()),
        //        Users = usersAndproducts
        //    };

        //    var result = XMLConverter.Serialize(resultDto, "Users");

        //    return result;
        //}
    }
}