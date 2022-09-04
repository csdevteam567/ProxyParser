namespace ProxyParser.Model
{
    public class Proxy
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public string IpAddress { get; set; }

        public int Port { get; set; }

        public string Country { get; set; }

        public string Anonymity { get; set; }
    }
}
