using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddMarathon.Services.Helpers
{
    public class OddsScraper
    {
        //public async Task<List<TennisOddViewModel>> BetMarathon()
        //{
        //    //IWebDriver driver = new ChromeDriver();

        //    //var url = "https://www.marathonbet.com/en/betting/Basketball/?menu=6";
        //    //var url = "https://www.marathonbet.com/en/betting/Tennis/?menu=2398";
        //    var url = "https://www.marathonbet.com/en/betting/Baseball/?menu=5";
        //    //var url = "https://www.marathonbet.com/en/betting/Snooker/?menu=2185";
        //    //var url = "https://www.marathonbet.com/en/betting/Darts/?menu=9";
        //    var httpClient = new HttpClient();
        //    var html = await httpClient.GetStringAsync(url);
        //    var htmlDoc = new HtmlDocument();

        //    htmlDoc.LoadHtml(html);

        //    var container = htmlDoc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("category-container")).ToList();


        //    List<TennisOddViewModel> lto = new List<TennisOddViewModel>();
        //    //List<TennisOdd> lto = new List<TennisOdd>();
        //    foreach (var item in container)
        //    {


        //        var tdp = item.Descendants("H2")
        //        .Where(node => node.GetAttributeValue("class", "")
        //        .Equals("category-label"))
        //        .FirstOrDefault()
        //        .InnerText;

        //        if (tdp == "Outright")
        //        {
        //            break;
        //        }
        //        //string turnirDataPocetok = tdp;

        //        var category_content = item.Descendants("tr").Where(node => node.GetAttributeValue("class", "").Equals(" event-header")).ToList();

        //        foreach (var item2 in category_content)
        //        {
        //            TennisOddViewModel to = new TennisOddViewModel();


        //            to.TurnirDataPocetok = tdp;


        //            var ParOneToday = item2.Descendants("td")
        //            .Where(node => node.GetAttributeValue("class", "")
        //            .Equals("today-name"));

        //            var DayAndHour = item2.Descendants("td")
        //                    .Where(node => node.GetAttributeValue("class", "")
        //                    .Equals("date "))
        //                    .FirstOrDefault().InnerText.Trim('\n', ' ');

        //            if (ParOneToday.Count() > 0)
        //            {
        //                var uu = ParOneToday.FirstOrDefault().InnerText.Trim('\n', ' ', '1', '2', '.');
        //                to.ParOne = uu;
        //                var uq = ParOneToday.LastOrDefault().InnerText.Trim('\n', ' ', '1', '2', '.');
        //                to.ParTwo = uq;


        //                var koeficientFirst = item2.Descendants("td")
        //                    .Where(node => node.GetAttributeValue("class", "")
        //                    .Equals("price height-column-with-price    first-in-main-row  "))
        //                    .FirstOrDefault().InnerText.Trim('\n', ' ');
        //                var koeficientSecond = item2.Descendants("td")
        //                    .Where(node => node.GetAttributeValue("class", "")
        //                    .Equals("price height-column-with-price    "))
        //                    .FirstOrDefault().InnerText.Trim('\n', ' ');



        //                to.KoeficientFirst = koeficientFirst;
        //                to.KoeficientSecond = koeficientSecond;
        //                to.DateAndBeginingTime = DayAndHour;
        //                to.DateAndTime = DateTime.Now;



        //            }
        //            else
        //            {
        //                string ParOneSoon = item2.Descendants("td")
        //                .Where(node => node.GetAttributeValue("class", "")
        //                .Equals("name")).FirstOrDefault()
        //                .InnerText.Trim('\n', ' ', '1', '2', '.');
        //                to.ParOne = ParOneSoon;

        //                var ParTwoSoon = item2.Descendants("td")
        //                .Where(node => node.GetAttributeValue("class", "")
        //                .Equals("name")).LastOrDefault()
        //                 .InnerText.Trim('\n', ' ', '1', '2', '.');
        //                to.ParTwo = ParTwoSoon;

        //                var koeficientFirst = item2.Descendants("td")
        //                    .Where(node => node.GetAttributeValue("class", "")
        //                    .Equals("price height-column-with-price    first-in-main-row  "))
        //                    .LastOrDefault().InnerText.Trim('\n', ' ');
        //                var koeficientSecond = item2.Descendants("td")
        //                    .Where(node => node.GetAttributeValue("class", "")
        //                    .Equals("price height-column-with-price    "))
        //                    .FirstOrDefault().InnerText.Trim('\n', ' ');

        //                to.KoeficientFirst = koeficientFirst;
        //                to.KoeficientSecond = koeficientSecond;
        //                to.DateAndBeginingTime = DayAndHour;
        //                to.DateAndTime = DateTime.Now;


        //            }
        //            lto.Add(to);
        //        };


        //    }
        //    return lto;

        //}
    }
}
