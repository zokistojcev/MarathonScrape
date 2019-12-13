using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OddMarathon.Scraper.Scrapers
{
    public interface IOddsScraper
    {
        Task GetOdds();
    }
}
