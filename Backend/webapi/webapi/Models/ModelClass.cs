using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapi.Models
{
    public class ModelClass
    {
        public int NetworkSId { get; set; }
        public DateTime DatatimeKey { get; set; }
        public int NeId { get; set; }
        public string Object { get; set; }
        public DateTime time { get; set; }
        public int Interval_t { get; set; }
        public string Direction { get; set; }
        public string NeAlias { get; set; }
        public string NeType { get; set; }
        public int RxLevelBelowTS1 { get; set; }
        public int RxLevelBelowTS2 { get; set; }
        public float MinRxLevel { get; set; }
        public float MaxRxLevel { get; set; }
        public int TxLevelAboveTS1 { get; set; }
        public float MinTxLevel { get; set; }
        public float MaxTxLevel { get; set; }
        public string FailureDescription { get; set; }
        public string Link { get; set; }
        public string TId { get; set; }
        public string FarendTId { get; set; }
        public string Slot { get; set; }
        public string Slot2 { get; set; }
        public string Port { get; set; }
     
    }
}
