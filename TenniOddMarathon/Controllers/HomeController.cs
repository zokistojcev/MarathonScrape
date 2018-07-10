using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TenniOddMarathon.Models;
using TenniOddMarathon.ViewModels;
using System;


namespace TenniOddMarathon.Controllers
{
    public class HomeController : Controller
    {

        private TennisOddContext _context;


        public HomeController()
        {
            _context = new TennisOddContext();

        }

        public async Task<ActionResult> Index()
        {
            var t = await Mains();
            Thread.Sleep(3000);
            return View(t);
        }

        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();

        }

        public async Task<List<TennisOdd>> Mains()
        {
            // Da se zemat od internet  
            var internetParovi = await BetMarathon();

            // da se zemat od DB
            var databaseParovi = _context.TennisOdd.Include(o => o.Koefecienti).ToList();

            // ako e prazna listata
            if (databaseParovi.Count() < 1)
            {
                var listaOdNoviParovi2 = new List<TennisOdd>();
                foreach (var ni in internetParovi)
                {
                    var par = new TennisOdd
                    {
                        DateAndBeginingTime = ni.DateAndBeginingTime,
                        TurnirDataPocetok = ni.TurnirDataPocetok,
                        ParOne = ni.ParOne,
                        ParTwo = ni.ParTwo,
                        Koefecienti = new List<Koeficienti> { new Koeficienti { KoeficientFirst = ni.KoeficientFirst, KoeficientSecond = ni.KoeficientSecond, DateAndTime = ni.DateAndTime} }
                    };

                    databaseParovi.Add(par);
                }
                _context.TennisOdd.AddRange(databaseParovi);
                _context.SaveChanges();

                return databaseParovi.ToList();
            }

            //da se prociste spisoko
            var deleteParovi = databaseParovi.Where(y =>
                !internetParovi.Any(z => z.ParOne == y.ParOne && z.ParTwo == y.ParTwo)).ToList();


            if (deleteParovi.Count() > 1)
            {
                _context.TennisOdd.RemoveRange(deleteParovi);
                _context.SaveChanges();
            }


            //parovi so ostanuvat u db i uslovno im se dodavat koeficeni vo listata
            var paroviSoOstanuvatVoDb = databaseParovi.Where(y =>
                internetParovi.Any(z => z.ParOne == y.ParOne && z.ParTwo == y.ParTwo)).ToList();

            var postojatIGiImaNaInternet = internetParovi.Where(y =>
                paroviSoOstanuvatVoDb.Any(z => z.ParOne == y.ParOne && z.ParTwo == y.ParTwo)).ToList();

            //vie se novi so nagolo treba da se kladat
            var noviInternet = internetParovi.Where(y =>
                !paroviSoOstanuvatVoDb.Any(z => z.ParOne == y.ParOne && z.ParTwo == y.ParTwo)).ToList();
           

            // SO POSTOJAT DODAVANJE NA KOEF
            foreach (var parSoOstanuve in paroviSoOstanuvatVoDb)
            {
                foreach (var pi in postojatIGiImaNaInternet)
                {
                    
                    var koef = parSoOstanuve.Koefecienti.First();
                    if (koef==null)
                    {
                        break;
                    }
                    if (parSoOstanuve.ParOne == pi.ParOne && parSoOstanuve.ParTwo == pi.ParTwo)
                    {

                       
                            if (koef.KoeficientFirst != pi.KoeficientFirst || koef.KoeficientSecond != pi.KoeficientSecond)
                            {
                                Koeficienti k = new Koeficienti();

                                k.KoeficientFirst = pi.KoeficientFirst;
                                k.KoeficientSecond = pi.KoeficientSecond;
                                k.TennisOddId = parSoOstanuve.Id;
                                k.DateAndTime = pi.DateAndTime;
                                parSoOstanuve.Koefecienti.Add(k);
  
                            }
                        }                                                            

                }
            }
           

            //DODAVANJE NA NOVI

            if (noviInternet.Count() > 1)
            {
                var listaOdNoviParovi = new List<TennisOdd>();
                foreach (var ni in noviInternet)
                {
                    var par = new TennisOdd
                    {
                        DateAndBeginingTime = ni.DateAndBeginingTime,
                        TurnirDataPocetok = ni.TurnirDataPocetok,
                        ParOne = ni.ParOne,
                        ParTwo = ni.ParTwo,
                        Koefecienti = new List<Koeficienti> { new Koeficienti { KoeficientFirst = ni.KoeficientFirst, KoeficientSecond = ni.KoeficientSecond, DateAndTime = ni.DateAndTime } }
                    };

                    listaOdNoviParovi.Add(par);
                    _context.TennisOdd.AddRange(listaOdNoviParovi);
                }
                _context.SaveChanges();
            }

            //var yy = _context.TennisOdd.ToList();
            _context.SaveChanges();
            var g = _context.TennisOdd.ToList();
            return g;
        }

