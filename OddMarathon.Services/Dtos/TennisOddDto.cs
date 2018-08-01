

using OddMarathon.Dal.DataAccess.DomainModels;
using System;

namespace OddMarathon.Services.Dtos
{
    public class TennisOddDto: BaseOdd
    {
        public TennisOddDto()
        {
            SportId = Sport.Tennis;
        }
        public string CoefficientFirst { get; set; }
        public string CoefficientSecond { get; set; }
        public DateTime Time { get; set; }
    }
}
