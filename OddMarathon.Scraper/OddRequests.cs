using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using OddMarathon.Dal.DataAccess.DomainModels;
using OddMarathon.Services.BusinessLogic.Odds;
using OddMarathon.Services.Dtos;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using NUnit.Framework;
using System.Threading;
using mshtml;
using CrossPlatformLibrary.WebBrowser;
using System.Collections;
using OpenQA.Selenium;
using NReco.PhantomJS;
using System.Windows.Forms;
using System.Xml;

namespace OddMarathon.Scraper
{
    public class OddRequests
    {
        private readonly IOddsService _oddsService;


        public OddRequests(IOddsService oddsService)
        {
            _oddsService = oddsService;

        }

        //public async Task<List<TennisOddDto>> GetTennisOdds(string url)
        public async Task<List<TennisOddDto>> GetTennisOdds(string url)
        {

            var htmlDoc = await GetHtmlDocument(url);


            var containers = htmlDoc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                    .Equals("category-container"))
                .ToList();



            var containersClean = new List<HtmlNode>();
            var containersWithDuplicate = new List<HtmlNode>();

            foreach (var item in containers)
            {
                var duplicate = item.SelectNodes("div[contains(@class,'category-content')]/div[contains(@class,'foot-market-border')]/div[contains(@class,'category-container')]");
                if (duplicate == null)
                {

                    containersClean.Add(item);
                }
                else
                {
                    containersWithDuplicate.Add(item);
                }
            }

            var containersEdited = new List<HtmlNode>();

            foreach (var item in containersWithDuplicate)
            {
                var divsToBeDeleted = item.SelectNodes("div[contains(@class,'category-content')]/div[contains(@class,'foot-market-border')]/div[contains(@class,'category-container')]");
                if (divsToBeDeleted != null)
                {
                    foreach (var item3 in divsToBeDeleted)
                    {

                        item.SelectSingleNode("div[contains(@class,'category-content')]/div[contains(@class,'foot-market-border')]").RemoveChild(item3, false);
                    }

                }
                containersEdited.Add(item);
            }
            containersClean.AddRange(containersEdited);



            List<TennisOddDto> listTennisOdds = new List<TennisOddDto>();

            foreach (var section in containersClean)
            {
                //var tableSectionTitle = section.Descendants("H2")
                //    .Where(node => node.GetAttributeValue("class", "")
                //        .Equals("category-label"))
                //    .FirstOrDefault()
                //    .InnerText;


                var tableH = section.Descendants("H2").Where(node => node.GetAttributeValue("class", "")
                        .Equals("category-label"))
                    .FirstOrDefault();
                if (tableH == null)
                {
                    tableH = section.Descendants("H1").Where(node => node.GetAttributeValue("class", "")
                    .Equals("category-label"))
                .FirstOrDefault(); ;
                }
                var tableSectionTitle = tableH.InnerText;




                var resultString = Regex.Replace(tableSectionTitle, @"^((?:\S+\s+){0}\S+).*", "${1}", RegexOptions.Multiline);

                if (resultString == "Outright." || resultString == "Outright")
                {
                    continue;
                }


                var oddsPerSection = section.Descendants("tbody").ToList();
                //var oddsPerSection = section.Descendants("tr")
                //    .Where(node => node.GetAttributeValue("class", "")
                //        .Equals(" event-header"))
                //    .ToList();

                foreach (var odd in oddsPerSection)
                {
                    TennisOddDto tennisOdd = new TennisOddDto
                    {
                        Tournament = tableSectionTitle
                    };

                    var match = odd.Descendants("td")
                        .Where(node =>
                            node.GetAttributeValue("class", "").Equals("today-name")
                            ||
                            node.GetAttributeValue("class", "").Equals("name"));

                    var dayAndHour = odd.Descendants("td")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("date "))
                        .FirstOrDefault().InnerText.Trim('\n', ' ');


                    bool isValidData = DateTime.TryParse(dayAndHour, out DateTime data);

                    if (!isValidData)
                    {
                        String inputString = dayAndHour;

                        inputString = Regex.Replace(inputString, " \\(.*\\)$", "");

                        data = DateTime.ParseExact(inputString, "dd MMM HH:mm",
                            System.Globalization.CultureInfo.InvariantCulture);

                    }


                    tennisOdd.BeginingTime = data;

                    var pairOneTitle = match
                        .First()
                        .InnerText.Trim('\n', ' ', '1', '2', '.');

                    var pairTwoTitle = match
                        .Last()
                        .InnerText.Trim('\n', ' ', '1', '2', '.');

                    tennisOdd.PairOne = pairOneTitle;
                    tennisOdd.PairTwo = pairTwoTitle;


                    string coefficientFirst = "";
                    var cf = odd.Descendants("td")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("price height-column-with-price    first-in-main-row  "))
                        .FirstOrDefault();

