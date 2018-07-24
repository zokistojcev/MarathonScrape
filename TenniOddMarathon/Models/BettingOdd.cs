using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenniOddMarathon.Models
{
    public class BettingOdd
    {
        public int Id { get; set; }
        public string ParOne { get; set; }
        public string ParTwo { get; set; }
        public List<KoeficientiTennis> KoefecientiTennis { get; set; }
        public List<KoeficientiFootball> KoefecientiFootball { get; set; }
        public List<KoeficientiHandball> KoefecientiHandball { get; set; }
        public string DateAndBeginingTime { get; set; }
        public string TurnirDataPocetok { get; set; }
        public Sports Sport { get; set; }



    }

    public enum Sports
    {
        Football,
        Handball,
        Tennis,
        Cricket,
        Basketball
    }
}