        public async Task<List<TennisOddViewModel>> BetMarathon()
        {
            //IWebDriver driver = new ChromeDriver();

            //var url = "https://www.marathonbet.com/en/betting/Basketball/?menu=6";
            //var url = "https://www.marathonbet.com/en/betting/Tennis/?menu=2398";
            var url = "https://www.marathonbet.com/en/betting/Baseball/?menu=5";
            //var url = "https://www.marathonbet.com/en/betting/Snooker/?menu=2185";
            //var url = "https://www.marathonbet.com/en/betting/Darts/?menu=9";
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(html);

            var container = htmlDoc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("category-container")).ToList();


            List<TennisOddViewModel> lto = new List<TennisOddViewModel>();
            //List<TennisOdd> lto = new List<TennisOdd>();
            foreach (var item in container)
            {


                var tdp = item.Descendants("H2")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("category-label"))
                .FirstOrDefault()
                .InnerText;

                if (tdp == "Outright")
                {
                    break;
                }
                //string turnirDataPocetok = tdp;

                var category_content = item.Descendants("tr").Where(node => node.GetAttributeValue("class", "").Equals(" event-header")).ToList();

                foreach (var item2 in category_content)
                {
                    TennisOddViewModel to = new TennisOddViewModel();


                    to.TurnirDataPocetok = tdp;


                    var ParOneToday = item2.Descendants("td")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("today-name"));

