using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Reflection;
using LogHelper;

namespace CrawlUtility
{
    public class ProxyCrawler : Crawler
    {
        private static object o = new object();
        private static ProxyCrawler proxyCrawler = null;
        
        public List<string> Proxy { get; private set; }
        private readonly string[] proxySites = {
            "https://www.xicidaili.com/nn/1",
            "https://www.xicidaili.com/nt/1",
            "https://www.xicidaili.com/wn/1",
            "https://www.xicidaili.com/wt/1"
        };
        //private readonly string testSite = "http://www.ipchina.com/";
        public string testSite { get; set; }
        public static string proxyFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "proxy.txt");
        private readonly string ipXPath = "//div[@id='body']//tr/td[2]";
        private readonly string portXPath = "//div[@id='body']//tr/td[3]";
        private readonly string maxPagNumXPath = "//div[@id='body']//a[10]";


        private  ProxyCrawler()
        {
            Proxy = new List<string>();
        }
        public static ProxyCrawler getProxyCrawler()
        {
            if (proxyCrawler == null)
            {
                lock (o)
                {
                    if (proxyCrawler == null)
                    {
                        proxyCrawler = new ProxyCrawler();  
                    }
                }
            }
            return proxyCrawler;
        }

        public void removeProxy(string proxy)
        {
            if (proxy == null)
            {
                return;
            }
            if (!isVaildProxy(proxy))
            {
                Proxy.Remove(proxy);
                Log2Txt.logInfo("移除代理：" + proxy);
            }
        }

        public void start()
        {
            while (true)
            {
                try
                {
                    if (File.Exists(proxyFilePath))
                    {
                        getProxyFormFile();
                    }
                    crawlProxy();
                }
                catch (Exception e)
                {
                    Log2Txt.logError(Log2Txt.GetMethodInfo() + e.Message);
                }
            }

        }

        public string getProxy()
        {
            string itemProxy = null;
            try
            {
                if (!Proxy.Contains(null))
                {
                    Proxy.Add(null);
                    new Thread(new ThreadStart(start)).Start();
                }
                Random random = new Random();
                int index = random.Next(0, Proxy.Count);
                itemProxy = Proxy[index];
            }
            catch (Exception e)
            {
                Log2Txt.logError(Log2Txt.GetMethodInfo() + e.Message);
            }
            return itemProxy;
        }

        private void crawlProxy()
        {
            HtmlDocument html = new HtmlDocument();
            List<string> ipList = new List<string>();
            List<string> portList = new List<string>();
            int maxPageNum = 0;
            string page = string.Empty;
            int pageIndex = 1;
            string itemUrl = string.Empty;
            string itemProxy = string.Empty;
            try
            {
                for (int i = 0; i < proxySites.Length; i++)
                {
                    pageIndex = 1;
                    itemUrl = proxySites[i];
                    itemProxy = getProxy();
                    page = getPage(itemUrl);
                    if (string.IsNullOrEmpty(page))
                    {
                        continue;
                    }
                    html.LoadHtml(page);
                    string item = getValueByXPathAndAttribute(html, maxPagNumXPath);
                    maxPageNum = Convert.ToInt32(item);
                    while (maxPageNum >= pageIndex)
                    {
                        
                        do
                        {
                            itemProxy = getProxy();
                            Log2Txt.logInfo(Log2Txt.GetMethodInfo() + "page(" + pageIndex + "/" + maxPageNum + "):" + "currentUrl:" + itemUrl + " currentProxy:" + itemProxy + "\r\n");
                            page = getPage(itemUrl, itemProxy);
                            html.LoadHtml(page);
                            ipList = getValuesByXPathAndAttribute(html, ipXPath);
                            portList = getValuesByXPathAndAttribute(html, portXPath);
                            for (int j = 0; j < ipList.Count; j++)
                            {
                                string str = ipList[j].ToString() + ":" + portList[j].ToString();
                                if (!isVaildProxy(str))
                                {
                                    continue;
                                }
                                if (Proxy.Contains(str))
                                {
                                    continue;
                                }
                                Proxy.Add(str);
                                Log2Txt.logInfo("Find a proxy:" + str);
                            }
                            if (ipList.Count > 0)
                            {
                                pageIndex++;
                                itemUrl = proxySites[i].Replace("1", pageIndex.ToString());
                            }
                            else
                            {
                                removeProxy(itemProxy);
                            }
                            Thread.Sleep(1000);
                        } while (ipList.Count == 0);
                    }
                    if (Proxy.Count > 1)
                    {
                        setProxysToFile();
                    }
                }
            }
            catch (Exception e)
            {
                Log2Txt.logError(Log2Txt.GetMethodInfo() + e.Message);
            }

        }

        public void getProxyFormFile()
        {
            try
            {
                if (!File.Exists(proxyFilePath))
                {
                    File.Create(proxyFilePath).Close();
                }
                Proxy.AddRange(File.ReadAllLines(proxyFilePath).ToList());
            }
            catch (IOException e)
            {
                Log2Txt.logError(Log2Txt.GetMethodInfo() + e.Message);
            }

        }

        public void setProxysToFile()
        {
            try
            {
                File.WriteAllLines(proxyFilePath, Proxy);
            }
            catch (IOException e)
            {
                Log2Txt.logError(Log2Txt.GetMethodInfo() + e.Message);
            }

        }
        public void setProxyToFile(string proxy)
        {
            try
            {
                File.AppendAllText(proxyFilePath, proxy+"\r\n");
            }
            catch (IOException e)
            {
                Log2Txt.logError(Log2Txt.GetMethodInfo() + e.Message);
            }

        }

        private bool isVaildProxy(string proxy)
        {
            if (GetResponse(testSite, proxy) == null)
            {
                return false;
            }
            return true;
        }
    }
}
