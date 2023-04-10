namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";

        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var validPlays = new List<Play>();

            var plays = XmlConverter.Deserializer<XmlPlayInputModel>(xmlString, "Plays");

            foreach (var currentPlay in plays)
            {
                if (!IsValid(currentPlay))
                {
                    sb.AppendLine($"Invalid data!");
                    continue;
                }

                var isValidDuration = TimeSpan.TryParseExact(currentPlay.Duration,
                    "c",
                    CultureInfo.InvariantCulture, out TimeSpan time);

                if (!isValidDuration)
                {
                    sb.AppendLine($"Invalid data!");
                    continue;
                }

                if (time.Hours < 1)
                {
                    sb.AppendLine($"Invalid data!");
                    continue;
                }

                var newPlay = new Play
                {
                    Title = currentPlay.Title,
                    Duration = time,
                    Rating = currentPlay.Rating,
                    Genre = Enum.Parse<Genre>(currentPlay.Genre),
                    Description = currentPlay.Description,
                    Screenwriter = currentPlay.Screenwriter,
                };

                validPlays.Add(newPlay);
                sb.AppendLine($"Successfully imported {newPlay.Title} with genre {newPlay.Genre} and a rating of {newPlay.Rating}!");
            }

            context.Plays.AddRange(validPlays);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            var sb = new StringBuilder();
            var validCasts = new List<Cast>();

            var casts = XmlConverter.Deserializer<XmlCastsInputModel>(xmlString, "Casts");

            foreach (var currentCast in casts)
            {
                if (!IsValid(currentCast))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                var cast = new Cast
                {
                    FullName = currentCast.FullName,
                    IsMainCharacter = currentCast.IsMainCharacter,
                    PhoneNumber = currentCast.PhoneNumber,
                    PlayId = currentCast.PlayId,
                };

                validCasts.Add(cast);

                if (cast.IsMainCharacter == true)
                {
                    sb.AppendLine($"Successfully imported actor {cast.FullName} as a main character!");
                }
                else
                {
                    sb.AppendLine($"Successfully imported actor {cast.FullName} as a lesser character!");

                }
            }

            context.Casts.AddRange(validCasts);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var validTheaters = new List<Theatre>();

            var theatherThikets = JsonConvert.
                DeserializeObject<IEnumerable<JsonTheatresAndTicketsImportModel>>(jsonString);

            foreach (var currentTheather in theatherThikets)
            {
                if (!IsValid(currentTheather))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                //var theater = new Theatre
                //{
                //    Director = currentTheather.Director,
                //    Name = currentTheather.Name,
                //    NumberOfHalls = currentTheather.NumberOfHalls,
                //    Tickets = currentTheather.Tickets.Select(t => new Ticket     // <----// Don't check for invalid ticket
                //    {
                //        PlayId = t.PlayId,
                //        RowNumber = t.RowNumber,
                //        Price = t.Price,
                //    })
                //    .ToArray()

                var theater = new Theatre
                {
                    Director = currentTheather.Director,
                    Name = currentTheather.Name,
                    NumberOfHalls = currentTheather.NumberOfHalls
                };

                foreach (var currTicket in currentTheather.Tickets)
                {
                    if (!IsValid(currTicket))
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }

                    //Play play = context.Plays.FirstOrDefault(p => p.Id == currTicket.PlayId);

                    Ticket ticket = new Ticket
                    {
                        Price = currTicket.Price,
                        RowNumber = currTicket.RowNumber,
                        PlayId= currTicket.PlayId
                    };

                    theater.Tickets.Add(ticket);
                }

                validTheaters.Add(theater);
                sb.AppendLine($"Successfully imported theatre {theater.Name} with #{theater.Tickets.Count} tickets!");
            }

            context.AddRange(validTheaters);    
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
