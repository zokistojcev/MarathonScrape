using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenniOddMarathon.ViewModels
{
    public class FootballOddViewModel
    {
        public int Id { get; set; }     
        public string KoeficientHost { get; set; }
        public string KoeficientDraw { get; set; }
        public string KoeficientVisitors { get; set; }
        public DateTime DateAndTime { get; set; }
        public string DateAndBeginingTime { get; set; }
        public string ParOne { get; set; }
        public string ParTwo { get; set; }
        public string TurnirDataPocetok { get; set; }
    }
}