using HtmlAgilityPack;
using ProxyParser.Model;

namespace ProxyParser.Parser
{
    public class FreeproxylistParserWorker : AbstractProxyParserWorker
    {
        public FreeproxylistParserWorker(ParserConfig config) : base(config)
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
                    string ipAddress = row[0].InnerText;
                    int port = int.Parse(row[1].InnerText);
                    string country = row[3].InnerText;
                    string anonymity = row[4].InnerText;
                    string type = "HTTP";
                    if (row[6].InnerText == "yes")
                        type = "HTTPS";
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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
