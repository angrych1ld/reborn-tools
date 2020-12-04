using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace RebornTools
{
    public class L2ItemService
    {
        private Dictionary<uint, L2Item> items;

        public async Task Load(HttpClient http)
        {
            items = (await DataAccess.GetItems(http)).OrderBy(i => i.Title).ToDictionary(i => i.ID, i => i);
            RecalculatePrices();
        }

        public List<L2Item> GetItems()
        {
            return items.Values.ToList();
        }

        public L2Item GetItem(uint id, bool clone)
        {
            if (items.TryGetValue(id, out L2Item item))
            {
                if (clone)
                {
                    return JsonSerializer.Deserialize<L2Item>(JsonSerializer.Serialize(item));
                }
                else
                {
                    return item;
                }
            }
            return null;
        }

        public void UpdateItem(uint id, L2Item item)
        {
            item.ID = id;

            if (!items.ContainsKey(id)) return;

            items[id] = item;
            RecalculatePrices();
        }

        public List<(uint id, string error)> AddItems(IEnumerable<L2Item> newItems)
        {
            List<(uint id, string error)> errors = new List<(uint id, string error)>();

            foreach (L2Item newItem in newItems)
            {
                if (!items.ContainsKey(newItem.ID))
                {
                    items.Add(newItem.ID, newItem);
                }
                else
                {
                    errors.Add((newItem.ID, "Duplicate ID: " + newItem.ID + ", " + newItem.Title));
                }
            }

            RecalculatePrices();

            return errors;
        }

        public string GetItemsSerialized()
        {
            return JsonSerializer.Serialize(items.Values.ToList());
        }

        private void RecalculatePrices()
        {
            // Reset everything
            foreach (L2Item item in items.Values)
            {
                item.CrystalPrice = IntRange.Zero;
                item.CraftPrice = IntRange.Zero;
                item.CraftCheaper = false;
                item.CraftCalculated = false;
            }

            // Calculate crystal values
            foreach (L2Item item in items.Values)
            {
                if (items.TryGetValue(item.CrystalType, out L2Item crystalItem))
                {
                    item.CrystalPrice = new IntRange
                    {
                        Min = crystalItem.BuyPriceMin * item.CrystalCount,
                        Max = crystalItem.BuyPriceMax * item.CrystalCount
                    };
                }
            }

            // Calculate craft price
            foreach (L2Item item in items.Values)
            {
                CalculateCraftPrice(item);
            }
        }

        private void CalculateCraftPrice(L2Item item)
        {
            if (item.CraftCalculated) return;

            if (item.Recipe.Ingredients.Count > 0)
            {
                List<(L2Item item, int amount)> ingredientItems = item.Recipe.Ingredients.Select(i => (items[i.ItemID], i.Amount)).ToList();

                // Calculate the price of every ingredient
                foreach (RecipeIngredient ing in item.Recipe.Ingredients)
                {
                    CalculateCraftPrice(items[ing.ItemID]);
                }

                // If at least 1 price is missing, can't calculate the craft price
                if (ingredientItems.Any(i => i.item.BuyPriceMin == 0 && i.item.BuyPriceMax == 0 && i.item.CraftPrice.IsZero) || item.Recipe.Yields == 0)
                {

                }
                else
                {
                    item.CraftPrice = new IntRange
                    {
                        Min = ingredientItems.Sum(i => (i.item.CraftCheaper ? i.item.CraftPrice.Min : i.item.BuyPriceMin) * i.amount)
                            + item.Recipe.CraftPriceMin,
                        Max = ingredientItems.Sum(i => (i.item.CraftCheaper ? i.item.CraftPrice.Max : i.item.BuyPriceMax) * i.amount)
                            + item.Recipe.CraftPriceMax
                    };

                    // Adjust for recipe quantity and success rate
                    item.CraftPrice = new IntRange
                    {
                        Min = (int)(item.CraftPrice.Min * ((double)item.Recipe.SuccessRate / 100) / item.Recipe.Yields),
                        Max = (int)(item.CraftPrice.Max * ((double)item.Recipe.SuccessRate / 100) / item.Recipe.Yields)
                    };

                    item.CraftCheaper = item.CraftPriceMean < item.BuyPriceMean || (item.BuyPriceMin == 0 && item.BuyPriceMax == 0);
                }
            }

            item.CraftCalculated = true;
        }
    }
}
