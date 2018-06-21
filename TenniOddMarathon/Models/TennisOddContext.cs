using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TenniOddMarathon.Models
{
    public class TennisOddContext: DbContext
    {

        public TennisOddContext() : base ("TennisOdd")
        {

        }

        public DbSet<TennisOdd> TennisOdd { get; set; }
        public DbSet<Koeficienti> Koeficienti { get; set; }
    }
}