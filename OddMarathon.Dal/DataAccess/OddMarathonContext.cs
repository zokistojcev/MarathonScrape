namespace OddMarathon.Dal.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using OddMarathon.Dal.DataAccess.DomainModels;

    public partial class OddMarathonContext : DbContext
    {
        public OddMarathonContext()
            : base("name=OddMarathonContext")
        {
        }

        public virtual DbSet<Koeficienti> Koeficientis { get; set; }
        public virtual DbSet<TennisOdd> TennisOdds { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
