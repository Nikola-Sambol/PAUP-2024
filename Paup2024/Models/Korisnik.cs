using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PAUP_2024.Models
{
    [Table("korisnici")]
    public class Korisnik
    {
        [Key]
        [Column("korisnicko_ime")]
        [Display(Name = "Korisničko ime")]
        [Required]
        public string KorisnickoIme { get; set; }

        [Display(Name = "Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Lozinka { get; set; }

        [Display(Name = "Prezime")]
        [Required]
        public string Prezime { get; set; }

        [Display(Name = "Ime")]
        [Required]
        public string Ime { get; set; }

        [Display(Name = "Prezime i ime")]
        public string PrezimeIme
        {
            get
            {
                return Prezime + " " + Ime;
            }
        }

        [Column("ovlast")]
        [Display(Name = "Ovlast")]
        [ForeignKey("Ovlast")]
        public string SifraOvlasti { get; set; }

        [Display(Name = "Ovlast")]
        public virtual Ovlast Ovlast { get; set; }

        [Display(Name = "Lozinka")]
        [DataType(DataType.Password)]
        [Required]
        [NotMapped]
        public string LozinkaUnos { get; set; }

        [Display(Name = "Lozinka ponovljena")]
        [DataType(DataType.Password)]
        [Required]
        [NotMapped]
        [Compare("LozinkaUnos", ErrorMessage = "Lozinke moraju biti jednake")]
        public string LozinkaUnos2 { get; set; }
    }
}