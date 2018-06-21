using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenniOddMarathon.Models
{
    public class TennisOdd
    {
        public int Id { get; set; }
        public string ParOne { get; set; }
        public string ParTwo { get; set; }
        public List<Koeficienti> Koefecienti { get; set; }
        public string DateAndBeginingTime { get; set; }
        public string TurnirDataPocetok { get; set; }

    }
}