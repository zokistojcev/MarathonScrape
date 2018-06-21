using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TenniOddMarathon.Models;
using TenniOddMarathon.ViewModels;

namespace TenniOddMarathon.Controllers
{
    public class HomeController : Controller
    {

        private TennisOddContext _context;

        //aasda
        //sdsdfs
        public HomeController()
        {
            _context = new TennisOddContext();

        }



        public async Task<ActionResult> Index()
        {


            //var t = await BetMarathon();
            var t = await Zoki();
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

        public async Task<List<TennisOddViewModel>> BetMarathon()
        {
            IWebDriver driver = new ChromeDriver();

            var url = "https://www.marathonbet.com/en/betting/Tennis/?menu=2398";
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
                       

                    }
                    lto.Add(to);
                };


            }
            return lto;

        }

        public async Task<List<TennisOdd>> Zoki()
        {
            var databaseParovi = _context.TennisOdd.ToList();
            var noviParovi = await BetMarathon();

            

            foreach (var novPar in noviParovi)
            {

                //var databasaPar = databaseParovi.Where(x => x.ParOne == novPar.ParOne && x.ParTwo == novPar.ParTwo).FirstOrDefault();
                //if (databasaPar==null)
                //{
                //    databaseParovi.Add(novPar);
                //}
                //else
                //{
                //    var k1 = databasaPar.Koefecienti.LastOrDefault().KoeficientFirst;
                //    var k2 = databasaPar.Koefecienti.LastOrDefault().KoeficientSecond;
                //    if (k1==novPar)
                //    {

                //    }
                //}
                bool isExisting = false;
                foreach (var databasePar in databaseParovi)
                {

                    

                    if (novPar.ParOne == databasePar.ParOne && novPar.ParTwo == databasePar.ParTwo)
                    {
                        var posledenKoeficient = databasePar.Koefecienti.LastOrDefault();
                        var fKoeficient = novPar.KoeficientFirst;
                        var sKoeficient = novPar.KoeficientSecond;
                        isExisting = true;
                       
                        break;
                    }
                    


                }

                if (!isExisting)
                {
                    TennisOdd too = new TennisOdd();
                    too.ParOne = novPar.ParOne;
                    too.ParTwo = novPar.ParTwo;
                    too.TurnirDataPocetok = novPar.TurnirDataPocetok;
                    too.DateAndBeginingTime = novPar.DateAndBeginingTime;

                    List<Koeficienti> lk = new List<Koeficienti>();
                    too.Koefecienti = lk;
                    Koeficienti k = new Koeficienti();
                    k.KoeficientFirst = novPar.KoeficientFirst;
                    k.KoeficientSecond = novPar.KoeficientSecond;
                    too.Koefecienti.Add(k);

                    databaseParovi.Add(too);
                }
                

            }
            _context.SaveChanges();
            return databaseParovi;
        }
    }
}