                    if (cf == null)
                    {
                        coefficientFirst = "1";
                    }
                    else
                    {
                        coefficientFirst = cf.InnerText.Trim('\n', ' ');
                    }


                    string coefficientSecond = "";
                    var cs = odd.Descendants("td")
                       .Where(node => node.GetAttributeValue("class", "")
                        .Equals("price height-column-with-price    "))
                        .FirstOrDefault();

                    if (cs == null)
                    {
                        coefficientSecond = "1";
                    }
                    else
                    {
                        coefficientSecond = cs.InnerText.Trim('\n', ' ');
                    }



                    //var ee = coefficientFirstt.InnerText;
                    //var coefficientSecond = odd.Descendants("td")
                    //    .Where(node => node.GetAttributeValue("class", "")
                    //    .Equals("price height-column-with-price    "))
                    //    .FirstOrDefault().InnerText.Trim('\n', ' '); ;


                    //}
                    //if (coefficientSecond == "—")
                    //{
                    //    coefficientSecond = "1";
                    //}
                    tennisOdd.CoefficientFirst = coefficientFirst;
                    tennisOdd.CoefficientSecond = coefficientSecond;

                    tennisOdd.Time = DateTime.Now;

                    listTennisOdds.Add(tennisOdd);
                };
            }



            _oddsService.AddNewOddsTennis(listTennisOdds);
            return listTennisOdds;
        }

        public async Task<List<FootballOddDto>> GetFootballsOdds(string url, string sport)
        {
            var htmlDoc = await GetHtmlDocument(url);
         
            var containers = htmlDoc.DocumentNode.Descendants("div").Where(t => t.GetAttributeValue("class", "").Equals("category-container"));
            
            var containersWithoutDuplicate = new List<HtmlNode>();
            var containersWithDuplicate = new List<HtmlNode>();

            foreach (var item in containers) 
            {
                var duplicate2 = item.SelectNodes("div[contains(@class,'category-content')]/div[contains(@class,'foot-market-border')]/div[contains(@class,'foot-market')]/div[contains(@class,' coupon-row')]/div[contains(@class,'category-container')]");
                var duplicat42 = item.SelectNodes("div[contains(@class,'bg coupon-row')]/div[contains(@class,'category-container')]");
                var duplicat424 = item.SelectNodes("*[contains(@class, 'category-content')]");
                var duplicate = item.SelectNodes("div[contains(@class,'category-content')]/div[contains(@class,'foot-market-border')]/div[contains(@class,'foot-market')]/div[contains(@class,' coupon-row')]/div[contains(@class,'category-container')]");
                var duplicate3 = item.SelectNodes("div[contains(@class,'category-content')]/div[contains(@class,'foot-market-border')]");
                var duplicate3jk = item.Descendants("div").Where(w => w.GetAttributeValue("class", "").Equals("category-container"));
                var parentNode = item.SelectNodes("*[contains(@class, 'category-content')]").First();

                if (duplicate3jk!=null&&duplicate3jk.Count()!=0)
                {
                    foreach (var item6 in duplicate3jk)
                    {
                        var yyy = item6.ParentNode.ToString();
                        item.SelectSingleNode(yyy).RemoveChild(item6, false);
                    }
                }
                    
                

                if (duplicate == null)
                {
                    containersWithoutDuplicate.Add(item);
                }
                else
                {
                    containersWithDuplicate.Add(item);
                }
            }

            var containersEdited = new List<HtmlNode>();

            foreach (var item in containersWithDuplicate)
            {
                var divsToBeDeleted = item.SelectNodes("div[contains(@class,'category-content')]/div[contains(@class,'foot-market-border')]/div[contains(@class,'foot-market')]/div[contains(@class,' coupon-row')]/div[contains(@class,'category-container')]");
                if (divsToBeDeleted != null)
                {
                    foreach (var item3 in divsToBeDeleted)
                    {
                        item.SelectSingleNode("div[contains(@class,'category-content')]/div[contains(@class,'foot-market-border')]/div[contains(@class,'foot-market')]/div[contains(@class,' coupon-row')]").RemoveChild(item3, false);
                    }

                }
                containersEdited.Add(item);
            }
            containersWithoutDuplicate.AddRange(containersEdited);

            List<FootballOddDto> listFootballOdds = new List<FootballOddDto>();

            foreach (var section in containersWithoutDuplicate)
            {


                var tableSection = section.Descendants("H2")
                    .Where(node => node.GetAttributeValue("class", "")
                        .Equals("category-label")).FirstOrDefault();
                if (tableSection==null)
                {
                    break;
                }
                var tableSectionTitle = tableSection.InnerText;

                if (tableSectionTitle == "Outright")
                {
                    break;
                }


                var oddsPerSection = section.Descendants("tr").Where(node => node.GetAttributeValue("class", "")
                        .Equals(" sub-row")).ToList();
                //oddsPerSection.First().Remove();
                //oddsPerSection.RemoveAt(0);


                foreach (var odd in oddsPerSection)
                {
                    FootballOddDto footballOdd = new FootballOddDto

                    {
                        Tournament = tableSectionTitle
                    };

                    
                    


                    var dayAndHour2 = odd.Descendants("td")
                       .Where(node => node.GetAttributeValue("class", "")
                       .Equals("date "))
                       ;

                    var dayAndHour = odd.Descendants("td")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("date "))
                        .First().InnerText.Trim('\n', ' ');



                    bool isValidData = DateTime.TryParse(dayAndHour, out DateTime data);

                    if (!isValidData)
                    {
                        String inputString = dayAndHour;

                        inputString = Regex.Replace(inputString, " \\(.*\\)$", "");

                        data = DateTime.ParseExact(inputString, "dd MMM HH:mm",
                            System.Globalization.CultureInfo.InvariantCulture);

                    }


                    footballOdd.BeginingTime = data;

                    var match = odd.Descendants("td")
                        .Where(node =>
                            node.GetAttributeValue("class", "").Equals("today-name")
                            ||
                            node.GetAttributeValue("class", "").Equals("name")
                            ||
                            node.GetAttributeValue("class", "").Equals("date-with-year-name"));





                    var pairOneTitle = match
                        .First()
                        .InnerText.Trim('\n', ' ', '1', '2', '.');


                    var pairTwoTitle = match
                        .Last()
                        .InnerText.Trim('\n', ' ', '1', '2', '.');

                    footballOdd.PairOne = pairOneTitle;
                    footballOdd.PairTwo = pairTwoTitle;


                    var coefficientFirstH = odd.Descendants("td")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("price height-column-with-price    first-in-main-row  coupone-width-3"))
                        .FirstOrDefault();
                    string coefficientFirst = "1";
                    if (coefficientFirstH == null)
                    {
                        footballOdd.CoefficientHost = coefficientFirst;
                    }
                    else
                    {
                        coefficientFirst = coefficientFirstH.InnerText.Trim('\n', ' ');
                    }


                    var coefficientDrawH = odd.Descendants("td")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("price height-column-with-price    coupone-width-3"))
                        .FirstOrDefault();


                    string coefficientDraw = "1";
                    if (coefficientDrawH == null)
                    {
                        footballOdd.CoefficientDraw = coefficientDraw;
                    }
                    else
                    {
                        coefficientDraw = coefficientDrawH.InnerText.Trim('\n', ' ');
                    }


                    var coefficientSecondH = odd.Descendants("td")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("price height-column-with-price    coupone-width-3"))
                        .LastOrDefault();

                    string coefficientSecond = "1";
                    if (coefficientSecondH == null)
                    {
                        footballOdd.CoefficientVisitors = coefficientSecond;
                    }
                    else
                    {
                        coefficientSecond = coefficientSecondH.InnerText.Trim('\n', ' ');
                    }

                    footballOdd.CoefficientHost = coefficientFirst;
                    footballOdd.CoefficientDraw = coefficientDraw;
                    footballOdd.CoefficientVisitors = coefficientSecond;
                    footballOdd.Time = DateTime.Now;

                    footballOdd.SportId = sport;
                    

                    listFootballOdds.Add(footballOdd);
                };
            }
            



            
                
            
            return listFootballOdds;
        }


        public async void Kopacka()
        {
            //var kopacka = "http://www.zlatnakopacka.mk/prematch";
            //var sportlife = "http://sportlife.com.mk/Oblozuvanje";
            //var ttt = "https://www.marathonbet.com/en/popular/Football/?menu=11";

            //var yyy = await GetHtmlDocument(kopacka);
            //var yyyd = await GetHtmlDocument(sportlife);
            //var ggg = await GetHtmlDocument(ttt);
            TestSelenium ts = new TestSelenium();
            ts.testUrl();
        }

        private static async Task<HtmlDocument> GetHtmlDocument(string url)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(html);
            return htmlDoc;
        }

        public async Task<List<TennisOddDto>> GetTennisOddsEdit(string url, string sport)
        {

            var htmlDoc = await GetHtmlDocument(url);


            var containers = htmlDoc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                    .Equals("category-container"))
                .ToList();



            var containersClean = new List<HtmlNode>();
            var containersWithDuplicate = new List<HtmlNode>();

            foreach (var item in containers)
            {
              
                var duplicate = item.SelectNodes("div[contains(@class,'category-content')]/div[contains(@class,'foot-market-border')]/div[contains(@class,'foot-market')]/div[contains(@class,' coupon-row')]/div[contains(@class,'category-container')]");
                if (duplicate == null)
                {

                    containersClean.Add(item);
                }
                else
                {
                    containersWithDuplicate.Add(item);
                }
            }

            var containersEdited = new List<HtmlNode>();

            foreach (var item in containersWithDuplicate)
            {
                var divsToBeDeleted = item.SelectNodes("div[contains(@class,'category-content')]/div[contains(@class,'foot-market-border')]/div[contains(@class,'foot-market')]/div[contains(@class,' coupon-row')]/div[contains(@class,'category-container')]");
                if (divsToBeDeleted != null)
                {
                    foreach (var item3 in divsToBeDeleted)
                    {

                        item.SelectSingleNode("div[contains(@class,'category-content')]/div[contains(@class,'foot-market-border')]/div[contains(@class,'foot-market')]/div[contains(@class,' coupon-row')]").RemoveChild(item3, false);
                    }

                }
                containersEdited.Add(item);
            }
            containersClean.AddRange(containersEdited);



            List<TennisOddDto> listTennisOdds = new List<TennisOddDto>();

            foreach (var section in containersClean)
            {
                //var tableSectionTitle = section.Descendants("H2")
                //    .Where(node => node.GetAttributeValue("class", "")
                //        .Equals("category-label"))
                //    .FirstOrDefault()
                //    .InnerText;


                var tableH = section.Descendants("H2").Where(node => node.GetAttributeValue("class", "")
                        .Equals("category-label"))
                    .FirstOrDefault();
                if (tableH == null)
                {
                    tableH = section.Descendants("H1").Where(node => node.GetAttributeValue("class", "")
                    .Equals("category-label"))
                .FirstOrDefault(); ;
                }
                var tableSectionTitle = tableH.InnerText;




                var resultString = Regex.Replace(tableSectionTitle, @"^((?:\S+\s+){0}\S+).*", "${1}", RegexOptions.Multiline);

                if (resultString == "Outright." || resultString == "Outright")
                {
                    continue;
                }


                var oddsPerSection = section.Descendants("tr").Where(node => node.GetAttributeValue("class", "")
                        .Equals(" sub-row"));
                //var oddsPerSection = section.Descendants("tr")
                //    .Where(node => node.GetAttributeValue("class", "")
                //        .Equals(" event-header"))
                //    .ToList();

                foreach (var odd in oddsPerSection)
                {
                    TennisOddDto tennisOdd = new TennisOddDto
                    {
                        Tournament = tableSectionTitle
                    };

                    var match = odd.Descendants("td")
                        .Where(node =>
                            node.GetAttributeValue("class", "").Equals("today-name")
                            ||
                            node.GetAttributeValue("class", "").Equals("name")                         
                            ||
                            node.GetAttributeValue("class", "").Equals("date-with-year-name")

                            );

                    var dayAndHour = odd.Descendants("td")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("date "))
                        .FirstOrDefault().InnerText.Trim('\n', ' ');


                    bool isValidData = DateTime.TryParse(dayAndHour, out DateTime data);

                    if (!isValidData)
                    {
                        String inputString = dayAndHour;

                        inputString = Regex.Replace(inputString, " \\(.*\\)$", "");

                        data = DateTime.ParseExact(inputString, "dd MMM HH:mm",
                            System.Globalization.CultureInfo.InvariantCulture);

                    }


                    tennisOdd.BeginingTime = data;

                    var pairOneTitle = match
                        .First()
                        .InnerText.Trim('\n', ' ', '1', '2', '.');

                    var pairTwoTitle = match
                        .Last()
                        .InnerText.Trim('\n', ' ', '1', '2', '.');

                    tennisOdd.PairOne = pairOneTitle;
                    tennisOdd.PairTwo = pairTwoTitle;


                    string coefficientFirst = "";
                    var cf = odd.Descendants("td")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("price height-column-with-price    first-in-main-row  coupone-width-2"))
                        .FirstOrDefault();

                    if (cf == null)
                    {
                        coefficientFirst = "1";
                    }
                    else
                    {
                        coefficientFirst = cf.InnerText.Trim('\n', ' ');
                    }


                    string coefficientSecond = "";
                    var cs = odd.Descendants("td")
                       .Where(node => node.GetAttributeValue("class", "")
                        .Equals("price height-column-with-price    coupone-width-2"))
                        .FirstOrDefault();

                    if (cs == null)
                    {
                        coefficientSecond = "1";
                    }
                    else
                    {
                        coefficientSecond = cs.InnerText.Trim('\n', ' ');
                    }



                    //var ee = coefficientFirstt.InnerText;
                    //var coefficientSecond = odd.Descendants("td")
                    //    .Where(node => node.GetAttributeValue("class", "")
                    //    .Equals("price height-column-with-price    "))
                    //    .FirstOrDefault().InnerText.Trim('\n', ' '); ;


                    //}
                    //if (coefficientSecond == "—")
                    //{
                    //    coefficientSecond = "1";
                    //}
                    tennisOdd.SportId = sport;
                    tennisOdd.CoefficientFirst = coefficientFirst;
                    tennisOdd.CoefficientSecond = coefficientSecond;
                    tennisOdd.Time = DateTime.Now;


                    listTennisOdds.Add(tennisOdd);
                };
            }




            return listTennisOdds;
        }

        public async Task<List<TennisOddDto>> GetTennisOddsFinal()
        {
            Dictionary<string, string> gg = new Dictionary<string, string>();
            //gg.Add(4, "https://www.marathonbet.com/en/betting/Basketball/?menu=6");
            gg.Add("Tennis", "https://www.marathonbet.com/en/betting/Tennis/?menu=2398");
            gg.Add("Baseball", "https://www.marathonbet.com/en/betting/Baseball/?menu=5");
            gg.Add("Darts", "https://www.marathonbet.com/en/betting/Darts/?menu=9");
            //gg.Add(7, "https://www.marathonbet.com/en/betting/Badminton/?menu=382581");
            gg.Add("American Football", "https://www.marathonbet.com/en/betting/American+Football/?menu=4");
            gg.Add("Voleyball", "https://www.marathonbet.com/en/betting/Volleyball/?menu=22712");
            //gg.Add(10, "https://www.marathonbet.com/en/betting/MMA/?menu=439050");
            gg.Add("Snooker", "https://www.marathonbet.com/en/betting/Snooker/?menu=2185");
            //gg.Add(12, "https://www.marathonbet.com/en/betting/e-Sports/?menu=1895085");
            //gg.Add(13, "https://www.marathonbet.com/en/betting/Cricket/?menu=8");
            //gg.Add(14, "https://www.marathonbet.com/en/betting/Boxing/?menu=7");


            List<TennisOddDto> listTennis = new List<TennisOddDto>();
            foreach (var item in gg)
            {

                var ttt = await GetTennisOddsEdit(item.Value, item.Key);
                listTennis.AddRange(ttt);

            }
            _oddsService.AddNewOddsTennis(listTennis);
            return listTennis;

        }

        public async Task<List<FootballOddDto>> GetFootballOddsFinal()
        {
            
            var stringsUrl = new List<string[]> {
                //new string[] { "Handball", "https://www.marathonbet.com/en/betting/Handball/?menu=52914" },
                //new string[]{ "IceHockey", "https://www.marathonbet.com/en/betting/Ice+Hockey/?menu=537" },
                new string[]{ "Football", "https://www.marathonbet.com/en/betting/Football/?menu=11" }
                
            };

  
            List<FootballOddDto> listFootball = new List<FootballOddDto>();
            foreach (var item in stringsUrl)
            {
                for (int i = 0; i < 50; i++)
                {
                    string z = i.ToString();
                    string url = item[1] + "&page=" + z;
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(url);

                        if (response.IsSuccessStatusCode)
                        {
                            var ttt = await GetFootballsOdds(url, item[0]);
                            if (ttt.Count()!=0)
                            {
                                listFootball.AddRange(ttt);
                            }
                           

                        }

                        else
                        {
                            break;
                        }
                    }

                }
            }


            //var yyy = new string[] { "IceHockey", "https://www.marathonbet.com/en/betting/Ice+Hockey/?menu=537&page=4" };

            //var tttc = await GetFootballsOdds(yyy[1], yyy[0]);
            //listFootball.AddRange(tttc);
           


            _oddsService.AddNewOddsFootball(listFootball);
            return listFootball;


        }

        //public async Task<List<FootballOddDto>> GetFootballsOddsEdit(string url, string sport)
        //{


        //    var htmlDoc = await GetHtmlDocument(url);

        //    var containers = htmlDoc.DocumentNode.Descendants("div").Where(t => t.GetAttributeValue("class", "").Equals("category-container"));

        //    var containersWithoutDuplicate = new List<HtmlNode>();
        //    var containersWithDuplicate = new List<HtmlNode>();

        //    foreach (var item in containers)
        //    {
        //        var duplicate = item.SelectNodes("div[contains(@class,'category-content')]/div[contains(@class,'foot-market-border')]/div[contains(@class,'category-container')]");
        //        if (duplicate == null)
        //        {
        //            containersWithoutDuplicate.Add(item);
        //        }
        //        else
        //        {
        //            containersWithDuplicate.Add(item);
        //        }
        //    }

        //    var containersEdited = new List<HtmlNode>();

        //    foreach (var item in containersWithDuplicate)
        //    {
        //        var divsToBeDeleted = item.SelectNodes("div[contains(@class,'category-content')]/div[contains(@class,'foot-market-border')]/div[contains(@class,'category-container')]");
        //        if (divsToBeDeleted != null)
        //        {
        //            foreach (var item3 in divsToBeDeleted)
        //            {
        //                item.SelectSingleNode("div[contains(@class,'category-content')]/div[contains(@class,'foot-market-border')]").RemoveChild(item3, false);
        //            }

        //        }
        //        containersEdited.Add(item);
        //    }
        //    containersWithoutDuplicate.AddRange(containersEdited);

        //    List<FootballOddDto> listFootballOdds = new List<FootballOddDto>();
        //    FootballOddDto footballOdd = new FootballOddDto();
        //    footballOdd.PairOne = "";
        //    footballOdd.PairTwo = "";
        //    footballOdd.Time = DateTime.Now;
        //    footballOdd.Tournament = "";
        //    footballOdd.BeginingTime = DateTime.Now;
        //    footballOdd.CoefficientHost = "1";
        //    footballOdd.CoefficientDraw = "1";
        //    footballOdd.CoefficientVisitors = "1";
        //    footballOdd.SportId = "";
            
        //    foreach (var section in containersWithoutDuplicate)
        //    {


        //        var tableSectionTitle = section.Descendants("H2")
        //            .Where(node => node.GetAttributeValue("class", "")
        //                .Equals("category-label"))
        //            .FirstOrDefault()
        //            .InnerText;

        //        if (tableSectionTitle == "Outright")
        //        {
        //            break;
        //        }

        //        var oddsPerSection = section.Descendants("tbody").ToList();

        //        foreach (var odd in oddsPerSection)
        //        {

        //            //FootballOddDto footballOdd = new FootballOddDto
        //            //{
        //            //    footballOddTournament = tableSectionTitle
        //            //};
        //            footballOdd.Tournament = tableSectionTitle;
        //            var match = odd.Descendants("td")
        //                .Where(c =>
        //                c.GetAttributeValue("class", "").Equals("name")
        //                ||
        //                c.GetAttributeValue("class", "").Equals("today-name"));







        //            var dayAndHour = odd.Descendants("td")
        //                .Where(node => node.GetAttributeValue("class", "")
        //                .Equals("date "))
        //                .FirstOrDefault().InnerText.Trim('\n', ' ');


        //            bool isValidData = DateTime.TryParse(dayAndHour, out DateTime data);

        //            if (!isValidData)
        //            {
        //                String inputString = dayAndHour;

        //                inputString = Regex.Replace(inputString, " \\(.*\\)$", "");

        //                data = DateTime.ParseExact(inputString, "dd MMM HH:mm",
        //                    System.Globalization.CultureInfo.InvariantCulture);

        //            }


        //            footballOdd.BeginingTime = data;




        //            var pairOneTitle = match
        //                .FirstOrDefault()
        //                .InnerText.Trim('\n', ' ', '1', '2', '.');


        //            var pairTwoTitle = match
        //                .LastOrDefault()
        //                .InnerText.Trim('\n', ' ', '1', '2', '.');

        //            footballOdd.PairOne = pairOneTitle;
        //            footballOdd.PairTwo = pairTwoTitle;


        //            var coefficientFirstH = odd.Descendants("td")
        //                .Where(node => node.GetAttributeValue("class", "")
        //                .Equals("price height-column-with-price    first-in-main-row  "))
        //                .FirstOrDefault();
        //            string coefficientFirst = "1";
        //            if (coefficientFirstH == null)
        //            {
        //                footballOdd.CoefficientHost = coefficientFirst;
        //            }
        //            else
        //            {
        //                coefficientFirst = coefficientFirstH.InnerText.Trim('\n', ' ');
        //            }


        //            var coefficientDrawH = odd.Descendants("td")
        //                .Where(node => node.GetAttributeValue("class", "")
        //                .Equals("price height-column-with-price    "))
        //                .FirstOrDefault();


        //            string coefficientDraw = "1";
        //            if (coefficientDrawH == null)
        //            {
        //                footballOdd.CoefficientDraw = coefficientDraw;
        //            }
        //            else
        //            {
        //                coefficientDraw = coefficientDrawH.InnerText.Trim('\n', ' ');
        //            }


        //            var coefficientSecondH = odd.Descendants("td")
        //                .Where(node => node.GetAttributeValue("class", "")
        //                .Equals("price height-column-with-price    "))
        //                .LastOrDefault();





        //            string coefficientSecond = "1";
        //            if (coefficientSecondH == null)
        //            {
        //                footballOdd.CoefficientVisitors = coefficientSecond;
        //            }
        //            else
        //            {
        //                coefficientSecond = coefficientSecondH.InnerText.Trim('\n', ' ');
        //            }

        //            footballOdd.CoefficientHost = coefficientFirst;
        //            footballOdd.CoefficientDraw = coefficientDraw;
        //            footballOdd.CoefficientVisitors = coefficientSecond;
        //            footballOdd.Time = DateTime.Now;

        //            footballOdd.SportId = sport;

        //            listFootballOdds.Add(footballOdd);
        //        };
        //    }
        //    return listFootballOdds;
        //}



    }

    public class TestSelenium
    {
        IWebDriver driver2 = new ChromeDriver();

        WebBrowser wb = new WebBrowser();
        
        //_driver = new PhantomJSDriver();
        [Test]
        public async void testUrl()
        {

            //var chromeOptions = new ChromeOptions();
            //chromeOptions.AddArguments("headless");

            //using (var browser = new ChromeDriver(chromeOptions))
            //{
            //    // add your code here
            //}

            //var options = new PhantomJSOptions();
            //options.AddAdditionalCapability("IsJavaScriptEnabled", true);

            //var driver = new RemoteWebDriver(new URI(Configuration.SeleniumServerHub),
            //                    options.ToCapabilities(),
            //                    TimeSpan.FromSeconds(3)
            //                  );
            //driver.Url = "http://www.regulations.gov/#!documentDetail;D=APHIS-2013-0013-0083";
            //driver.Navigate();
            ////the driver can now provide you with what you need (it will execute the script)
            ////get the source of the page
            //var source = driver.PageSource;
            ////fully navigate the dom
            //var pathElement = driver.FindElementById("some-id");



            //var ttt = new PhantomJS();


            //IWebDriver driver2 = new ChromeDriver();
            //driver2.Navigate().GoToUrl("http://sportlife.com.mk/Oblozuvanje");
            //String text = driver2.PageSource;
            //List<IWebElement> element = driver2.FindElements(By.ClassName("checkmark")).ToList();
            //foreach (var item in element)
            //{
            //    item.Click();
            //}
            //Console.WriteLine(text);
            //System.Threading.Thread.Sleep(4000);


            //var url = "http://sportlife.com.mk/Oblozuvanje";
            //var htmlDoc = await GetHtmlDocument(url);

            //var pageSource = driver2.FindElements(By.ClassName("panel custom-panel"));
            //foreach (var item in pageSource)
            //{
            //    var yy = item.FindElements(By.ClassName(""));
            //}
            //Thread.Sleep(6000);
            //pageSource.Contains()
            Uri uri = new Uri("http://www.somewebsite.com/somepage.htm");



            //webBrowserControl.AllowNavigation = true;
            //// optional but I use this because it stops javascript errors breaking your scraper
            //webBrowserControl.ScriptErrorsSuppressed = true;
            //// you want to start scraping after the document is finished loading so do it in the function you pass to this handler
            //webBrowserControl.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowserControl_DocumentCompleted);
            //webBrowserControl.Navigate(uri);


            //var containers = htmlDoc.DocumentNode.Descendants("div")
            //    .Where(node => node.GetAttributeValue("id", "")
            //        .Equals("tmpSportoviLigi"))
            //    .ToList();


            //driver.Close();
            //driver.Quit();
            driver2.Close();
            driver2.Quit();

        }

        private static async Task<HtmlDocument> GetHtmlDocument(string url)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(html);
            return htmlDoc;
        }

       

        //private void webBrowserControl_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    var url = "http://sportlife.com.mk/Oblozuvanje";
        //    var htmlDoc = await GetHtmlDocument(url);

        //    var containers = htmlDoc.DocumentNode.Descendants("div")
        //        .Where(node => node.GetAttributeValue("id", "")
        //            .Equals("tmpSportoviLigi"))
        //        .ToList();

        //    foreach (HtmlDoc div in divs)
        //    {
        //        //do something
        //    }
        //}        //private void webBrowserControl_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    var url = "http://sportlife.com.mk/Oblozuvanje";
        //    var htmlDoc = await GetHtmlDocument(url);

        //    var containers = htmlDoc.DocumentNode.Descendants("div")
        //        .Where(node => node.GetAttributeValue("id", "")
        //            .Equals("tmpSportoviLigi"))
        //        .ToList();

        //    foreach (HtmlDoc div in divs)
        //    {
        //        //do something
        //    }
        //}
    }

    //public class CustomBrowser
    //{
    //    public CustomBrowser()
    //    {
    //        //
    //        // TODO: Add constructor logic here
    //        //
    //    }

    //    protected string _url;
    //    string html = "";
    //    public string GetWebpage(string url)
    //    {
    //        _url = url;
    //        // WebBrowser is an ActiveX control that must be run in a
    //        // single-threaded apartment so create a thread to create the
    //        // control and generate the thumbnail
    //        Thread thread = new Thread(new ThreadStart(GetWebPageWorker));
    //        thread.SetApartmentState(ApartmentState.STA);
    //        thread.Start();
    //        thread.Join();
    //        string s = html;
    //        return s;
    //    }

        //protected void GetWebPageWorker()
        //{
        //    WebBrowser browser = new WebBrowser();
        //    try
        //    {
        //        browser.ScrollBarsEnabled = false;
        //        browser.ScriptErrorsSuppressed = true;
        //        browser.Navigate(_url);

        //        // Wait for control to load page
        //        while (browser.ReadyState != WebBrowserReadyState.Complete)
        //            Application.DoEvents();

        //        html = browser.DocumentText;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
          
        //        //  browser.ClientSize = new Size(_width, _height);
                
                
            
        //}
        //}
       
    
}




