using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RebornTools
{
    public class L2Item
    {
        public uint ID { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public Recipe Recipe { get; set; } = new Recipe();
        public int BuyPriceMin { get; set; }
        public int BuyPriceMax { get; set; }
        public uint CrystalType { get; set; }
        public int CrystalCount { get; set; }

        [JsonIgnore]
        public string IconPath => "l2-icons/" + Icon;
        [JsonIgnore]
        public IntRange CraftPrice { get; set; }
        [JsonIgnore]
        public IntRange CrystalPrice { get; set; }
        [JsonIgnore]
        public bool CraftCheaper { get; set; }
        [JsonIgnore]
        public bool CraftCalculated { get; set; }
        [JsonIgnore]
        public int BuyPriceMean => (BuyPriceMin + BuyPriceMax) / 2;
        [JsonIgnore]
        public int CraftPriceMean => (CraftPrice.Min + CraftPrice.Max) / 2;
        [JsonIgnore]
        public IntRange CraftBuyProfit => CraftPrice.IsZero || (BuyPriceMin == 0 && BuyPriceMax == 0) ? IntRange.Zero :
            new IntRange { Min = BuyPriceMin - CraftPrice.Max, Max = BuyPriceMax - CraftPrice.Min };
        [JsonIgnore]
        public IntRange LowerPrice => CraftCheaper ? CraftPrice : new IntRange { Min = BuyPriceMin, Max = BuyPriceMax };
    }
}
