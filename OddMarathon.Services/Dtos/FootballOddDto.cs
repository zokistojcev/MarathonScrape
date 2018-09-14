

using OddMarathon.Dal.DataAccess.DomainModels;
using System;

namespace OddMarathon.Services.Dtos
{
    public class FootballOddDto: BaseOdd
    {
        public FootballOddDto()
        {
            
        }
        public string CoefficientHost { get; set; }
        public string CoefficientDraw { get; set; }
        public string CoefficientVisitors { get; set; }
        public int OddId { get; set; }
        public DateTime Time { get; set; }
    }
}
