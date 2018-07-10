namespace OddMarathon.Dal.DataAccess.DomainModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Koeficienti
    {
        public int Id { get; set; }

        public int TennisOddId { get; set; }

        public string KoeficientFirst { get; set; }

        public string KoeficientSecond { get; set; }

        public DateTime DateAndTime { get; set; }

        public virtual TennisOdd TennisOdd { get; set; }
    }
}
