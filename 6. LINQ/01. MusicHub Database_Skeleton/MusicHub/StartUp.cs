namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            //Test your solutions here

           Console.WriteLine(ExportAlbumsInfo(context, 9));
          // Console.WriteLine(ExportSongsAboveDuration(context,4));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context
                .Albums
                .ToArray()
                .Where(a => a.ProducerId == producerId)
                .OrderByDescending(x => x.Price)
                .Select(x => new
                {
                    AlbumName = x.Name,
                    //Release = x.ReleaseDate.ToString("MM/dd/yyyy"),
                    Release = x.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    ProduceName = x.Producer.Name,
                    AlbumSongs = x.Songs
                                  .ToArray()
                                  .Select(s => new
                                  {
                                      SongName = s.Name,
                                      Price = s.Price,
                                      Writer = s.Writer.Name
                                  })
                                   .OrderByDescending(x => x.SongName)
                                   .ThenBy(x => x.Writer)
                                   .ToArray(),
                    TotalAlbumPrice = x.Price,
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();
            int i = 1;

            foreach (var album in albums)
            {

                sb.AppendLine($"-AlbumName: {album.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {album.Release}");
                sb.AppendLine($"-ProducerName: {album.ProduceName}");
                sb.AppendLine($"-Songs:");

                foreach (var song in album.AlbumSongs)
                {
                    sb.AppendLine($"---#{i++}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.Price:F2}");
                    sb.AppendLine($"---Writer: {song.Writer}");
                }

                sb.AppendLine($"-AlbumPrice: {album.TotalAlbumPrice:F2}");
            }

            return sb.ToString().TrimEnd();
        }


        //public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        //{
        //    var currAlbums = context.Producers
        //        .FirstOrDefault(x => x.Id == producerId)
        //        .Albums
        //        .ToList()
        //        .OrderByDescending(x => x.Price)
        //        .Select(album => new
        //        {
        //            AlbumName = album.Name,
        //            ReliseDate = album.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
        //            ProduserName = album.Producer.Name,
        //            CurrSongs = album.Songs.Select(song => new
        //            {
        //                SongName = song.Name,
        //                Price = song.Price,
        //                SongWriter = song.Writer.Name
        //            })
        //            .OrderByDescending(x => x.SongName)
        //            .ThenBy(x => x.SongWriter),

        //            AlbumPrice = album.Price
        //        })
        //        .ToList();

        //    var sb = new StringBuilder();

        //    foreach (var album in currAlbums)
        //    {
        //        sb.AppendLine($"-AlbumName: {album.AlbumName}");
        //        sb.AppendLine($"-ReleaseDate: {album.ReliseDate:MM/dd/yyyy}");
        //        sb.AppendLine($"-ProducerName: {album.ProduserName}");
        //        sb.AppendLine($"-Songs:");

        //        var counter = 1;
        //        foreach (var song in album.CurrSongs)
        //        {
        //            sb.AppendLine($"---#{counter++}");
        //            sb.AppendLine($"---SongName: {song.SongName}");
        //            sb.AppendLine($"---Price: {song.Price:f2}");
        //            sb.AppendLine($"---Writer: {song.SongWriter}");
        //        }
        //        sb.AppendLine($"-AlbumPrice: {album.AlbumPrice:f2}");
        //    }

        //    return sb.ToString().TrimEnd();
        //}
        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {//its Name, Performer Full Name, Writer Name, Album Producer and Duration (in format("c"))
            var songs = context
                .Songs
                .ToArray()
                .Where(x => x.Duration.TotalSeconds > duration)
                .Select(x => new
                {
                    SongName = x.Name,
                    Writer = x.Writer.Name,
                    Producer = x.Album.Producer.Name,
                    Duration = x.Duration.ToString("c"),
                    PerformerFullName = x.SongPerformers
                                        .ToArray()
                                        .Select(x => $"{x.Performer.FirstName} {x.Performer.LastName}")
                                        .FirstOrDefault()
                })
                .OrderBy(x => x.SongName)
                .ThenBy(x => x.Writer)
                .ThenBy(x => x.PerformerFullName)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            int i = 1;

            foreach (var song in songs)
            {
                sb.AppendLine($"-Song #{i++}");
                sb.AppendLine($"---SongName: {song.SongName}");
                sb.AppendLine($"---Writer: {song.Writer}");
                sb.AppendLine($"---Performer: {song.PerformerFullName}");
                sb.AppendLine($"---AlbumProducer: {song.Producer}");
                sb.AppendLine($"---Duration: {song.Duration}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
