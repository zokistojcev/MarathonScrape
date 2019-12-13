using System.Threading.Tasks;
using Unity;
using System.Diagnostics;
using System;
using System.Threading;
using ScrapySharp.Network;
using ScrapySharp.Extensions;
using System.Net;

namespace OddMarathon.Scraper
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }
        private static void example3()
        {
            var browser = new ScrapingBrowser();

            //set UseDefaultCookiesParser as false if a website returns invalid cookies format
            //browser.UseDefaultCookiesParser = false;
            Console.WriteLine("Open website http://www.ishadowsocks.org/");
            var homePage = browser.NavigateToPage(new Uri("http://www.ishadowsocks.org/"));

           
        }

        static async Task MainAsync(string[] args)
        {
            IUnityContainer container = new UnityContainer();
            DependencyInjection.RegisterDependency(container);

            var oddRequests = container.Resolve<OddRequests>();
 
            await oddRequests.GetFootballOddsFinal();         
            await oddRequests.GetTennisOddsFinal();

            Thread.Sleep(7000);
           
        }
        
    }
}
