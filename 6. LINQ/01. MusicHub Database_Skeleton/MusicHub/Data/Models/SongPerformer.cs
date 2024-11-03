using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MusicHub.Data.Models
{
    public class SongPerformer
    {      
        [Required]
        [ForeignKey(nameof(Song))]
        public int SongId { get; set; }
        public Song Song { get; set; }


        [Required]
        [ForeignKey(nameof(Performer))]
        public int PerformerId { get; set; }
        public Performer Performer { get; set; }


        //•	SongId – Integer, Primary Key
        //•	Song – the performer’s Song(required)
        //•	PerformerId – Integer, Primary Key
        //•	Performer – the song’s Performer(required)

    }
}
