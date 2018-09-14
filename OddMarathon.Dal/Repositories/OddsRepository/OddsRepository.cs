
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using OddMarathon.Dal.DataAccess;
using OddMarathon.Dal.DataAccess.DomainModels;

namespace OddMarathon.Dal.Repositories.OddsRepository
{
    public class OddsRepository : Repository<Odd>, IOddsRepository
    {
        public OddsRepository(OddMarathonContext context) : base(context)
        {
        }

        public async Task<List<Odd>> GetOddsBySport()
        {
            return await _context.Set<Odd>().Include(o => o.CoefficientsTennis).ToListAsync();

        }

        public async Task<List<Odd>> GetOddsBySportFootball()
        {
            return await _context.Set<Odd>().Include(o => o.CoefficientsFootball).ToListAsync();

        }


    }
}
