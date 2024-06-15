using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PAUP_2024.Models;
using PAUP_2024.Razno;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PAUP_2024.Controllers
{
    public class PlaniranaAktivnostsController : Controller
    {
        private BazaDbContext db = new BazaDbContext();

        // GET: PlaniranaAktivnosts
        public async Task<ActionResult> Index()
        {


            if (User.Identity.IsAuthenticated)
            {
                await ProvjeriIPosaljiObavijesti();

                if ((User as LogiraniKorisnik).IsInRole(OvlastKorisnik.Administrator))
                {
                    // Ako je ulogiran administrator, dohvati sve aktivnosti
                    await ProvjeriIPosaljiObavijesti();
                    var aktivnosti = db.PopisPlaniranihAktivnosti.ToList();
                    return View(aktivnosti);
                }
                else
                {
                    // Ako je ulogiran obični korisnik, dohvati samo aktivnosti tog korisnika
                    string trenutniKorisnik = (User as LogiraniKorisnik).KorisnickoIme;
                    var aktivnosti = db.PopisPlaniranihAktivnosti.Where(a => a.korisnikId == trenutniKorisnik);
                    return View(aktivnosti);
                }
            }
            else
            {
                // Ako nitko nije ulogiran, vrati praznu listu
                return View(new List<Aktivnosti>());
            }

        }

        // GET: PlaniranaAktivnosts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlaniranaAktivnost planiranaAktivnost = db.PopisPlaniranihAktivnosti.Find(id);
            if (planiranaAktivnost == null)
            {
                return HttpNotFound();
            }
            return View(planiranaAktivnost);
        }

        // GET: PlaniranaAktivnosts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlaniranaAktivnosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ID,nazivAktivnosti,vrstaAktivnosti,datumAktivnosti,korisnikId")] PlaniranaAktivnost planiranaAktivnost)
        {
            if (ModelState.IsValid)
            {
                var trenutniKorisnik = GetKorisnickoIme();
                planiranaAktivnost.korisnikId = trenutniKorisnik;

                db.PopisPlaniranihAktivnosti.Add(planiranaAktivnost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(planiranaAktivnost);
        }

        // GET: PlaniranaAktivnosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlaniranaAktivnost planiranaAktivnost = db.PopisPlaniranihAktivnosti.Find(id);
            if (planiranaAktivnost == null)
            {
                return HttpNotFound();
            }
            return View(planiranaAktivnost);
        }

        // POST: PlaniranaAktivnosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,nazivAktivnosti,vrstaAktivnosti,datumAktivnosti,korisnikId")] PlaniranaAktivnost planiranaAktivnost)
        {
            if (ModelState.IsValid)
            {
                var trenutniKorisnik = GetKorisnickoIme();
                planiranaAktivnost.korisnikId = trenutniKorisnik;

                db.Entry(planiranaAktivnost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(planiranaAktivnost);
        }

        // GET: PlaniranaAktivnosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlaniranaAktivnost planiranaAktivnost = db.PopisPlaniranihAktivnosti.Find(id);
            if (planiranaAktivnost == null)
            {
                return HttpNotFound();
            }
            return View(planiranaAktivnost);
        }

        // POST: PlaniranaAktivnosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PlaniranaAktivnost planiranaAktivnost = db.PopisPlaniranihAktivnosti.Find(id);
            db.PopisPlaniranihAktivnosti.Remove(planiranaAktivnost);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private string GetKorisnickoIme()
        {
            if (User.Identity.IsAuthenticated)
            {
                var korisnickoIme = (User as LogiraniKorisnik).KorisnickoIme;
                var korisnik = db.PopisKorisnika.FirstOrDefault(u => u.KorisnickoIme == korisnickoIme);
                if (korisnik != null)
                {
                    return korisnik.KorisnickoIme;
                }
            }
            return "Niste prijavljeni";
        }

        private string GetKorisnikovEmail(string korisnikId)
        {
            var korisnik = db.PopisKorisnika.FirstOrDefault(u => u.KorisnickoIme == korisnikId);
            if (korisnik != null)
            {
                return korisnik.Email;
            }
            return "Nema email adrese za korisnika";
        }

        // Funkcija za slanje maila korisniku ako se danasnji datum poklapa sa datumom neke aktivnosti
        private async Task ProvjeriIPosaljiObavijesti()
        {
            DateTime currentDate = DateTime.Now.Date;
            var aktivnosti = await db.PopisPlaniranihAktivnosti.ToListAsync();
            var danasnjeAktivnosti = aktivnosti.Where(a => a.datumAktivnosti.Date == currentDate).ToList();

            foreach (var aktivnost in danasnjeAktivnosti)
            {
                if (aktivnost.PoslednjaObavijest == null || aktivnost.PoslednjaObavijest.Value.Date != currentDate)
                {
                    if (currentDate == aktivnost.datumAktivnosti.Date)
                    {
                        string toEmail = GetKorisnikovEmail(aktivnost.korisnikId);
                        string subject = $"Podsjetnik: Planirana aktivnost - {aktivnost.nazivAktivnosti}";
                        string body = $"Poštovani {aktivnost.korisnikId},<br />" +
              $"Želimo vas podsjetiti na nadolazeću aktivnost koja je planirana za {aktivnost.datumAktivnosti.ToString("yyyy-MM-dd")}.<br /> Detalji aktivnosti su sljedeći:<br />" +
              $"Naziv aktivnosti: {aktivnost.nazivAktivnosti}<br />" +
              $"Vrsta aktivnosti: {aktivnost.vrstaAktivnosti}<br />" +
              $"Srdačan pozdrav";


                        string apiKey = "";
                        await SendEmail(apiKey, toEmail, subject, body);

                        // Ažurirajte datum poslednje obavijesti
                        aktivnost.PoslednjaObavijest = DateTime.Now;
                        db.Entry(aktivnost).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                }
            }
        }


        private async Task SendEmail(string apiKey, string toEmail, string subject, string body)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("activitytrackerpaup2024@gmail.com", "Activity Tracker");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, body, body);
            var response = await client.SendEmailAsync(msg);

        }
    }
}
