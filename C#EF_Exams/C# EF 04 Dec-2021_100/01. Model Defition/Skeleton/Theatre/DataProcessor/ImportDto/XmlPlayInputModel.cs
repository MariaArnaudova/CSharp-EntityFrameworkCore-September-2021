using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using Theatre.Data.Models.Enums;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType("Play")]
    public class XmlPlayInputModel
    {
        [Required]
        [XmlElement("Title")]
        [StringLength(50, MinimumLength = 4)]
        public string Title { get; set; }

        [Required]
        [XmlElement("Duration")]
        //[EnumDataType(typeof(TimeSpan))]
        public string Duration { get; set; }

        [Required]
        [XmlElement("Rating")]
        [Range(typeof(float), "0.00", "10.00")]
        public float Rating { get; set; }

        [Required]
        [EnumDataType(typeof(Genre))]
        [XmlElement("Genre")]
        public string Genre { get; set; }

        [Required]
        [XmlElement("Description")]
        [MaxLength(700)]
        public string Description { get; set; }

        [Required]
        [XmlElement("Screenwriter")]
        [StringLength(30, MinimumLength = 4)]
        public string Screenwriter { get; set; }

        //•	Id – integer, Primary Key
        //•	Title – text with length[4, 50] (required)
        //•	Duration – TimeSpan in format {hours:minutes:seconds
        //                                                          }, with a minimum length of 1 hour. (required)
        //•	Rating – float in the range[0.00….10.00] (required)
        //•	Genre – enumeration of type Genre, with possible values (Drama, Comedy, Romance, Musical) (required)
        //•	Description – text with length up to 700 characters (required)
        //•	Screenwriter – text with length [4, 30] (required)
        //•	Casts - a collection of type Cast
        //•	Tickets - a collection of type Ticket


        // <Play>
        //  <Title>The Hsdfoming</Title>
        //  <Duration>03:40:00</Duration>
        //  <Rating>8.2</Rating>
        //  <Genre>Action</Genre>
        //  <Description>A guyat Pinter turns into a debatable conundrum as oth ordinary and menacing. Much of this has to do with the fabled Pinter Pause, which simply mirrors the way we often respond to each other in conversation, tossing in remainders of thoughts on one subject well after having moved on to another.</Description>
        //  <Screenwriter>Roger Nciotti</Screenwriter>
        //</Play>
    }
}
