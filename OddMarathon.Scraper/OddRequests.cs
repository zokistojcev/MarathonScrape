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
                    
                    if (cf==null)
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

        public async Task<List<FootballOddDto>> GetFootballsOdds(string url, byte sport)
        {
            

            var htmlDoc = await GetHtmlDocument(url);

            var containers = htmlDoc.DocumentNode.Descendants("div").Where(t => t.GetAttributeValue("class", "").Equals("category-container"));

            var containersWithoutDuplicate = new List<HtmlNode>();
            var containersWithDuplicate = new List<HtmlNode>();

            foreach (var item in containers)
            {
                var duplicate = item.SelectNodes("div[contains(@class,'category-content')]/div[contains(@class,'foot-market-border')]/div[contains(@class,'category-container')]");
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
            containersWithoutDuplicate.AddRange(containersEdited);
            
            List<FootballOddDto> listFootballOdds = new List<FootballOddDto>();

            foreach (var section in containersWithoutDuplicate)
            {


                var tableSectionTitle = section.Descendants("H2")
                    .Where(node => node.GetAttributeValue("class", "")
                        .Equals("category-label"))
                    .FirstOrDefault()
                    .InnerText;

                if (tableSectionTitle == "Outright")
                {
                    break;
                }

                var oddsPerSection = section.Descendants("tbody").ToList();

                foreach (var odd in oddsPerSection)
                {
                    
                    FootballOddDto footballOdd = new FootballOddDto
                    {
                        Tournament = tableSectionTitle
                    };

                    var match = odd.Descendants("td")
                        .Where(c=>
                        c.GetAttributeValue("class", "").Equals("name")
                        ||
                        c.GetAttributeValue("class", "").Equals("today-name"));


                   




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


                    footballOdd.BeginingTime = data;




                    var pairOneTitle = match
                        .FirstOrDefault()
                        .InnerText.Trim('\n', ' ', '1', '2', '.');


                    var pairTwoTitle = match
                        .LastOrDefault()
                        .InnerText.Trim('\n', ' ', '1', '2', '.');

                    footballOdd.PairOne = pairOneTitle;
                    footballOdd.PairTwo = pairTwoTitle;


                    var coefficientFirstH = odd.Descendants("td")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("price height-column-with-price    first-in-main-row  "))
                        .FirstOrDefault();
                    string coefficientFirst = "1";
                    if (coefficientFirstH==null)
                    {
                        footballOdd.CoefficientHost = coefficientFirst;
                    }
                    else
                    {
                         coefficientFirst = coefficientFirstH.InnerText.Trim('\n', ' ');
                    }


                    var coefficientDrawH = odd.Descendants("td")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("price height-column-with-price    "))
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
                        .Equals("price height-column-with-price    "))
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


        //public async void Kopacka()
        //{
        //    var kopacka = "http://www.zlatnakopacka.mk/prematch";
        //    var sportlife = "http://sportlife.com.mk/Oblozuvanje";
        //    var ttt = "https://www.marathonbet.com/en/popular/Football/?menu=11";
            
        //    var yyy = await GetHtmlDocument(kopacka);
        //    var yyyd = await GetHtmlDocument(sportlife);
        //    var ggg = await GetHtmlDocument(ttt);
        //    var tt = "";
        //}

        private static async Task<HtmlDocument> GetHtmlDocument(string url)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(html);
            return htmlDoc;
        }
      
        public async Task<List<TennisOddDto>> GetTennisOddsEdit(string url, byte sport)
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
            Dictionary<byte, string> gg = new Dictionary<byte, string>();
            gg.Add(4, "https://www.marathonbet.com/en/betting/Basketball/?menu=6");
            gg.Add(3, "https://www.marathonbet.com/en/betting/Tennis/?menu=2398");
            gg.Add(5, "https://www.marathonbet.com/en/betting/Baseball/?menu=5");
            gg.Add(6, "https://www.marathonbet.com/en/betting/Darts/?menu=9");
            //gg.Add(7, "https://www.marathonbet.com/en/betting/Badminton/?menu=382581");
            //gg.Add(8, "https://www.marathonbet.com/en/betting/American+Football/?menu=4");
            //gg.Add(9, "https://www.marathonbet.com/en/betting/Volleyball/?menu=22712");
            //gg.Add(10, "https://www.marathonbet.com/en/betting/MMA/?menu=439050");
            //gg.Add(11, "https://www.marathonbet.com/en/betting/Snooker/?menu=2185");
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
            Dictionary<byte, string> gg = new Dictionary<byte, string>();
            gg.Add(2, "https://www.marathonbet.com/en/popular/Football/?menu=11");
            gg.Add(1, "https://www.marathonbet.com/en/betting/Handball/?menu=52914");
            gg.Add(15, "https://www.marathonbet.com/en/betting/Ice+Hockey/?menu=537");
            
         

            List<FootballOddDto> listFootball = new List<FootballOddDto>();
            foreach (var item in gg)
            {

                var ttt = await GetFootballsOdds(item.Value, item.Key);
                listFootball.AddRange(ttt);

            }
            _oddsService.AddNewOddsFootball(listFootball);
            return listFootball;

 
        }

    }

}
