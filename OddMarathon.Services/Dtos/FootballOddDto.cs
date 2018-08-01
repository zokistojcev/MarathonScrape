

using OddMarathon.Dal.DataAccess.DomainModels;

namespace OddMarathon.Services.Dtos
{
    public class FootballOddDto: BaseOdd
    {
        public FootballOddDto()
        {
            SportId = Sport.Football;
        }
        public string CoefficientHost { get; set; }
        public string CoefficientDraw { get; set; }
        public string CoefficientVisitors { get; set; }
    }
}
