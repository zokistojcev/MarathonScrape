﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenniOddMarathon.Models
{
    public class KoeficientiTennis
    {
        public int Id { get; set; }
        public int BettingOddId { get; set; }
        public string KoeficientFirst { get; set; }
        public string KoeficientSecond { get; set; }
        public DateTime DateAndTime { get; set; }
    }
}