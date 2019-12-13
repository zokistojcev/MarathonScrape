using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using NuGet.Common;
using OddMarathon.Services.BusinessLogic.Odds;
using OddMarathon.Services.Dtos;
using ScrapySharp.Extensions;


namespace OddMarathon.Scraper
{
    public class OddRequests
    {
        private static string[][] MARATHON_TWO_ODDS_URLS;

        private readonly IOddsService _oddsService;

        public OddRequests(IOddsService oddsService)
        {
            _oddsService = oddsService;

        }

        public async Task<IEnumerable<FootballOddDto>> GetFootballsOdds(IEnumerable<HtmlDocument> htmlFootball)
        {
            List<FootballOddDto> listFootballOdds = new List<FootballOddDto>();
            //var htmlDoc = await GetHtmlDocument(url);
            foreach (var htmlDoc in htmlFootball)
            {
             
                var containers = htmlDoc.DocumentNode.Descendants("div").Where(t => t.GetAttributeValue("class", "").Equals("category-container"));

                var sport2 = htmlDoc.DocumentNode.Descendants("a").Where(t => t.GetAttributeValue("class", "").Equals("sport-category-label")).FirstOrDefault().InnerText;

                foreach (var section in containers)
                {



                    var tableH = section.Descendants("H2").Where(node => node.GetAttributeValue("class", "")
                           .Equals("category-label "))
                       .FirstOrDefault();

                   
                    if (tableH == null)
                    {
                        tableH = section.Descendants("H1").Where(node => node.GetAttributeValue("class", "")
                        .Equals("category-label "))
                    .FirstOrDefault(); ;
                    }
                    var tableSectionTitle = tableH.InnerText;

                  
                    var resultString = Regex.Replace(tableSectionTitle, @"^((?:\S+\s+){0}\S+).*", "${1}", RegexOptions.Multiline);

                    if (resultString.Contains("Outright."))
                    {
                        continue;
                    }

                


                    var oddsPerSection = section.Descendants("tr").Where(node => node.GetAttributeValue("class", "")
                            .Equals("sub-row")).ToList();
                    //oddsPerSection.First().Remove();
                    //oddsPerSection.RemoveAt(0);


                    foreach (var odd in oddsPerSection)
                    {
                        FootballOddDto footballOdd = new FootballOddDto

                        {
                            Tournament = tableSectionTitle
                        };

                        var giii = odd.Descendants("div")
                          .Where(node =>
                              node.GetAttributeValue("class", "").Equals("hint")

                              );

                        if (giii.Count() > 0)
                        {
                            Console.WriteLine("zokiiiiiiiiiiiii");

                            var dupicateSection = oddsPerSection.FirstOrDefault();


                            var dayAndHour = dupicateSection.Descendants("td")
                                .Where(node => node.GetAttributeValue("class", "")
                                .Equals("date"));
                            string dayAndHour2;
                            if (dayAndHour == null)
                            {
                                dayAndHour2 = DateTime.Now.ToString("yyyyMMddHHmmss");



                            }
                            else
                            {

                                dayAndHour2 = dayAndHour.First().InnerText.Trim('\n', ' ');
                                GetDateTime(dayAndHour2);
                            }

                            footballOdd.BeginingTime = GetDateTime(dayAndHour2);

                            var match = dupicateSection.Descendants("td")
                             .Where(node =>
                                 node.GetAttributeValue("class", "").Equals("today-name")
                                 ||
                                 node.GetAttributeValue("class", "").Equals("name")
                                 ||
                                 node.GetAttributeValue("class", "").Equals("date-with-year-name"));

                       
                            var pairOneTitle = odd.Descendants("span").FirstOrDefault().InnerText;
                            var pairTwoTitle = odd.Descendants("span").Skip(1).FirstOrDefault().InnerText;

                           
                            var plusButton = odd.Descendants("span").Skip(2).FirstOrDefault();



                            var coefficientFirstH = odd.Descendants("span").Skip(3).FirstOrDefault();
                            HtmlNode coefficientDrawH;
                            HtmlNode coefficientSecondH;
                            var dfg = odd.Descendants("span").Skip(4);
                            if (dfg.Count() < 1)
                            {
                                Console.WriteLine("errror");

                                coefficientDrawH = coefficientFirstH;
                                coefficientFirstH = plusButton;
                                coefficientSecondH = odd.Descendants("span").Skip(4).FirstOrDefault();
                            }
                            else
                            {

                                coefficientDrawH = odd.Descendants("span").Skip(4).FirstOrDefault();
                                coefficientSecondH = odd.Descendants("span").Skip(5).FirstOrDefault();
                            }





                            footballOdd.PairOne = pairOneTitle;
                            footballOdd.PairTwo = pairTwoTitle;


                            if (coefficientFirstH == null)
                            {
                                footballOdd.CoefficientHost = "1";
                            }
                            else
                            {
                                if (coefficientFirstH.InnerText.Contains("("))
                                {
                                    footballOdd.CoefficientHost = "1";
                                }
                                footballOdd.CoefficientHost = coefficientFirstH.InnerText;

                            }


                            if (coefficientDrawH == null)
                            {
                                footballOdd.CoefficientDraw = "1";
                            }
                            else
                            {
                                if (coefficientDrawH.InnerText.Contains("("))
                                {
                                    footballOdd.CoefficientDraw = "1";
                                }
                                footballOdd.CoefficientDraw = coefficientDrawH.InnerText;
                            }


                            if (coefficientSecondH == null)
                            {
                                footballOdd.CoefficientVisitors = "1";
                            }
                            else
                            {
                                if (coefficientSecondH.InnerText.Contains("("))
                                {
                                    footballOdd.CoefficientVisitors = "1";
                                }

                                footballOdd.CoefficientVisitors = coefficientSecondH.InnerText;
                            }

                            footballOdd.Time = DateTime.Now;
                            footballOdd.SportId = sport2;
                            listFootballOdds.Add(footballOdd);
                            break;


                        }






                        else
                        {
                            var dayAndHour = odd.Descendants("td")
                                .Where(node => node.GetAttributeValue("class", "")
                                .Equals("date"));
                            string dayAndHour2;
                            if (dayAndHour==null)
                            {
                                dayAndHour2 = DateTime.Now.ToString("yyyyMMddHHmmss");
                                


                            }
                            else
                            {

                                dayAndHour2 = dayAndHour.First().InnerText.Trim('\n', ' ');
                                
                            }
                            GetDateTime(dayAndHour2);

                            footballOdd.BeginingTime = GetDateTime(dayAndHour2);

                            var match = odd.Descendants("td")
                             .Where(node =>
                                 node.GetAttributeValue("class", "").Equals("today-name")
                                 ||
                                 node.GetAttributeValue("class", "").Equals("name")
                                 ||
                                 node.GetAttributeValue("class", "").Equals("date-with-year-name"));


                            var pairOneTitle = odd.Descendants("span").FirstOrDefault().InnerText;
                            var pairTwoTitle = odd.Descendants("span").Skip(1).FirstOrDefault().InnerText;

                            var coefficientFirstH = odd.Descendants("span").Skip(3).FirstOrDefault();
                            var coefficientDrawH = odd.Descendants("span").Skip(4).FirstOrDefault();
                            var coefficientSecondH = odd.Descendants("span").Skip(5).FirstOrDefault();


                            var plusButton = odd.Descendants("span").Skip(2).FirstOrDefault();

                            if (!plusButton.InnerText.Contains("+"))
                            {
                                coefficientFirstH = odd.Descendants("span").Skip(2).FirstOrDefault();
                                coefficientDrawH = odd.Descendants("span").Skip(3).FirstOrDefault();
                                coefficientSecondH = odd.Descendants("span").Skip(4).FirstOrDefault();
                            }




                            
                            footballOdd.PairOne = pairOneTitle;
                            footballOdd.PairTwo = pairTwoTitle;



                            if (coefficientFirstH == null)
                            {
                                footballOdd.CoefficientHost = "1";
                            }
                            else
                            {
                                if (coefficientFirstH.InnerText.Contains("("))
                                {
                                    footballOdd.CoefficientHost = "1";
                                }
                                footballOdd.CoefficientHost = coefficientFirstH.InnerText;
                               
                            }


                            if (coefficientDrawH == null)
                            {
                                footballOdd.CoefficientDraw = "1";
                            }
                            else
                            {
                                if (coefficientDrawH.InnerText.Contains("("))
                                {
                                    footballOdd.CoefficientDraw = "1";
                                }
                                footballOdd.CoefficientDraw = coefficientDrawH.InnerText;
                            }


                            if (coefficientSecondH == null)
                            {
                                footballOdd.CoefficientVisitors = "1";
                            }
                            else
                            {
                                if (coefficientSecondH.InnerText.Contains("("))
                                {
                                    footballOdd.CoefficientVisitors = "1";
                                }
                                
                                footballOdd.CoefficientVisitors = coefficientSecondH.InnerText;
                            }
                            footballOdd.Time = DateTime.Now;

                            footballOdd.SportId = sport2;
                        }






                        listFootballOdds.Add(footballOdd);
                    };
                }



            }
            var cleanedList = listFootballOdds.DistinctBy(x => new { x.PairOne, x.PairTwo, x.SportId });
            


            return cleanedList;
        }

