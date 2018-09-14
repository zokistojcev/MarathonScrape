using System;


namespace OddMarathon.Services.Dtos
{
    public abstract class BaseOdd
    {
        public int Id { get; set; }
        public string PairOne { get; set; }
        public string PairTwo { get; set; }
        public DateTime BeginingTime { get; set; }
        public string Tournament { get; set; }
        public byte SportId { get; set; }      
    } 

}
