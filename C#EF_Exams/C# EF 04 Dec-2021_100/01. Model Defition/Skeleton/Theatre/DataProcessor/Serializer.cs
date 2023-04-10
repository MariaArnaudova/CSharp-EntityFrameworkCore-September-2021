namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using Theatre.Data;
    using Theatre.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var theatres = context.Theatres.ToArray()
                .Where(h => h.NumberOfHalls >= numbersOfHalls && h.Tickets.Count >= 20)
                .Select(h => new
                {
                    Name = h.Name,
                    Halls = h.NumberOfHalls,
                    TotalIncome = h.Tickets.Where(t => t.RowNumber >= 1 && t.RowNumber <= 5)
                    .Sum(t => t.Price),
                    Tickets = h.Tickets.Where(t => t.RowNumber >= 1 && t.RowNumber <= 5)
                    .Select(t => new
                    {
                        Price = t.Price,
                        RowNumber = t.RowNumber
                    })
                    .OrderByDescending(t => t.Price)
                    .ToArray()
                })
                .OrderByDescending(t => t.Halls)
                .ThenBy(t => t.Name)
                 .ToList();

            string jsonResult = JsonConvert.SerializeObject(theatres, Formatting.Indented);

             return jsonResult;

            //return "TODO";

        }

        public static string ExportPlays(TheatreContext context, double rating)
        {
            var result = context.Plays.ToArray()
                .Where(p => p.Rating <= rating)
                .Select(p => new ExportPlaysXmlModel
                {
                    Title = p.Title,
                    Duration = p.Duration.ToString("c"),
                    Rating = p.Rating == 0 ? "Premier" : p.Rating.ToString(),
                    Genre = p.Genre.ToString(),
                    Actors = p.Casts.Where(c => c.IsMainCharacter == true)
                    .Select(c => new ActorViewModel
                    {
                        FullName = c.FullName,
                        MainCharacter = $"Plays main character in '{p.Title}'."
                    })
                    .OrderByDescending(a => a.FullName)
                    .ToArray()
                })
                .OrderBy(p => p.Title)
                .OrderByDescending(p => p.Genre)
                .ToArray();

            var xml = XmlConverter.Serialize(result, "Plays");
            return xml;
        }
    }
}
