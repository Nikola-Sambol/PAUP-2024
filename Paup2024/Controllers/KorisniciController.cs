using PAUP_2024.Models;
using PAUP_2024.Razno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace PAUP_2024.Controllers
{
    [Authorize(Roles = OvlastKorisnik.Administrator)]
    public class KorisniciController : Controller
    {
        BazaDbContext bazaPodataka = new BazaDbContext();

        // GET: Korisnici
        public ActionResult Index()
        {
            var listaKorisnika = bazaPodataka.PopisKorisnika.OrderBy(x => x.SifraOvlasti).ThenBy(x => x.Prezime).ToList();
            return View(listaKorisnika);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Prijava(string returnUrl)
        {
            KorisnikPrijava model = new KorisnikPrijava();
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Prijava(KorisnikPrijava model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var korisnikBaza = bazaPodataka.PopisKorisnika.FirstOrDefault(x => x.KorisnickoIme == model.KorisnickoIme);
                if (korisnikBaza != null)
                {
                    var passwordOK = korisnikBaza.Lozinka == Razno.GetPasswordHash.IzracunajHash(model.Lozinka);

                    if (passwordOK)
                    {
                        LogiraniKorisnik prijavljeniKorisnik = new LogiraniKorisnik(korisnikBaza);
                        LogiraniKorisnikSerializeModel serializeModel = new LogiraniKorisnikSerializeModel();
                        serializeModel.CopyFromUser(prijavljeniKorisnik);
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        string korisnickiPodaci = serializer.Serialize(serializeModel);

                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                            1,
                            prijavljeniKorisnik.Identity.Name,
                            DateTime.Now,
                            DateTime.Now.AddDays(1),
                            false,
                            korisnickiPodaci);

                        string ticketEncrypted = FormsAuthentication.Encrypt(authTicket);

                        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, ticketEncrypted);
                        Response.Cookies.Add(cookie);

                        if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("", "Neispravno korisničko ime ili lozinka");
            return View(model);
        }

        [OverrideAuthorization]
        [Authorize]
        public ActionResult Odjava()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Registracija()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Registracija(Korisnik model)
        {
            if (!String.IsNullOrWhiteSpace(model.KorisnickoIme))
            {
                var korImeZauzeto = bazaPodataka.PopisKorisnika.Any(x => x.KorisnickoIme == model.KorisnickoIme);
                if (korImeZauzeto)
                {
                    ModelState.AddModelError("KorisnickoIme", "Korisničko ime je već zauzeto");
                }
            }
            if (!String.IsNullOrWhiteSpace(model.Email))
            {
                var emailZauzet = bazaPodataka.PopisKorisnika.Any(x => x.Email == model.Email);
                if (emailZauzet)
                {
                    ModelState.AddModelError("Email", "Email je već zauzet");
                }
            }

            if (ModelState.IsValid)
            {
                model.Lozinka = Razno.GetPasswordHash.IzracunajHash(model.LozinkaUnos);
                model.SifraOvlasti = "MO";

                bazaPodataka.PopisKorisnika.Add(model);
                bazaPodataka.SaveChanges();

                return View("UspjesnaRegistracija");
            }

            var ovlasti = bazaPodataka.PopisOvlast.OrderBy(x => x.Naziv).ToList();
            ViewBag.Ovlasti = ovlasti;

            return View(model);
        }


    }


}