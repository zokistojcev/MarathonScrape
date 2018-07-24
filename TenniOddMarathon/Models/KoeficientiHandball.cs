using System;

namespace TenniOddMarathon.Models
{
    public class KoeficientiHandball
    {
        public int Id { get; set; }
        public int OddId { get; set; }
        public string KoeficientHost { get; set; }
        public string KoeficientDraw { get; set; }
        public string KoeficientVisitors { get; set; }
        public DateTime DateAndTime { get; set; }
    }
}