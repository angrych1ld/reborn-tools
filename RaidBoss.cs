using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RebornTools
{
    public class RaidBoss
    {
        public uint ID { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public bool? Alive { get; set; }
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }


        public string RebornSummaryAddress { get; set; }
        public string L2PortalLocationAddress { get; set; }
    }
}
