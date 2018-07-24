using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TenniOddMarathon.Models
{
    public class OddContext: DbContext
    {

        public OddContext() : base ("TennisOdd")
        {

        }

        public DbSet<BettingOdd> BettingOdds { get; set; }
        public DbSet<KoeficientiTennis> KoeficientiTennis { get; set; }
        public DbSet<KoeficientiFootball> KoeficientiFootball { get; set; }
    }
}