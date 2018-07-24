using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddMarathon.Scraper.Dtos
{
    public class TennisOddDto: BaseOdd
    {
        
        public string CoefficientFirst { get; set; }
        public string CoefficientSecond { get; set; }
        
    }
}
