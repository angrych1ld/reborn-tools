using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RebornTools
{
    public class RaidBoss
    {
        public enum LocationSourceType { Unknown = 0, GraciaPortal = 1, L2RebornManual = 2}

        public uint ID { get; set; } 
        public string Title { get; set; }
        public int Level { get; set; }
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
        public int CoordinateZ { get; set; }
        public LocationSourceType LocationSource { get; set; }

        [JsonIgnore]
        public bool? Alive { get; set; }
        [JsonIgnore]
        public string L2PortalLocationAddress { get; set; }
        [JsonIgnore]
        public int AliveInt => Alive == null ? 2 : (Alive.Value ? 0 : 1);
    }
}
