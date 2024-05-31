using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PAUP_2024.Models
{
    [Table("buducaAktivnost")]
    public class PlaniranaAktivnost
    {
        [Key]
        [Display(Name = "ID aktivnosti")]
        public int ID { get; set; }

        [Column("naziv")]
        [Display(Name = "Naziv aktivnosti")]
        [Required(ErrorMessage = "{0} je obavezna")]
        public string nazivAktivnosti { get; set; }

        [Column("vrsta")]
        [Display(Name = "Vrsta aktivnosti")]
        [Required(ErrorMessage = "{0} je obavezna")]
        public VrstaAktivnosti vrstaAktivnosti { get; set; }

        [Column("datum")]
        [Display(Name = "Datum aktivnosti")]
        [Required(ErrorMessage = "{0} je obavezan")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(PlaniranaAktivnost), nameof(ValidirajDatumAktivnosti))]
        public DateTime datumAktivnosti { get; set; }

        [Column("korisnikID")]
        public string korisnikId { get; set; }

        public DateTime? PoslednjaObavijest { get; set; }


        // Metoda za validaciju datuma
        public static ValidationResult ValidirajDatumAktivnosti(DateTime datumAktivnosti, ValidationContext context)
        {
            if (datumAktivnosti < DateTime.Today)
            {
                return new ValidationResult("Datum aktivnosti ne može biti u prošlosti.");
            }
            return ValidationResult.Success;
        }
    }
}