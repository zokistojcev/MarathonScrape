using OddMarathon.Dal.DataAccess;
using OddMarathon.Dal.DataAccess.DomainModels;
using OddMarathon.Dal.Repositories;
using OddMarathon.Dal.Repositories.OddsRepository;
using OddMarathon.Services.BusinessLogic.Odds;
using System.Runtime.Remoting.Contexts;
using System.Data.Entity;
using Unity;

namespace OddMarathon.Scraper
{
    public static class DependencyInjection
    {
        public static void RegisterDependency(IUnityContainer container)
        {
            container.RegisterType<IOddsRepository, OddsRepository>();
            container.RegisterType<IRepository<Odd>, Repository<Odd>>();
            container.RegisterType<IOddsService, OddsService>();
            container.RegisterType<OddMarathonContext>();
                  

            container.RegisterType<OddRequests>();
            //var config = new MapperConfiguration(c => c.AddProfile(new AutoMapperProfiles()));
            //container.RegisterInstance<IMapper>(config.CreateMapper());
        }
    }
}
