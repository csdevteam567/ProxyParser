using HtmlAgilityPack;
using ProxyParser.Model;
using System.Net;

namespace ProxyParser.Parser
{
    public class FoxtoolsParserWorker : AbstractProxyParserWorker
    {
        public FoxtoolsParserWorker(ParserConfig config) : base(config)
        {
        }

        protected override void ParseDocument(HtmlDocument doc)
        {
            HtmlNodeCollection contentNodes = null;
            contentNodes = doc.DocumentNode.SelectNodes(config.ContentTag);
            if (contentNodes != null)
            {
                int proxiesCount = 0;
                foreach (var node in contentNodes)
                {
                    var row = node.ChildNodes.Where(n => n.Name == "td").ToArray();
                    //string innerhtml = node.InnerHtml;
                    string ipAddress = row[1].InnerText;
                    int port = int.Parse(row[2].InnerText);
                    string country = row[3].InnerText;
                    string anonymity = row[4].InnerText;
                    string type = row[5].InnerText;
                    if (CheckProxy(ipAddress, port))
                    {
                        lock (lockObj)
                        {
                            dbAccess.AddProxyRecord(new Proxy()
                            {
                                Type = type,
                                IpAddress = ipAddress,
                                Port = port,
                                Country = country,
                                Anonymity = anonymity
                            });
                        }
                    }
                    proxiesCount++;
                }
                Console.WriteLine("Proxies count: " + proxiesCount.ToString());
            }
        }

        public override void ParserWorkerFunc()
        {
            HtmlWeb web = new HtmlWeb();
            try
            {
                HtmlDocument doc = web.Load(config.BaseUrl);

                if (doc != null)
                {
                    ParseDocument(doc);

                    HtmlNodeCollection pagerNodes = null;
                    pagerNodes = doc.DocumentNode.SelectNodes(config.PagesTag);
                    Uri uri = new Uri(config.BaseUrl);

                    string host = "http://" + uri.Host;
                    if (pagerNodes != null)
                    {
                        foreach (var node in pagerNodes)
                        {
                            string pageUri = node.Attributes.Where(a => a.Name == "href").First().Value;
                            HtmlDocument page = null;
                            page = web.Load(host + pageUri);
                            if (page != null)
                            {
                                ParseDocument(page);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
