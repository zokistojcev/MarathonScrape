using OddMarathon.Dal.DataAccess.DomainModels;
using OddMarathon.Dal.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddMarathon.Services.BusinessLogic.Odds
{
    public class OddsService : Service<TennisOdd>, IOddsService
    {
        public OddsService(IRepository<TennisOdd> repo) : base(repo)
        {
        }

    }
}
