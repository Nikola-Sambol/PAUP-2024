using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PAUP_2024.Controllers;
using PAUP_2024.Models;
using PAUP_2024.Razno;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace PAUP_2024_UnitTest
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void KorisnickoIme_Required()
        {
            // Arrange
            var korisnik = new Korisnik();

            // Act
            var results = ValidirajModel(korisnik);

            // Assert
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("KorisnickoIme") && r.ErrorMessage == "The Korisničko ime field is required."));
        }

        [TestMethod]
        public void Email_Required()
        {
            // Arrange
            var korisnik = new Korisnik();

            // Act
            var results = ValidirajModel(korisnik);

            // Assert
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Email") && r.ErrorMessage == "The Email field is required."));
        }

        [TestMethod]
        public void Email_ValidEmailAddress()
        {
            // Arrange
            var korisnik = new Korisnik { Email = "invalidemail" };

            // Act
            var results = ValidirajModel(korisnik);

            // Assert
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Email") && r.ErrorMessage == "The Email field is not a valid e-mail address."));
        }

        [TestMethod]
        public void Prezime_Required()
        {
            // Arrange
            var korisnik = new Korisnik();

            // Act
            var results = ValidirajModel(korisnik);

            // Assert
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Prezime") && r.ErrorMessage == "The Prezime field is required."));
        }

        [TestMethod]
        public void Ime_Required()
        {
            // Arrange
            var korisnik = new Korisnik();

            // Act
            var results = ValidirajModel(korisnik);

            // Assert
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Ime") && r.ErrorMessage == "The Ime field is required."));
        }

        [TestMethod]
        public void LozinkaUnos_Required()
        {
            // Arrange
            var korisnik = new Korisnik();

            // Act
            var results = ValidirajModel(korisnik);

            // Assert
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("LozinkaUnos") && r.ErrorMessage == "The Lozinka field is required."));
        }

        [TestMethod]
        public void LozinkaUnos2_Required()
        {
            // Arrange
            var korisnik = new Korisnik();

            // Act
            var results = ValidirajModel(korisnik);

            // Assert
            Assert.IsTrue(results.Any(r => r.MemberNames.Contains("LozinkaUnos2") && r.ErrorMessage == "The Lozinka ponovljena field is required."));
        }

        [TestMethod]
        public void LozinkaUnos2_MatchWithLozinkaUnos()
        {
            // Arrange
            var korisnik = new Korisnik { LozinkaUnos = "password", LozinkaUnos2 = "password123" };

            // Act
            var results = ValidirajModel(korisnik);

            // Assert
            var matchError = results.FirstOrDefault(r => r.MemberNames.Contains("LozinkaUnos2") && r.ErrorMessage == "Lozinke moraju biti jednake");
            Assert.IsNull(matchError, $"Error message: {matchError?.ErrorMessage}");
        }





        [TestMethod]
        public void Korisnik()
        {
            BazaDbContext db = new BazaDbContext();
            Korisnik korisnik = new Korisnik
            {
                KorisnickoIme = "ivan",
                Email = "example@gmail.com",
                Lozinka = "1234",
                Prezime = "Novak",
                Ime = "Ivan",
                SifraOvlasti = "MO",
                LozinkaUnos = "1234",
                LozinkaUnos2 = "1234"
            };
            db.PopisKorisnika.Add(korisnik);
            db.SaveChanges();

            db.PopisKorisnika.Remove(korisnik);
            db.SaveChanges();
        }

        [TestMethod]
        public void Aktivnost_Dodaj_Izbrisi()
        {
            BazaDbContext db = new BazaDbContext();
            Aktivnosti aktivnosti = new Aktivnosti
            {
                ID = 0,
                nazivAktivnosti = "Prva setnja",
                vrstaAktivnosti = VrstaAktivnosti.Hodanje,
                vrijemeTrajanjaAktivnosti = 102,
                duljinaAktivnosti = 20413,
                datumAktivnosti = new DateTime(2024, 05, 21),
                korisnikId = "admin"

            };
            db.PopisAktivnosti.Add(aktivnosti);
            db.SaveChanges();

            db.PopisAktivnosti.Remove(aktivnosti);
            db.SaveChanges();
        }

        [TestMethod]
        public void PlaniranaAktivnost_Dodaj_Izbrisi()
        {
            BazaDbContext db = new BazaDbContext();
            PlaniranaAktivnost aktivnosti = new PlaniranaAktivnost
            {
                ID = 10,
                nazivAktivnosti = "Maraton",
                vrstaAktivnosti = VrstaAktivnosti.Trčanje,
                datumAktivnosti = new DateTime(2024, 06, 15),
                korisnikId = "novi"
            };

            db.PopisPlaniranihAktivnosti.Add(aktivnosti);
            db.SaveChanges();

            db.PopisPlaniranihAktivnosti.Remove(aktivnosti);
            db.SaveChanges();
        }

        [TestMethod]
        public void Aktivnosti_SvaSvojstvaValjana_ValidacijaUspješna()
        {
            // Arrange
            var aktivnosti = new Aktivnosti
            {
                ID = 1,
                nazivAktivnosti = "Test aktivnost",
                vrstaAktivnosti = VrstaAktivnosti.Hodanje,
                vrijemeTrajanjaAktivnosti = 60,
                duljinaAktivnosti = 5000,
                datumAktivnosti = DateTime.Now,
                korisnikId = "testuser"
            };

            // Act
            var rezultati = ValidirajModel(aktivnosti);

            // Assert
            Assert.IsTrue(rezultati.Count == 0);
        }

        [TestMethod]
        public void Aktivnosti_NedostajuObaveznaSvojstva_NeuspješnaValidacija()
        {
            // Arrange
            var aktivnosti = new Aktivnosti(); // Kreiranje instance bez postavljenih obaveznih svojstava

            // Act
            var rezultati = ValidirajModel(aktivnosti);

            // Assert
            Assert.IsTrue(rezultati.Count > 0);
        }

        // Metoda za validaciju modela
        private static System.Collections.Generic.List<ValidationResult> ValidirajModel(object model)
        {
            var validationResults = new System.Collections.Generic.List<ValidationResult>();
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            Validator.TryValidateObject(model, context, validationResults, true);
            return validationResults;
        }

    }
}





