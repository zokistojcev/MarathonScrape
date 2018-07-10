using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TenniOddMarathon.Models
{
    public class KoeficientiFootball
    {
        public int Id { get; set; }
        public int FootballOddId { get; set; }
        public string KoeficientHost { get; set; }
        public string KoeficientDraw { get; set; }
        public string KoeficientVisitors { get; set; }
        public DateTime DateAndTime { get; set; }
    }
}