using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OddMarathon.Dal.DataAccess.DomainModels;
using OddMarathon.Services.Dtos;

namespace OddMarathon.Services.BusinessLogic.Odds
{
    public interface IOddsService: IService<Odd>
    {
        void AddNewOddsTennis(IEnumerable<TennisOddDto> tennisOddDtos);
    }
}
