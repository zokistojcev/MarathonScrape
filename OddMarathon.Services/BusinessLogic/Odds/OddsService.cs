using OddMarathon.Dal.DataAccess.DomainModels;
using OddMarathon.Dal.Repositories;
using OddMarathon.Dal.Repositories.OddsRepository;
using OddMarathon.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OddMarathon.Services.BusinessLogic.Odds
{
    public class OddsService : IOddsService
    {
        private readonly IOddsRepository _oddsRepository;

        public OddsService(IOddsRepository oddsRepository)
        {
            _oddsRepository = oddsRepository;
        }


        public async void AddNewOddsTennis(IEnumerable<TennisOddDto> tennisOddDtos)
        {

            var newPairs = tennisOddDtos;
            var databaseParovi = await _oddsRepository.GetOddsBySport();
           
            // da se zemat od DB
            //var databaseParovi = _oddsRepository.GetAll();
            //var er = DatabaseSport();
            
            
            // ako e prazna listata
            if (databaseParovi.Count() < 1)
            {
                var listaOdNoviParovi2 = new List<Odd>();
                foreach (var ni in newPairs)
                {
                    var par = new Odd
                    {
                        BeginingTime = ni.BeginingTime,
                        Tournament = ni.Tournament,
                        PairOne = ni.PairOne,
                        PairTwo = ni.PairTwo,
                        Sport = ni.SportId



                    };
                    par.CoefficientsTennis = new List<CoefficientsTennis> { new CoefficientsTennis { CoefficientFirst = ni.CoefficientFirst, CoefficientSecond = ni.CoefficientSecond } };
                    databaseParovi.Add(par);
                }
                _oddsRepository.CreateRange(databaseParovi);   
                
                
            }

            //da se prociste spisoko
            var deleteParovi = databaseParovi.Where(y =>
                !newPairs.Any(z => z.PairOne == y.PairOne && z.PairTwo == y.PairTwo)).ToList();


            if (deleteParovi.Count() > 1)
            {
                _oddsRepository.DeleteRange(deleteParovi);               
            }


            //parovi so ostanuvat u db i uslovno im se dodavat koeficeni vo listata
            var paroviSoOstanuvatVoDb = databaseParovi.Where(y =>
                newPairs.Any(z => z.PairOne == y.PairOne && z.PairTwo == y.PairTwo)).ToList();

            var postojatIGiImaNaInternet = newPairs.Where(y =>
                paroviSoOstanuvatVoDb.Any(z => z.PairOne == y.PairOne && z.PairTwo == y.PairTwo)).ToList();

            //vie se novi so nagolo treba da se kladat
            var noviInternet = newPairs.Where(y =>
                !paroviSoOstanuvatVoDb.Any(z => z.PairOne == y.PairOne && z.PairTwo == y.PairTwo)).ToList();


            // SO POSTOJAT DODAVANJE NA KOEF
            foreach (var parSoOstanuve in paroviSoOstanuvatVoDb)
            {
                foreach (var pi in postojatIGiImaNaInternet)
                {

                    var koef = parSoOstanuve.CoefficientsTennis.First();
                    if (koef == null)
                    {
                        break;
                    }
                    if (parSoOstanuve.PairOne == pi.PairOne && parSoOstanuve.PairTwo == pi.PairTwo)
                    {


                        if (koef.CoefficientFirst != pi.CoefficientFirst || koef.CoefficientSecond != pi.CoefficientSecond)
                        {
                            CoefficientsTennis k = new CoefficientsTennis();

                            k.CoefficientFirst = pi.CoefficientFirst;
                            k.CoefficientSecond = pi.CoefficientSecond;
                            k.OddId = parSoOstanuve.Id;
                            k.DateAndTime = pi.Time;
                            

                            parSoOstanuve.CoefficientsTennis.Add(k);

                        }
                    }

                }
                
            }


            //DODAVANJE NA NOVI

            if (noviInternet.Count() > 1)
            {
                var listaOdNoviParovi = new List<Odd>();
                foreach (var ni in noviInternet)
                {
                    var par = new Odd
                    {
                        BeginingTime = ni.BeginingTime,
                        Tournament = ni.Tournament,
                        PairOne = ni.PairOne,
                        PairTwo = ni.PairTwo,
                        Sport = ni.SportId

                    };
                    par.CoefficientsTennis = new List<CoefficientsTennis> { new CoefficientsTennis { CoefficientFirst = ni.CoefficientFirst, CoefficientSecond = ni.CoefficientSecond, DateAndTime = ni.Time } };
                    listaOdNoviParovi.Add(par);
                    _oddsRepository.CreateRange(listaOdNoviParovi);
                }
              
            }

            //var yy = _context.TennisOdd.ToList();

            var g = _oddsRepository.GetOddsBySport();
            
           
        }




        public Task<int> Count(Expression<Func<Odd, bool>> predicate)
        {
            return _oddsRepository.Count(predicate);
        }

        public void Create(Odd entity)
        {
            _oddsRepository.Create(entity);
        }

        public void Delete(Odd entity)
        {
            _oddsRepository.Delete(entity);
        }

        public Task<IEnumerable<Odd>> Find(Expression<Func<Odd, bool>> predicate)
        {
            return _oddsRepository.Find(predicate);
        }

        public Task<IEnumerable<Odd>> GetAll()
        {
            return _oddsRepository.GetAll();
        }

        public Task<Odd> GetById(int id)
        {
            return _oddsRepository.GetById(id);
        }

        public void Update(Odd entity)
        {
            _oddsRepository.Update(entity);
        }
    }
}
