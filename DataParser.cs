using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RebornTools
{
    public class DataParser
    {

        public static Recipe ParseRecipe(string data)
        {
            HtmlParser parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(data);

            if (document.All.Any(e => e.TagName == "IMG" && e.GetAttribute("ALT").StartsWith("PMfun :: Lineage 2")))
            {
                return ParseRecipe_PMFUN(document);
            }

            return new Recipe();
        }

        public static Recipe ParseRecipe_PMFUN(IHtmlDocument document)
        {
            IElement holder = document.All
                .First(e => e.TagName == "TABLE" && e.GetAttribute("CELLSPACING") == "0"
                    && e.GetAttribute("CELLPADDING") == "0" && e.ClassName == "txt")
                .Children.First(c => c.TagName == "TBODY")
                .Children.First(c => c.TagName == "TR")
                .Children.First(c => c.TagName == "TD");

            string headerText = holder.Children.Where(c => c.TagName == "B").Skip(1).First().InnerHtml;
            string quantitySection = headerText.Substring(headerText.IndexOf("quantity"), 14);
            string rateSection = headerText.Substring(headerText.IndexOf("rate"), 12);
            string quantityValueString = quantitySection.Split(",", StringSplitOptions.RemoveEmptyEntries)[0].Replace("quantity ", "");
            string rateValueString = rateSection.Split("%", StringSplitOptions.RemoveEmptyEntries)[0].Replace("rate ", "");

            IEnumerable<RecipeIngredient> ingredientsSelector = holder.Children
                .Where(c => c.TagName == "UL").Select(e => e.Children.First(c => c.TagName == "LI"))
                .Select(e => (
                    e.Children.First(c => c.TagName == "A" && c.GetAttribute("HREF").StartsWith("/item/")),
                    e.InnerHtml.Split(">", StringSplitOptions.RemoveEmptyEntries)[1].Split("<", StringSplitOptions.RemoveEmptyEntries)[0]
                ))
                .Select(e => new RecipeIngredient
                {
                    ItemID = uint.Parse(e.Item1.GetAttribute("HREF").Replace("/item/", "").Split("/", StringSplitOptions.RemoveEmptyEntries)[0]),
                    Amount = int.Parse(e.Item2.Replace("\"", ""))
                });

            return new Recipe
            {
                SuccessRate = int.Parse(rateValueString),
                Yields = int.Parse(quantityValueString),
                Ingredients = ingredientsSelector.ToList()
            };
        }

        public static List<L2Item> ParseItems(string data)
        {
            HtmlParser parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(data);

            if (document.All.Any(e => e.TagName == "IMG" && e.GetAttribute("ALT").StartsWith("PMfun :: Lineage 2")))
            {
                return ParseItems_PMFUN(document);
            }

            return new List<L2Item>();
        }

        public static List<L2Item> ParseItems_PMFUN(IHtmlDocument document)
        {
            IEnumerable<(IElement icon, IElement titleAnchor)> items = document.All
                .Where(e => e.ParentElement != null && e.TagName == "IMG" && e.GetAttribute("SRC").StartsWith("data/img/"))
                .Select(e => (e, e.ParentElement.Children.FirstOrDefault(c => c.TagName == "A" && c.GetAttribute("HREF").StartsWith("/item/"))))
                .Where(pair => pair.Item2 != null);

            return items.Select(item => new L2Item
            {
                ID = uint.Parse(item.titleAnchor.GetAttribute("HREF").Split("/", StringSplitOptions.RemoveEmptyEntries)[1]),
                Icon = item.icon.GetAttribute("SRC").Replace("data/img/", ""),
                Title = item.titleAnchor.InnerHtml
            }).GroupBy(i => i.ID).Select(g => g.First()).ToList();
        }
    }
}
