namespace OddMarathon.Dal.DataAccess.DomainModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TennisOdd
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TennisOdd()
        {
            Koeficientis = new HashSet<Koeficienti>();
        }

        public int Id { get; set; }

        public string ParOne { get; set; }

        public string ParTwo { get; set; }

        public string DateAndBeginingTime { get; set; }

        public string TurnirDataPocetok { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Koeficienti> Koeficientis { get; set; }
    }
}
