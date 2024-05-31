using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PAUP_2024.Models;
using PAUP_2024.Razno;

namespace PAUP_2024.Controllers
{
    public class AktivnostisController : Controller
    {
        public BazaDbContext db = new BazaDbContext();

        // GET: Aktivnostis
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if ((User as LogiraniKorisnik).IsInRole(OvlastKorisnik.Administrator))
                {
                    // Ako je ulogiran administrator, dohvati sve aktivnosti
                    var sveAktivnosti = db.PopisAktivnosti.ToList();
                    return View(sveAktivnosti);
                }
                else
                {
                    // Ako je ulogiran obični korisnik, dohvati samo aktivnosti tog korisnika
                    string currentUser = (User as LogiraniKorisnik).KorisnickoIme;
                    var korisnikoveAktivnosti = db.PopisAktivnosti.Where(a => a.korisnikId == currentUser).ToList();
                    return View(korisnikoveAktivnosti);
                }
            }
            else
            {
                // Ako nitko nije ulogiran, vrati praznu listu
                return View(new List<Aktivnosti>());
            }
        }

        // GET: Aktivnostis/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aktivnosti aktivnosti = db.PopisAktivnosti.Find(id);
            if (aktivnosti == null)
            {
                return HttpNotFound();
            }
            return View(aktivnosti);
        }

        // GET: Aktivnostis/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Aktivnostis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,nazivAktivnosti,vrstaAktivnosti,vrijemeTrajanjaAktivnosti,duljinaAktivnosti,datumAktivnosti")] Aktivnosti aktivnosti)
        {
            if (ModelState.IsValid)
            {
                db.PopisAktivnosti.Add(aktivnosti);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aktivnosti);
        }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ID,nazivAktivnosti,vrstaAktivnosti,vrijemeTrajanjaAktivnosti,duljinaAktivnosti,datumAktivnosti,korisnikId")] Aktivnosti aktivnosti)
        {
            if (ModelState.IsValid)
            {
                var trenutniKorisnik = GetKorisnickoIme();

                aktivnosti.korisnikId = trenutniKorisnik;

                db.PopisAktivnosti.Add(aktivnosti);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aktivnosti);
        }


        // GET: Aktivnostis/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aktivnosti aktivnosti = db.PopisAktivnosti.Find(id);
            if (aktivnosti == null)
            {
                return HttpNotFound();
            }
            return View(aktivnosti);
        }

        // POST: Aktivnostis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,nazivAktivnosti,vrstaAktivnosti,vrijemeTrajanjaAktivnosti,duljinaAktivnosti,datumAktivnosti")] Aktivnosti aktivnosti)
        {
            if (ModelState.IsValid)
            {
                var trenutniKorisnik = GetKorisnickoIme();

                aktivnosti.korisnikId = trenutniKorisnik;

                db.Entry(aktivnosti).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aktivnosti);
        }

        // GET: Aktivnostis/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Aktivnosti aktivnosti = db.PopisAktivnosti.Find(id);
            if (aktivnosti == null)
            {
                return HttpNotFound();
            }
            return View(aktivnosti);
        }

        // POST: Aktivnostis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Aktivnosti aktivnosti = db.PopisAktivnosti.Find(id);
            db.PopisAktivnosti.Remove(aktivnosti);
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

        public ActionResult PopisAktivnosti()
        {
            return View();
        }


        public ActionResult Hodanje(string sortOrder, DateTime? startDate, DateTime? endDate)
        {
            if (User.Identity.IsAuthenticated)
            {
                if ((User as LogiraniKorisnik).IsInRole(OvlastKorisnik.Administrator))
                {
                    // Ako je ulogiran administrator, dohvati sve aktivnosti
                    var aktivnosti = db.PopisAktivnosti.Where(a => a.vrstaAktivnosti == VrstaAktivnosti.Hodanje).ToList();
                    return View(Sortiraj(sortOrder, aktivnosti, startDate, endDate));
                }
                else
                {
                    // Ako je ulogiran obični korisnik, dohvati samo aktivnosti tog korisnika
                    string trenutniKorisnik = (User as LogiraniKorisnik).KorisnickoIme;
                    var aktivnosti = db.PopisAktivnosti.Where(a => a.vrstaAktivnosti == VrstaAktivnosti.Hodanje && a.korisnikId == trenutniKorisnik).ToList();
                    return View(Sortiraj(sortOrder, aktivnosti, startDate, endDate));
                }
            }
            else
            {
                // Ako nitko nije ulogiran, vrati praznu listu
                return View(new List<Aktivnosti>());
            }
        }

        public ActionResult Trcanje(string sortOrder, DateTime? startDate, DateTime? endDate)
        {
            if (User.Identity.IsAuthenticated)
            {
                if ((User as LogiraniKorisnik).IsInRole(OvlastKorisnik.Administrator))
                {
                    // Ako je ulogiran administrator, dohvati sve aktivnosti
                    var aktivnosti = db.PopisAktivnosti.Where(a => a.vrstaAktivnosti == VrstaAktivnosti.Trčanje).ToList();
                    return View(Sortiraj(sortOrder, aktivnosti, startDate, endDate));
                }
                else
                {
                    // Ako je ulogiran obični korisnik, dohvati samo aktivnosti tog korisnika
                    string trenutniKorisnik = (User as LogiraniKorisnik).KorisnickoIme;
                    var aktivnosti = db.PopisAktivnosti.Where(a => a.vrstaAktivnosti == VrstaAktivnosti.Trčanje && a.korisnikId == trenutniKorisnik).ToList();
                    return View(Sortiraj(sortOrder, aktivnosti, startDate, endDate));
                }
            }
            else
            {
                // Ako nitko nije ulogiran, vrati praznu listu
                return View(new List<Aktivnosti>());
            }
        }

        public ActionResult Biciklizam(string sortOrder, DateTime? startDate, DateTime? endDate)
        {
            if (User.Identity.IsAuthenticated)
            {
                if ((User as LogiraniKorisnik).IsInRole(OvlastKorisnik.Administrator))
                {
                    // Ako je ulogiran administrator, dohvati sve aktivnosti
                    var aktivnosti = db.PopisAktivnosti.Where(a => a.vrstaAktivnosti == VrstaAktivnosti.Biciklizam).ToList();
                    return View(Sortiraj(sortOrder, aktivnosti, startDate, endDate));
                }
                else
                {
                    // Ako je ulogiran obični korisnik, dohvati samo aktivnosti tog korisnika
                    string trenutniKorisnik = (User as LogiraniKorisnik).KorisnickoIme;
                    var aktivnosti = db.PopisAktivnosti.Where(a => a.vrstaAktivnosti == VrstaAktivnosti.Biciklizam && a.korisnikId == trenutniKorisnik).ToList();
                    return View(Sortiraj(sortOrder, aktivnosti, startDate, endDate));
                }
            }
            else
            {
                // Ako nitko nije ulogiran, vrati praznu listu
                return View(new List<Aktivnosti>());
            }
        }

        public ActionResult Plivanje(string sortOrder, DateTime? startDate, DateTime? endDate)
        {
            if (User.Identity.IsAuthenticated)
            {
                if ((User as LogiraniKorisnik).IsInRole(OvlastKorisnik.Administrator))
                {
                    // Ako je ulogiran administrator, dohvati sve aktivnosti
                    var aktivnosti = db.PopisAktivnosti.Where(a => a.vrstaAktivnosti == VrstaAktivnosti.Plivanje).ToList();
                    return View(Sortiraj(sortOrder, aktivnosti, startDate, endDate));
                }
                else
                {
                    // Ako je ulogiran obični korisnik, dohvati samo aktivnosti tog korisnika
                    string trenutniKorisnik = (User as LogiraniKorisnik).KorisnickoIme;
                    var aktivnosti = db.PopisAktivnosti.Where(a => a.vrstaAktivnosti == VrstaAktivnosti.Plivanje && a.korisnikId == trenutniKorisnik).ToList();
                    return View(Sortiraj(sortOrder, aktivnosti, startDate, endDate));
                }
            }
            else
            {
                // Ako nitko nije ulogiran, vrati praznu listu
                return View(new List<Aktivnosti>());
            }
        }

        public ActionResult Planinarenje(string sortOrder, DateTime? startDate, DateTime? endDate)
        {
            if (User.Identity.IsAuthenticated)
            {
                if ((User as LogiraniKorisnik).IsInRole(OvlastKorisnik.Administrator))
                {
                    // Ako je ulogiran administrator, dohvati sve aktivnosti
                    var Aktivnosti = db.PopisAktivnosti.Where(a => a.vrstaAktivnosti == VrstaAktivnosti.Planinarenje).ToList();
                    return View(Aktivnosti);
                }
                else
                {
                    // Ako je ulogiran obični korisnik, dohvati samo aktivnosti tog korisnika
                    string trenutniKorisnik = (User as LogiraniKorisnik).KorisnickoIme;
                    var korisnikoveAktivnosti = db.PopisAktivnosti.Where(a => a.vrstaAktivnosti == VrstaAktivnosti.Planinarenje && a.korisnikId == trenutniKorisnik).ToList();
                    return View(korisnikoveAktivnosti);
                }
            }
            else
            {
                // Ako nitko nije ulogiran, vrati praznu listu
                return View(new List<Aktivnosti>());
            }
        }

        private List<Aktivnosti> Sortiraj(string sortOrder, List<Aktivnosti> Aktivnosti, DateTime? startDate = null, DateTime? endDate = null)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NazivSortParm = String.IsNullOrEmpty(sortOrder) ? "Naziv_desc" : "";
            ViewBag.TrajanjeSortParm = sortOrder == "Trajanje" ? "Trajanje_desc" : "Trajanje";
            ViewBag.DuljinaSortParm = sortOrder == "Duljina" ? "Duljina_desc" : "Duljina";
            ViewBag.DatumSortParm = sortOrder == "Datum" ? "Datum_desc" : "Datum";
            ViewBag.KorisnikSortParm = sortOrder == "Korisnik" ? "Korisnik_desc" : "Korisnik";

            if (startDate.HasValue && endDate.HasValue)
            {
                Aktivnosti = Aktivnosti.Where(a => a.datumAktivnosti >= startDate.Value && a.datumAktivnosti <= endDate.Value).ToList();
            }

            switch (sortOrder)
            {
                case "Naziv":
                    return Aktivnosti.OrderBy(a => a.nazivAktivnosti).ToList();
                case "Naziv_desc":
                    return Aktivnosti.OrderByDescending(a => a.nazivAktivnosti).ToList();
                case "Trajanje":
                    return Aktivnosti.OrderBy(a => a.vrijemeTrajanjaAktivnosti).ToList();
                case "Trajanje_desc":
                    return Aktivnosti.OrderByDescending(a => a.vrijemeTrajanjaAktivnosti).ToList();
                case "Duljina":
                    return Aktivnosti.OrderBy(a => a.duljinaAktivnosti).ToList();
                case "Duljina_desc":
                    return Aktivnosti.OrderByDescending(a => a.duljinaAktivnosti).ToList();
                case "Datum":
                    return Aktivnosti.OrderBy(a => a.datumAktivnosti).ToList();
                case "Datum_desc":
                    return Aktivnosti.OrderByDescending(a => a.datumAktivnosti).ToList();
                case "Korisnik":
                    return Aktivnosti.OrderBy(a => a.korisnikId).ToList();
                case "Korisnik_desc":
                    return Aktivnosti.OrderByDescending(a => a.korisnikId).ToList();
                default:
                    return Aktivnosti.OrderBy(a => a.nazivAktivnosti).ToList();
            }
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


    }
}
