using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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
            return await _context.Set<Odd>().ToListAsync();

            var tttt = "";
        }
     
   
    }
}
