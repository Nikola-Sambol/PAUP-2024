using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PAUP_2024.Razno
{
    public class LogiraniKorisnikSerializeModel
    {
        public string KorisnickoIme { get; set; }
        public string PrezimeIme { get; set; }
        public string Ovlast { get; set; }

        internal void CopyFromUser(LogiraniKorisnik user)
        {
            this.KorisnickoIme = user.KorisnickoIme;
            this.PrezimeIme = user.PrezimeIme;
            this.Ovlast = user.Ovlast;
        }
    }
}