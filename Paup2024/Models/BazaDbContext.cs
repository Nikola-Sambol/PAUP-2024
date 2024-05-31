using MySql.Data.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PAUP_2024.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class BazaDbContext : DbContext
    {
        public DbSet<Aktivnosti> PopisAktivnosti { get; set; }

        public DbSet<Korisnik> PopisKorisnika { get; set; }

        public DbSet<Ovlast> PopisOvlast { get; set; }

        public DbSet<PlaniranaAktivnost> PopisPlaniranihAktivnosti { get; set; }
    }
}