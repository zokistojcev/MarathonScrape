using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddMarathon.Dal.DataAccess.DomainModels
{
    public class CoefficientsFootball
    {
        public int Id { get; set; }

        public int OddId { get; set; }

        public string CoefficientHost { get; set; }

        public string CoefficientDraw { get; set; }

        public string CoefficientVisitor { get; set; }

        public DateTime DateAndTime { get; set; }

        public virtual Odd Odd { get; set; }
    }
}