        private static DateTime GetDateTime(string dayAndHour)
        {
            if (string.IsNullOrWhiteSpace(dayAndHour))
            {
                //throw new ArgumentException("dayAndHour Is Null Or White Space", nameof(dayAndHour));
                return DateTime.Now;
            }

            if (false)
            {
                throw new ArgumentException(nameof(dayAndHour));
            }

            var inputString = Regex.Replace(dayAndHour, " \\(.*\\)$", "");

            //if (inputString.Any(c => char.IsLetter(c)))
            //{
            //    return DateTime.ParseExact(inputString, "dd MMM HH:mm",
            //        System.Globalization.CultureInfo.InvariantCulture);
            //}
            CultureInfo culture = new CultureInfo("en-US");
            DateTime tempDate = Convert.ToDateTime(dayAndHour, culture);

            

            return tempDate;
        }

        private static async Task<HtmlDocument> GetHtmlDocument3(string url)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(html);
            return htmlDoc;
        }

        private static async Task<IEnumerable<HtmlDocument>> GetHtmlDocument(List<string> urls)
        {
            
            
            var x = new List<HtmlDocument>();
            //var htmlDoc = new HtmlDocument();



            using (var httpClient = new HttpClient())
            {
                foreach (var baseUrl in urls)
                {
                    for (int i = 0; i < 50; i++)
                    {

                        string url = baseUrl + "?page=" + i;


                        using (var response = await httpClient.GetAsync(url))
                        {
                            var zz = response.RequestMessage.RequestUri.ToString();
                            string constent = await response.Content.ReadAsStringAsync();
                            if (!response.IsSuccessStatusCode || zz == "https://www.marathonbet.com/en/")
                            {
                                break;
                            }
                            var document = new HtmlDocument();

                           document.LoadHtml(await response.Content.ReadAsStringAsync());
                            x.Add(document);
                        }


                    }
                }
            }
            return x;
            
          
        }

