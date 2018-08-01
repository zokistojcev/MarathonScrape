namespace OddMarathon.Dal.DataAccess.DomainModels
{
    using System;
    using System.Collections.Generic;
 

    public partial class Odd
    {
  
        public Odd()
        {
            CoefficientsTennis = new HashSet<CoefficientsTennis>();
            CoefficientsFootball = new HashSet<CoefficientsFootball>();
        }

        public int Id { get; set; }

        public string PairOne { get; set; }

        public string PairTwo { get; set; }

        public DateTime BeginingTime { get; set; }

        public string Tournament { get; set; }
        public byte Sport { get; set; }
        
     

        public virtual ICollection<CoefficientsTennis> CoefficientsTennis { get; set; }
        public virtual ICollection<CoefficientsFootball> CoefficientsFootball { get; set; }
    }

}
