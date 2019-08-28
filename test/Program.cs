using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrawlUtility;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            Crawler crawl = new Crawler();
            string str = crawl.getPage("https://www.tomtop.com");
        }
    }
}
