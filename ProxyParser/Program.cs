﻿using HtmlAgilityPack;
using ProxyParser;
using ProxyParser.Model;
using System.IO.Compression;
using System.Net;

class Program
{
    static void Main(string[] args)
    {
        ConsoleKeyInfo option;
        ParserConfig config = new ParserConfig()
        {
            BaseUrl = "http://foxtools.ru/Proxy",
            PagesTag = "//div[@class=\"pager\"][1]/a",
            ContentTag = "//table[@id=\"theProxyList\"]/tbody/tr",
            TestUrl = "https://en.wikipedia.org/wiki/Main_Page"
        };

        //ParserConfig config = new ParserConfig()
        //{
        //    BaseUrl = "https://premproxy.com/list/",
        //    PagesTag = "//ul[@class=\"pagination\"][1]/li/a",
        //    ContentTag = "//table[@id=\"proxylist\"]/tbody/tr",
        //    TestUrl = "https://en.wikipedia.org/wiki/Main_Page"
        //};

        ProxyParserWorker parser = new ProxyParserWorker(config);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Parser start - 1");
            Console.WriteLine("Check stored proxies - 2");
            Console.WriteLine("Exit - 3");
            option = Console.ReadKey();

            if (option.KeyChar == '1')
            {
                Task.Run(() => parser.ParserWorkerFunc());
            }
            else if (option.KeyChar == '2')
            {
                //bool check = parser.CheckProxy("65.20.99.43", 11221);
                Task.Run(() => parser.CheckProxyList());
            }

            if (option.KeyChar == '3')
                break;
        }
    }
}