        public async Task<IEnumerable<TennisOddDto>> GetTennisOddsEdit(IEnumerable<HtmlDocument> htmlTennis)
        {
            List<TennisOddDto> listTennisOdds = new List<TennisOddDto>();
            //var htmlDoc = await GetHtmlDocument(url);
            foreach (var htmlDoc in htmlTennis)
            {
                var containers = htmlDoc.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                    .Equals("category-container"))
                .ToList();

                var sport = htmlDoc.DocumentNode.Descendants("a").Where(t => t.GetAttributeValue("class", "").Equals("sport-category-label")).FirstOrDefault().InnerText;

                


                foreach (var section in containers)
                {

                    var tableH = section.Descendants("H2").Where(node => node.GetAttributeValue("class", "")
                            .Equals("category-label "))
                        .FirstOrDefault();

                    if (tableH == null)
                    {
                        tableH = section.Descendants("H1").Where(node => node.GetAttributeValue("class", "")
                        .Equals("category-label "))
                    .FirstOrDefault(); ;
                    }
                    var tableSectionTitle = tableH.InnerText;


                    var resultString = Regex.Replace(tableSectionTitle, @"^((?:\S+\s+){0}\S+).*", "${1}", RegexOptions.Multiline);

                    if (resultString.Contains("Outright."))
                    {
                        continue;
                    }



                    var oddsPerSection = section.Descendants("tr").Where(node => node.GetAttributeValue("class", "")
                            .Equals("sub-row"));
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


                        var giii = odd.Descendants("div")
                           .Where(node =>
                               node.GetAttributeValue("class", "").Equals("hint")

                               );

                        if (giii.Count() > 0)
                        {
                            Console.WriteLine("zokiiiiiiiiiiiii");

                            var dupicateSection = oddsPerSection.FirstOrDefault();
                            var match = dupicateSection.Descendants("td")
                                .Where(node =>
                                    node.GetAttributeValue("class", "").Equals("today-name")
                                    ||
                                    node.GetAttributeValue("class", "").Equals("name")
                                    ||
                                    node.GetAttributeValue("class", "").Equals("date-with-year-name")

                                    );

                            var dayAndHour = dupicateSection.Descendants("td")
                                .Where(node => node.GetAttributeValue("class", "")
                                .Equals("date"));
                                
                            string dayAndHour2;
                            if (dayAndHour!=null)
                            {
                                dayAndHour2 = dayAndHour.FirstOrDefault().InnerText.Trim('\n', ' ');
                            }
                            else
                            {
                                dayAndHour2 = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                            }





                            tennisOdd.BeginingTime = GetDateTime(dayAndHour2);

                            var pairOneTitle = odd.Descendants("span").FirstOrDefault().InnerText;
                            var pairTwoTitle = odd.Descendants("span").Skip(1).FirstOrDefault().InnerText;

                            var coefficientFirstH = odd.Descendants("span").Skip(3).FirstOrDefault();
                            var coefficientSecondH = odd.Descendants("span").Skip(4).FirstOrDefault();

                            var plusButton = odd.Descendants("span").Skip(2).FirstOrDefault().InnerText;

                            if (!plusButton.Contains("+"))
                            {
                                coefficientFirstH = odd.Descendants("span").Skip(2).FirstOrDefault();
                                coefficientSecondH = odd.Descendants("span").Skip(3).FirstOrDefault();
                            }




                            tennisOdd.PairOne = pairOneTitle;
                            tennisOdd.PairTwo = pairTwoTitle;

                            if (coefficientFirstH == null)
                            {
                                tennisOdd.CoefficientFirst = "1";
                            }

                            else
                            {
                                if (coefficientFirstH.InnerText.Contains("("))
                                {
                                    tennisOdd.CoefficientFirst = "1";
                                }
                                tennisOdd.CoefficientFirst = coefficientFirstH.InnerText;
                            }

                            if (coefficientSecondH == null)
                            {
                                tennisOdd.CoefficientSecond = "1";
                            }
                            else
                            {

                                if (coefficientSecondH.InnerText.Contains("("))
                                {
                                    tennisOdd.CoefficientSecond = "1";
                                }
                                tennisOdd.CoefficientSecond = coefficientSecondH.InnerText;
                            }

                            tennisOdd.SportId = sport;

                            tennisOdd.Time = DateTime.Now;


                            listTennisOdds.Add(tennisOdd);
                            break;
                        }


                        else
                        {





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
                                .Equals("date"));

                            string dayAndHour2;
                            if (dayAndHour != null)
                            {
                                dayAndHour2 = dayAndHour.FirstOrDefault().InnerText.Trim('\n', ' ');
                            }
                            else
                            {
                                dayAndHour2 = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                            }





                            tennisOdd.BeginingTime = GetDateTime(dayAndHour2);

                            var pairOneTitle = odd.Descendants("span").FirstOrDefault().InnerText;
                            var pairTwoTitle = odd.Descendants("span").Skip(1).FirstOrDefault().InnerText;

                            var plusButton = odd.Descendants("span").Skip(2).FirstOrDefault().InnerText;



                            var coefficientFirstH = odd.Descendants("span").Skip(3).FirstOrDefault().InnerText;
                            var coefficientSecondH = "1";
                            var dfg = odd.Descendants("span").Skip(4);
                            if (dfg.Count() < 1)
                            {
                                Console.WriteLine("errror");

                                coefficientSecondH = coefficientFirstH;
                                coefficientFirstH = plusButton;
                            }
                            else
                            {
                                coefficientSecondH = odd.Descendants("span").Skip(4).FirstOrDefault().InnerText;
                            }


                            tennisOdd.PairOne = pairOneTitle;
                            tennisOdd.PairTwo = pairTwoTitle;

                            if (coefficientFirstH == null)
                            {
                                tennisOdd.CoefficientFirst = "1";
                            }

                            else
                            {
                                if (coefficientFirstH.Contains("("))
                                {
                                    tennisOdd.CoefficientFirst = "1";
                                }
                                tennisOdd.CoefficientFirst = coefficientFirstH;
                            }

                            if (coefficientSecondH == null)
                            {
                                tennisOdd.CoefficientSecond = "1";
                            }
                            else
                            {

                                if (coefficientSecondH.Contains("("))
                                {
                                    tennisOdd.CoefficientSecond = "1";
                                }
                                tennisOdd.CoefficientSecond = coefficientSecondH;
                            }

                            tennisOdd.SportId = sport;
                            tennisOdd.Time = DateTime.Now;

                            listTennisOdds.Add(tennisOdd);
                        }
                    };
                }


               
            }
            var cleanedList = listTennisOdds.DistinctBy(x => new { x.PairOne, x.PairTwo, x.SportId });

            return listTennisOdds;

        }

        public async Task<IEnumerable<TennisOddDto>> GetTennisOddsFinal()
        {        
            var urls = new List<string> {
               
                "https://www.marathonbet.com/en/popular/Tennis" ,
                "https://www.marathonbet.com/en/popular/Volleyball" ,
                "https://www.marathonbet.com/en/popular/Basketball" ,
                "https://www.marathonbet.com/en/betting/Table+Tennis" ,
                "https://www.marathonbet.com/en/popular/Baseball" ,
                "https://www.marathonbet.com/en/betting/e-Sports" ,
                "https://www.marathonbet.com/en/betting/Snooker",
                "https://www.marathonbet.com/en/betting/Darts",
                "https://www.marathonbet.com/en/betting/Cricket",
                "https://www.marathonbet.com/en/betting/Boxing",
                // "https://www.marathonbet.com/en/betting/MMA"
            };

            

            //List<TennisOddDto> listTennis = new List<TennisOddDto>();


            var tennisPairs = await GetHtmlDocument(urls);
                //listTennis.AddRange(tennisPairs);
                var listTennis = await GetTennisOddsEdit(tennisPairs);
                var filteredPairsTennis = listTennis.DistinctBy(x => new { x.PairOne, x.PairTwo }).ToList();
                _oddsService.AddNewOddsTennis(filteredPairsTennis);

            

            return filteredPairsTennis;

        }

        public async Task<IEnumerable<FootballOddDto>> GetFootballOddsFinal()
        {
            var urls = new List<string> {
                "https://www.marathonbet.com/en/popular/Ice+Hockey" ,
                "https://www.marathonbet.com/en/popular/Football" ,
                "https://www.marathonbet.com/en/popular/Handball" ,
                "https://www.marathonbet.com/en/popular/Water+Polo" ,

            };

            


            List<HtmlDocument> lh = new List<HtmlDocument>();
            
            var listHtml1 = await GetHtmlDocument(urls);
           

            List<FootballOddDto> listFootball = new List<FootballOddDto>();

            
                var ttt = await GetFootballsOdds(listHtml1);
            

            if (ttt.Count() != 0)
            {
                var yuuyif = ttt.DistinctBy(x => new { x.PairOne, x.PairTwo });

                listFootball.AddRange(yuuyif);
            }


            var filteredPairs = listFootball.DistinctBy(x => new { x.PairOne, x.PairTwo });
            _oddsService.AddNewOddsFootball(filteredPairs);

            //return listFootball;
            return listFootball.DistinctBy(x => new { x.PairOne, x.PairTwo });





        }

        //public async Task<IEnumerable<TennisOddDto>> GetTennisOddsFinal2()
        //{



        //    List<TennisOddDto> listTennis = new List<TennisOddDto>();

        //    foreach (var item in stringsUrl)
        //    {
        //        for (int i = 0; i < 15; i++)
        //        {
        //            string z = i.ToString();
        //            string url = item[1] + "?page=" + z;
        //            using (HttpClient client = new HttpClient())
        //            {
        //                HttpResponseMessage response = await client.GetAsync(url);

        //                if (!response.IsSuccessStatusCode)
        //                {
        //                    break;
        //                }

        //                var ttt = await GetTennisOddsEdit(url, item[0]);

        //                if (ttt.Count() != 0)
        //                {
        //                    var yuuyif = ttt.DistinctBy(x => new { x.PairOne, x.PairTwo });

        //                    listTennis.AddRange(yuuyif);
        //                }
        //            }
        //        }
        //        var filteredPairs = listTennis.DistinctBy(x => new { x.PairOne, x.PairTwo });

        //        _oddsService.AddNewOddsTennis(filteredPairs, item[0]);
        //    }

        //    return listTennis.DistinctBy(x => new { x.PairOne, x.PairTwo });
        //}

        //public async Task<List<TennisOddDto>> GetTennisOddsEdit3(string url, string sport)
        //{
        //    //var htmlDoc = await GetHtmlDocument(url);

        //    var containers = htmlDoc.DocumentNode.Descendants("div")
        //        .Where(node => node.GetAttributeValue("class", "")
        //            .Equals("category-container"))
        //        .ToList();

        //    List<TennisOddDto> listTennisOdds = new List<TennisOddDto>();

        //    foreach (var section in containers)
        //    {
        //        var tableH = section.Descendants("H2").Where(node => node.GetAttributeValue("class", "")
        //              .Equals("category-label "))
        //          .FirstOrDefault();
        //        if (tableH == null)
        //        {
        //            tableH = section.Descendants("H1").Where(node => node.GetAttributeValue("class", "")
        //            .Equals("category-label "))
        //        .FirstOrDefault(); ;
        //        }
        //        var tableSectionTitle = tableH.InnerText;

        //        var resultString = Regex.Replace(tableSectionTitle, @"^((?:\S+\s+){0}\S+).*", "${1}", RegexOptions.Multiline);

        //        if (resultString == "Outright." || resultString == "Outright")
        //        {
        //            continue;
        //        }


        //        var oddsPerSection = section.Descendants("tr").Where(node => node.GetAttributeValue("class", "")
        //                .Equals(" sub-row"));

        //        foreach (var odd in oddsPerSection)
        //        {
        //            TennisOddDto tennisOdd = new TennisOddDto
        //            {
        //                Tournament = tableSectionTitle
        //            };
        //            var giii = odd.Descendants("div")
        //                .Where(node =>
        //                    node.GetAttributeValue("class", "").Equals("hint")

        //                    );

        //            if (giii.Count() > 0)
        //            {
        //                var match = odd.Descendants("td")
        //                .Where(node =>
        //                    node.GetAttributeValue("class", "").Equals("today-name")
        //                    ||
        //                    node.GetAttributeValue("class", "").Equals("name")
        //                    ||
        //                    node.GetAttributeValue("class", "").Equals("date-with-year-name")

        //                    ).FirstOrDefault();

        //                var dayAndHour = odd.Descendants("td")
        //                    .Where(node => node.GetAttributeValue("class", "")
        //                    .Equals("date "))
        //                    .FirstOrDefault().InnerText.Trim('\n', ' ');

        //                bool isValidData = DateTime.TryParse(dayAndHour, out DateTime data);

        //                if (!isValidData)
        //                {
        //                    String inputString = dayAndHour;

        //                    inputString = Regex.Replace(inputString, " \\(.*\\)$", "");

        //                    data = DateTime.ParseExact(inputString, "dd MMM HH:mm",
        //                        System.Globalization.CultureInfo.InvariantCulture);

        //                }

        //                tennisOdd.BeginingTime = data;

        //                string coefficientFirst = "";
        //                var cf = odd.Descendants("td")
        //                    .Where(node => node.GetAttributeValue("class", "")
        //                    .Equals("price height-column-with-price    first-in-main-row  coupone-width-2"))
        //                    .FirstOrDefault();

        //                if (cf == null)
        //                {
        //                    break;
        //                }
        //                else
        //                {
        //                    if (cf.InnerText.Trim('\n', ' ').Contains("("))
        //                    {
        //                        break;
        //                    }
        //                    coefficientFirst = cf.InnerText.Trim('\n', ' ');
        //                }


        //                string coefficientSecond = "";
        //                var cs = odd.Descendants("td")
        //                   .Where(node => node.GetAttributeValue("class", "")
        //                    .Equals("price height-column-with-price    coupone-width-2"))
        //                    .FirstOrDefault();

        //                if (cs == null)
        //                {
        //                    break;
        //                }
        //                else
        //                {
        //                    if (cs.InnerText.Trim('\n', ' ').Contains("("))
        //                    {
        //                        break;
        //                    }
        //                    coefficientSecond = cs.InnerText.Trim('\n', ' ');

        //                }

        //                tennisOdd.SportId = sport;
        //                tennisOdd.CoefficientFirst = coefficientFirst;
        //                tennisOdd.CoefficientSecond = coefficientSecond;
        //                tennisOdd.Time = DateTime.Now;

        //                listTennisOdds.Add(tennisOdd);
        //            }
        //            else
        //            {
        //                var match = odd.Descendants("td")
        //                .Where(node =>
        //                    node.GetAttributeValue("class", "").Equals("today-name")
        //                    ||
        //                    node.GetAttributeValue("class", "").Equals("name")
        //                    ||
        //                    node.GetAttributeValue("class", "").Equals("date-with-year-name")

        //                    );

        //                var dayAndHour = odd.Descendants("td")
        //                    .Where(node => node.GetAttributeValue("class", "")
        //                    .Equals("date "))
        //                    .FirstOrDefault().InnerText.Trim('\n', ' ');


        //                bool isValidData = DateTime.TryParse(dayAndHour, out DateTime data);

        //                if (!isValidData)
        //                {
        //                    String inputString = dayAndHour;

        //                    inputString = Regex.Replace(inputString, " \\(.*\\)$", "");

        //                    data = DateTime.ParseExact(inputString, "dd MMM HH:mm",
        //                        System.Globalization.CultureInfo.InvariantCulture);

        //                }

        //                tennisOdd.BeginingTime = data;

        //                var pairOneTitle = match
        //                    .First()
        //                    .InnerText.Trim('\n', ' ', '1', '2', '.', ',');

        //                var pairTwoTitle = match
        //                    .Last()
        //                    .InnerText.Trim('\n', ' ', '1', '2', '.', ',');

        //                tennisOdd.PairOne = pairOneTitle;
        //                tennisOdd.PairTwo = pairTwoTitle;


        //                string coefficientFirst = "";
        //                var cf = odd.Descendants("td")
        //                    .Where(node => node.GetAttributeValue("class", "")
        //                    .Equals("price height-column-with-price    first-in-main-row  coupone-width-2"))
        //                    .FirstOrDefault();

        //                if (cf == null)
        //                {
        //                    break;
        //                }
        //                else
        //                {
        //                    if (cf.InnerText.Trim('\n', ' ').Contains("("))
        //                    {
        //                        break;
        //                    }
        //                    coefficientFirst = cf.InnerText.Trim('\n', ' ');
        //                }


        //                string coefficientSecond = "";
        //                var cs = odd.Descendants("td")
        //                   .Where(node => node.GetAttributeValue("class", "")
        //                    .Equals("price height-column-with-price    coupone-width-2"))
        //                    .FirstOrDefault();

        //                if (cs == null)
        //                {
        //                    break;
        //                }
        //                else
        //                {
        //                    if (cs.InnerText.Trim('\n', ' ').Contains("("))
        //                    {
        //                        break;
        //                    }
        //                    coefficientSecond = cs.InnerText.Trim('\n', ' ');

        //                }

        //                tennisOdd.SportId = sport;
        //                tennisOdd.CoefficientFirst = coefficientFirst;
        //                tennisOdd.CoefficientSecond = coefficientSecond;
        //                tennisOdd.Time = DateTime.Now;

        //                listTennisOdds.Add(tennisOdd);
        //            }
        //        };
        //    }
        //    var cleanedList = listTennisOdds.DistinctBy(x => new { x.PairOne, x.PairTwo, x.SportId });

        //    return listTennisOdds;
        //}


        private string BuildId (string first, string second, DateTime dateTime)
        {
            var names = string.Concat(first, second).ToLower().OrderBy(c => c);

            return string.Concat(names, dateTime);
        }
    }
}




