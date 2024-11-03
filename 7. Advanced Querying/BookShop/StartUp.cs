namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            // string input = Console.ReadLine();
            // Console.WriteLine(GetBooksByAgeRestriction(db, input));

            // Console.WriteLine(GetGoldenBooks(db));
            // Console.WriteLine(GetBooksByPrice(db));

            //int year = int.Parse(Console.ReadLine());
            //Console.WriteLine(GetBooksNotReleasedIn(db, year));


            // Console.WriteLine(GetBooksByCategory(db, input));

            // Console.WriteLine(GetBooksReleasedBefore(db, input));
            // Console.WriteLine(GetAuthorNamesEndingIn(db, input));
            // Console.WriteLine(GetBookTitlesContaining(db, input));
            // Console.WriteLine(GetBooksByAuthor(db, input));

            // int lengthCheck = int.Parse(Console.ReadLine());
            // Console.WriteLine(CountBooks(db, lengthCheck));
            // Console.WriteLine(CountCopiesByAuthor(db));

            // Console.WriteLine(GetTotalProfitByCategory(db));

            // Console.WriteLine(GetMostRecentBooks(db));
            // IncreasePrices(db);
            Console.WriteLine(RemoveBooks(db));
        }


        //1. Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            AgeRestriction ageRestriction = Enum.Parse<AgeRestriction>(command, true);
            string[] books = context.Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .Select(b => b.Title)
                .OrderBy(title => title)
                .ToArray();

            var result = string.Join(Environment.NewLine, books);
            return result;
        }

        //2. Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            string[] goldenBooks = context.Books
                .Where(gb => gb.EditionType == EditionType.Gold && gb.Copies < 5000)
                .OrderBy(gb => gb.BookId)
                .Select(gb => gb.Title)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var title in goldenBooks)
            {
                sb.AppendLine($"{title}");
            }

            return sb.ToString().TrimEnd();
        }

        //3. Books by Price 
        public static string GetBooksByPrice(BookShopContext context)
        {
            var booksByPrice = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    Title = b.Title,
                    Price = b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var book in booksByPrice)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:F2}");
            }
            return sb.ToString().TrimEnd();
        }

        //4. Not Released In 
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books          //x.ReleaseDate.HasValue
                .Where(x => x.ReleaseDate != null && x.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(t => t.Title)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var title in books)
            {
                sb.AppendLine($"{title}");
            }

            return sb.ToString().TrimEnd();
        }

        // 5. Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower())
                .ToArray();

            var books = context.BooksCategories
                .Where(b => categories.Contains(b.Category.Name.ToLower()))
                .Select(bc => bc.Book.Title)
                .OrderBy(b => b)
                .ToArray();


            StringBuilder sb = new StringBuilder();

            foreach (var title in books)
            {
                sb.AppendLine($"{title}");
            }

            return sb.ToString().TrimEnd();
        }


        //6. Released Before Date 
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var dateResult = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var books = context.Books
               .Where(x => x.ReleaseDate.Value < dateResult)
               .OrderByDescending(x => x.ReleaseDate)
               .Select(x => new
               {
                   x.Title,
                   x.EditionType,
                   x.Price
               })
               .ToArray();


            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //7. Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => a.FirstName.ToLower().EndsWith(input.ToLower()))
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName
                })
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.FirstName} {author.LastName}");
            }

            return sb.ToString().TrimEnd();
        }

        // 8. Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {

            var books = context.Books
                .Where(book => book.Title.ToLower().Contains(input.ToLower()))
                .OrderBy(book => book.Title)
                .Select(book => new
                {
                    book.Title
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }

        // 9. Book Search by Author 
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title,
                    b.Author.FirstName,
                    b.Author.LastName
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.FirstName} {book.LastName})");
            }

            return sb.ToString().TrimEnd();
        }

        // 10. Count Books 
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .ToArray();

            int result = books.Count();
            return result;
        }

        // 11. Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var books = context.Authors
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    sumCopies = x.Books.Sum(x => x.Copies)
                })
                .OrderByDescending(b => b.sumCopies)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.FirstName} {book.LastName} - {book.sumCopies}");
            }

            return sb.ToString().TrimEnd();
        }

        //12. Profit by Category 
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                .Select(c => new
                {
                    c.Name,
                    Profit = c.CategoryBooks.Sum(b => b.Book.Price * b.Book.Copies)
                })
                .OrderByDescending(b => b.Profit)
                .ThenBy(b => b.Name)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var category in categories)
            {
                sb.AppendLine($"{category.Name} ${category.Profit:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //13. Most Recent Books 

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                .OrderBy(b => b.Name)
                .Select(c => new
                {
                    CategoryName = c.Name,
                    Books = c.CategoryBooks.Select(b => new
                    {
                        BookTitle = b.Book.Title,
                        BookReleaseDate = b.Book.ReleaseDate
                    })
                    .OrderByDescending(b => b.BookReleaseDate)
                    .Take(3)

                });

            StringBuilder sb = new StringBuilder();

            foreach (var category in categories)
            {
                sb.AppendLine($"--{category.CategoryName}");

                foreach (var book in category.Books)
                {
                    sb.AppendLine($"{book.BookTitle} ({book.BookReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        // 14. Increase Prices 
        public static void IncreasePrices(BookShopContext context)
        {
            //Increase the prices of all books released before 2010 by 5.
            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2010)
                .ToArray();

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        // 15. Remove Books 

        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Copies < 4200)
                .ToArray();

            context.Books.RemoveRange(books);
            context.SaveChanges();
            return books.Count();
        }

        //
    }
}
