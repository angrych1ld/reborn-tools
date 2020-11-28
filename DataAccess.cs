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
            Stream dataStream = await http.GetStreamAsync("third-party-pages/l2reborn_npc-data_raids.data");

            HtmlParser parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(dataStream);

            IEnumerable<IElement> validParents = document.All.Where(e => e.TagName.Equals("DIV") && e.ClassName == "entry-content");

            IEnumerable<IElement> raidAnchors = validParents.SelectMany(p => p.Children.Where(e => e.TagName.Equals("A")
                && e.ClassName == "raid" && e.HasAttribute("HREF") && e.GetAttribute("HREF").Contains(@"/npc/")));

            return raidAnchors.Select(e => new RaidBoss
            {
                ID = uint.Parse(e.GetAttribute("href").Split("/", StringSplitOptions.RemoveEmptyEntries)[1]),
                Title = e.InnerHtml.Substring(0, e.InnerHtml.IndexOf("(")),
                Level = int.Parse(e.InnerHtml.Split("(", StringSplitOptions.RemoveEmptyEntries)[1].Split(")", StringSplitOptions.RemoveEmptyEntries)[0]),
                RebornSummaryAddress = @"https://l2reborn.com" + e.GetAttribute("href")
            }).ToList();
        }
    }
}
