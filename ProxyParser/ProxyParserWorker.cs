using HtmlAgilityPack;
using ProxyParser.Model;
using System.Net;

namespace ProxyParser
{
    public class ProxyParserWorker
    {
        private ParserConfig config;
        private static object lockObj = new object();
        private DatabaseAccess dbAccess;

        public ProxyParserWorker(ParserConfig config)
        {
            this.config = config;
            dbAccess = new DatabaseAccess(AppConfiguration.Instance.ConnectionString);
        }

        private void ParseDocument(HtmlDocument doc)
        {
            HtmlNodeCollection contentNodes = null;
            contentNodes = doc.DocumentNode.SelectNodes(config.ContentTag);
            if(contentNodes != null)
            {
                int proxiesCount = 0;
                foreach(var node in contentNodes)
                {
                    var row = node.ChildNodes.Where(n => n.Name == "td").ToArray();
                    //string innerhtml = node.InnerHtml;
                    string ipAddress = row[1].InnerText;
                    int port = int.Parse(row[2].InnerText);
                    string country = row[3].InnerText;
                    string anonymity = row[4].InnerText;
                    string type = row[5].InnerText;
                    if(CheckProxy(ipAddress,port))
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

        public void ParserWorkerFunc()
        {
            HtmlWeb web = new HtmlWeb();
            try
            {
                HtmlDocument doc = web.Load(config.BaseUrl);

                if(doc != null)
                {
                    ParseDocument(doc);
                }

                HtmlNodeCollection pagerNodes = null;
                pagerNodes = doc.DocumentNode.SelectNodes(config.PagesTag);
                Uri uri = new Uri(config.BaseUrl);
                
                string host = "http://" + uri.Host;
                if(pagerNodes != null)
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public bool CheckProxy(string host, int port)
        {
            bool result = false;

            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(config.TestUrl);
            webRequest.UserAgent = @"PostmanRuntime/7.29.2";
            Uri uri = new Uri(config.TestUrl);
            webRequest.Host = uri.Host;
            webRequest.Headers.Add("Postman-Token", "d74f2c26-4c9b-4c66-83a1-f6d135506c4b");
            webRequest.Headers.Add("Accept", "*/*");
            //webRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            webRequest.Headers.Add("Connection", "keep-alive");

            webRequest.Method = "GET";
            webRequest.Proxy = new WebProxy(host, port);
            webRequest.Credentials = CredentialCache.DefaultCredentials;
            try
            {
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                if (webResponse.StatusCode == HttpStatusCode.OK)
                    result = true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }

        public void CheckProxyList()
        {
            List<Proxy> proxies = dbAccess.GetProxyList();
            if(proxies != null && proxies.Count > 0)
            {
                foreach(Proxy proxy in proxies)
                {
                    if (!CheckProxy(proxy.IpAddress, proxy.Port))
                        dbAccess.DeleteProxyRecord(proxy.Id);
                }
            }
        }
    }
}
