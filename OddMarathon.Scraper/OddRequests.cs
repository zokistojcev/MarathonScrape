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

                    var coefficientFirst = odd.Descendants("td")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("price height-column-with-price    first-in-main-row  "))
                        .FirstOrDefault().InnerText.Trim('\n', ' ');


                    var coefficientSecond = odd.Descendants("td")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("price height-column-with-price    "))
                        .FirstOrDefault().InnerText.Trim('\n', ' '); ;
                   

                    tennisOdd.CoefficientFirst = coefficientFirst;
                    tennisOdd.CoefficientSecond = coefficientSecond;
                    tennisOdd.Time = DateTime.Now;

                    listTennisOdds.Add(tennisOdd);
                };
            }



            _oddsService.AddNewOddsTennis(listTennisOdds);
            return listTennisOdds;
        }

        public async Task<List<FootballOddDto>> GetFootballsOdds(string url)
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


                    string dateTimeString = odd.Descendants("td")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("date "))
                        .FirstOrDefault().InnerText.Trim('\n', ' ');


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

                    listFootballOdds.Add(footballOdd);
                };
            }
            return listFootballOdds;
        }




        private static async Task<HtmlDocument> GetHtmlDocument(string url)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(html);
            return htmlDoc;
        }
    }
}
