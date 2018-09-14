using OddMarathon.Dal.DataAccess.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddMarathon.Dal.Repositories.OddsRepository
{
    public interface IOddsRepository : IRepository<Odd>
    {
        Task<List<Odd>> GetOddsBySport();
        Task<List<Odd>> GetOddsBySportFootball();
    }
}
