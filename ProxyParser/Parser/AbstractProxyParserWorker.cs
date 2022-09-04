using HtmlAgilityPack;
using ProxyParser.Model;
using System.Net;

namespace ProxyParser.Parser
{
    public abstract class AbstractProxyParserWorker
    {
        protected ParserConfig config;
        protected static object lockObj = new object();
        protected DatabaseAccess dbAccess;

        public AbstractProxyParserWorker(ParserConfig config)
        {
            this.config = config;
            dbAccess = new DatabaseAccess(AppConfiguration.Instance.ConnectionString);
        }

        protected abstract void ParseDocument(HtmlDocument doc);

        public abstract void ParserWorkerFunc();

        public bool CheckProxy(string host, int port)
        {
            bool result = false;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(config.TestUrl);
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return result;
        }

        public void CheckProxyList()
        {
            List<Proxy> proxies = dbAccess.GetProxyList();
            if (proxies != null && proxies.Count > 0)
            {
                foreach (Proxy proxy in proxies)
                {
                    if (!CheckProxy(proxy.IpAddress, proxy.Port))
                        dbAccess.DeleteProxyRecord(proxy.Id);
                }
            }
        }
    }
}