                    var DayAndHour = item2.Descendants("td")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("date "))
                            .FirstOrDefault().InnerText.Trim('\n', ' ');

                    if (ParOneToday.Count() > 0)
                    {
                        var uu = ParOneToday.FirstOrDefault().InnerText.Trim('\n', ' ', '1', '2', '.');
                        to.ParOne = uu;
                        var uq = ParOneToday.LastOrDefault().InnerText.Trim('\n', ' ', '1', '2', '.');
                        to.ParTwo = uq;


                        var koeficientFirst = item2.Descendants("td")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("price height-column-with-price    first-in-main-row  "))
                            .FirstOrDefault().InnerText.Trim('\n', ' ');
                        var koeficientSecond = item2.Descendants("td")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("price height-column-with-price    "))
                            .FirstOrDefault().InnerText.Trim('\n', ' ');



                        to.KoeficientFirst = koeficientFirst;
                        to.KoeficientSecond = koeficientSecond;
                        to.DateAndBeginingTime = DayAndHour;
                        to.DateAndTime = DateTime.Now;



                    }
                    else
                    {
                        string ParOneSoon = item2.Descendants("td")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("name")).FirstOrDefault()
                        .InnerText.Trim('\n', ' ', '1', '2', '.');
                        to.ParOne = ParOneSoon;

                        var ParTwoSoon = item2.Descendants("td")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("name")).LastOrDefault()
                         .InnerText.Trim('\n', ' ', '1', '2', '.');
                        to.ParTwo = ParTwoSoon;

                        var koeficientFirst = item2.Descendants("td")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("price height-column-with-price    first-in-main-row  "))
                            .LastOrDefault().InnerText.Trim('\n', ' ');
                        var koeficientSecond = item2.Descendants("td")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("price height-column-with-price    "))
                            .FirstOrDefault().InnerText.Trim('\n', ' ');

                        to.KoeficientFirst = koeficientFirst;
                        to.KoeficientSecond = koeficientSecond;
                        to.DateAndBeginingTime = DayAndHour;
                        to.DateAndTime = DateTime.Now;


                    }
                    lto.Add(to);
                };


            }
            return lto;

        }


        public async Task<List<TennisOddViewModel>> BetMarathonFootball()
        {
            //IWebDriver driver = new ChromeDriver();

            var url = "https://www.marathonbet.com/en/betting/Basketball/?menu=6";     
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(html);

            var container = htmlDoc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("category-container")).ToList();


            List<TennisOddViewModel> lto = new List<TennisOddViewModel>();
            //List<TennisOdd> lto = new List<TennisOdd>();
            foreach (var item in container)
            {


                var tdp = item.Descendants("H2")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("category-label"))
                .FirstOrDefault()
                .InnerText;

                if (tdp == "Outright")
                {
                    break;
                }
                //string turnirDataPocetok = tdp;

                var category_content = item.Descendants("tr").Where(node => node.GetAttributeValue("class", "").Equals(" event-header")).ToList();

                foreach (var item2 in category_content)
                {
                    TennisOddViewModel to = new TennisOddViewModel();


                    to.TurnirDataPocetok = tdp;


                    var ParOneToday = item2.Descendants("td")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("today-name"));

                    var DayAndHour = item2.Descendants("td")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("date "))
                            .FirstOrDefault().InnerText.Trim('\n', ' ');

                    if (ParOneToday.Count() > 0)
                    {
                        var uu = ParOneToday.FirstOrDefault().InnerText.Trim('\n', ' ', '1', '2', '.');
                        to.ParOne = uu;
                        var uq = ParOneToday.LastOrDefault().InnerText.Trim('\n', ' ', '1', '2', '.');
                        to.ParTwo = uq;


                        var koeficientFirst = item2.Descendants("td")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("price height-column-with-price    first-in-main-row  "))
                            .FirstOrDefault().InnerText.Trim('\n', ' ');
                        var koeficientSecond = item2.Descendants("td")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("price height-column-with-price    "))
                            .FirstOrDefault().InnerText.Trim('\n', ' ');



                        to.KoeficientFirst = koeficientFirst;
                        to.KoeficientSecond = koeficientSecond;
                        to.DateAndBeginingTime = DayAndHour;
                        to.DateAndTime = DateTime.Now;



                    }
                    else
                    {
                        string ParOneSoon = item2.Descendants("td")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("name")).FirstOrDefault()
                        .InnerText.Trim('\n', ' ', '1', '2', '.');
                        to.ParOne = ParOneSoon;

                        var ParTwoSoon = item2.Descendants("td")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("name")).LastOrDefault()
                         .InnerText.Trim('\n', ' ', '1', '2', '.');
                        to.ParTwo = ParTwoSoon;

                        var koeficientFirst = item2.Descendants("td")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("price height-column-with-price    first-in-main-row  "))
                            .LastOrDefault().InnerText.Trim('\n', ' ');
                        var koeficientSecond = item2.Descendants("td")
                            .Where(node => node.GetAttributeValue("class", "")
                            .Equals("price height-column-with-price    "))
                            .FirstOrDefault().InnerText.Trim('\n', ' ');

                        to.KoeficientFirst = koeficientFirst;
                        to.KoeficientSecond = koeficientSecond;
                        to.DateAndBeginingTime = DayAndHour;
                        to.DateAndTime = DateTime.Now;


                    }
                    lto.Add(to);
                };


            }
            return lto;

        }
    }
}
