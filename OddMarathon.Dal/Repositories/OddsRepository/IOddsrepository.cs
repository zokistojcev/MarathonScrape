using OddMarathon.Dal.DataAccess.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddMarathon.Dal.Repositories.OddsRepository
{
    public interface IOddsRepository: IRepository<TennisOdd>
    {
        Task<IEnumerable<TennisOdd>> GetByTournament(string tournament);
    }
}
