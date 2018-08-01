using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddMarathon.Dal.DataAccess.DomainModels
{
    public class CoefficientsTennis
    {
        public int Id { get; set; }

        public int OddId { get; set; }

        public string CoefficientFirst { get; set; }

        public string CoefficientSecond { get; set; }

        public DateTime DateAndTime { get; set; }

        public virtual Odd Odd { get; set; }
    }
}
