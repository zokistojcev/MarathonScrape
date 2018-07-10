using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenniOddMarathon.Models
{
    public class FootballOdd
    {
        public int Id { get; set; }
        public string Host { get; set; }
        public string Visitors { get; set; }
        public List<KoeficientiFootball> Koefecienti { get; set; }
        public string DateAndBeginingTime { get; set; }
        public string CountryLeagueDivision { get; set; }
    }
}