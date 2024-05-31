using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PAUP_2024.Models
{
    [Table("aktivnosti")]
    public class Aktivnosti
    {
        [Key]
        [Display(Name = "ID aktivnosti")]
        public int ID { get; set; }

        [Column("naziv")]
        [Display(Name = "Naziv aktivnosti")]
        [Required(ErrorMessage = "{0} je obavezan")]
        public string nazivAktivnosti { get; set; }

        [Column("vrsta")]
        [Display(Name = "Vrsta aktivnosti")]
        [Required(ErrorMessage = "{0} je obavezna")]
        public VrstaAktivnosti vrstaAktivnosti { get; set; }

        [Column("trajanje")]
        [Display(Name = "Vrijeme izvođenja (u minutama)")]
        [Required(ErrorMessage = "{0} je obavezna")]
        public int vrijemeTrajanjaAktivnosti { get; set; }

        [Column("duljina")]
        [Display(Name = "Duljina aktivnosti (u metrima)")]
        [Required(ErrorMessage = "{0} je obavezna")]
        public int duljinaAktivnosti { get; set; }

        [Column("datum")]
        [Display(Name = "Datum aktivnosti")]
        [Required(ErrorMessage = "{0} je obavezan")]
        [DataType(DataType.Date)]
        public DateTime datumAktivnosti { get; set; }

        [Column("korisnikID")]
        [Display(Name = "Korisnicko ime")]
        public string korisnikId { get; set; }



    }
}