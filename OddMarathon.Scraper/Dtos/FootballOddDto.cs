using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddMarathon.Scraper.Dtos
{
    public class FootballOddDto: BaseOdd
    {
        public string CoefficientHost { get; set; }
        public string CoefficientDraw { get; set; }
        public string CoefficientVisitors { get; set; }
    }
}
