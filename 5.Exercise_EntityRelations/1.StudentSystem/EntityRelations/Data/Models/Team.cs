using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P03_FootballBetting.Data.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LogoUrl { get; set; }

        [Required]
        public string Initials { get; set; }

        [Required]
        public decimal Budget { get; set; }

        [Required]
        [ForeignKey(nameof(Color))]
        public int PrimaryKitColorId { get; set; }
        public Color PrimaryKitColor { get; set; }

        [Required]
        [ForeignKey(nameof(Color))]
        public int SecondaryKitColorId { get; set; }
        public Color SecondaryKitColor { get; set; }


        [Required]
        [ForeignKey(nameof(Town))]
        public int TownId { get; set; }
        public Town Town { get; set; }

        public virtual ICollection<Player> Players { get; set; }
        public virtual ICollection<Game> HomeGames { get; set; }
        public virtual ICollection<Game> AwayGames { get; set; }
        //TeamId, Name, LogoUrl, Initials (JUV, LIV, ARS…),
        //Budget, PrimaryKitColorId, SecondaryKitColorId, TownId
    }
}
