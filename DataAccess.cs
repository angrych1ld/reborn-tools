using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RebornTools
{
    public class DataAccess
    {
        public static async Task<List<RaidBossStatus>> FetchRaidbossStatus(HttpClient httpClient)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, @"https://l2reborn.com/wp-content/uploads/raids/raids.json");
            req.SetBrowserRequestCache(BrowserRequestCache.NoCache);
            string rawResult = await (await httpClient.SendAsync(req)).Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(rawResult);

            List<RaidBossStatus> result = new List<RaidBossStatus>();

            foreach (JsonElement element in doc.RootElement.EnumerateArray())
            {
                try
                {
                    result.Add(new RaidBossStatus
                    {
                        ID = uint.Parse(element.GetProperty("id").GetString()),
                        Alive = int.Parse(element.GetProperty("status").GetString()) == 1
                    });

                }
                catch { }
            }

            return result;
        }

        public static async Task<List<RaidBoss>> FetchRaidbosses(HttpClient http)
        {
            Stream dataStream = await http.GetStreamAsync("data/raidbosses.json");
            return await JsonSerializer.DeserializeAsync<List<RaidBoss>>(dataStream);
        }

        public static async Task<List<RaidbossLocation>> FetchRaidbossLocations(HttpClient http)
        {
            Stream dataStream = await http.GetStreamAsync("data/raidboss-locations.json");
            return await JsonSerializer.DeserializeAsync<List<RaidbossLocation>>(dataStream);
        }

        public static async Task<List<RaidbossLocation>> PrintRaidbossLocations(HttpClient http)
        {
            Stream dataStream = await http.GetStreamAsync("third-party-pages/gracia_l2portal_raidboss-map.data");

            HtmlParser parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(dataStream);

            List<RaidbossLocation> result = new List<RaidbossLocation>();

            IEnumerable<IElement> anchors = document.All
                .Where(e => e.ClassName == "tt" && e.TagName == "A" && e.GetAttribute("HREF").StartsWith("Npc.aspx?ID="));

            Console.WriteLine(anchors.Count());

            foreach (IElement anchor in anchors)
            {
                RaidbossLocation loc = new RaidbossLocation { RaidbossID = uint.Parse(anchor.GetAttribute("HREF").Split("=", StringSplitOptions.RemoveEmptyEntries)[1]) };
                IElement middleSection = anchor.Children.FirstOrDefault(c => c.TagName == "SPAN" && c.ClassName == "tooltip")?
                    .Children.FirstOrDefault(c => c.TagName == "SPAN" && c.ClassName == "middle");

                string[] parts = middleSection.InnerHtml.Split(new string[] { "<br \\=\"\">", "\">" }, StringSplitOptions.RemoveEmptyEntries);

                loc.CoorX = ExtractCoordinate('X', parts);
                loc.CoorY = ExtractCoordinate('Y', parts);
                loc.CoorZ = ExtractCoordinate('Z', parts);

                result.Add(loc);
            }

            Console.WriteLine(JsonSerializer.Serialize(result));

            return result;
        }

        private static int ExtractCoordinate(char coordinate, string[] textParts)
        {
            return int.Parse((textParts.Where(p => p.Contains("Loc" + coordinate)).FirstOrDefault() ?? "Loc" + coordinate + ": 0").Split(":", StringSplitOptions.RemoveEmptyEntries)[1]);
        }
    }
}
