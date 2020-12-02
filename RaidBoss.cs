using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RebornTools
{
    public class RaidBoss
    {
        public enum LocationSourceType { Unknown = 0, GraciaPortal = 1, L2RebornManual = 2 }
        public enum LocationEnvironmentType
        {
            Unknown = 0,
            Outside = 1,
            InsideBuilding = 2,
            InsideRuins = 3,
            InsideCatacombs = 4,
            InsideNecropolis = 5,
            InsideFortress = 6,
            InsideTemple = 7,
            SchoolOfDarkArts = 8,
            TopOfMountain = 9
        }

        public uint ID { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
        public int CoordinateZ { get; set; }
        public LocationSourceType LocationSource { get; set; }
        public LocationEnvironmentType LocationEnvironment { get; set; }

        [JsonIgnore]
        public bool? Alive { get; set; }
        [JsonIgnore]
        public string L2PortalLocationAddress { get; set; }
        [JsonIgnore]
        public int AliveInt => Alive == null ? 2 : (Alive.Value ? 0 : 1);
        [JsonIgnore]
        public string EnvironmentTypeTitle
        {
            get
            {
                switch (LocationEnvironment)
                {
                    case LocationEnvironmentType.Outside:
                        return "Outside";
                    case LocationEnvironmentType.InsideBuilding:
                        return "Building";
                    case LocationEnvironmentType.InsideRuins:
                        return "Ruins";
                    case LocationEnvironmentType.InsideCatacombs:
                        return "Catacombs";
                    case LocationEnvironmentType.InsideNecropolis:
                        return "Necropolis";
                    case LocationEnvironmentType.InsideFortress:
                        return "Fortress";
                    case LocationEnvironmentType.InsideTemple:
                        return "Temple";
                    case LocationEnvironmentType.SchoolOfDarkArts:
                        return "School of DA";
                    case LocationEnvironmentType.TopOfMountain:
                        return "Top of Mountain";
                    default:
                        return "Unknown";
                }
            }
        }

    }
}
