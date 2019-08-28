using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace CrawlUtility
{
    public class Crawler
    {

        /// <summary>
        /// 获取网页
        /// </summary>
        /// <param name="uri">爬虫URL地址</param>
        /// <param name="proxy">代理服务器</param>
        /// <returns>网页源代码</returns>
        public string getPage(string uri, string proxy = null)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            string pageSource = string.Empty;
            try
            {
                using (response = GetResponse(uri,proxy))
                {//获取请求响应
                    if (response.ContentEncoding.ToLower().Contains("gzip"))//解压
                    {
                        using (GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                        {
                            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                            {

                                pageSource = reader.ReadToEnd();
                            }
                        }
                    }
                    else if (response.ContentEncoding.ToLower().Contains("deflate"))//解压
                    {
                        using (DeflateStream stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress))
                        {
                            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                pageSource = reader.ReadToEnd();
                            }
                        }
                    }
                    else
                    {
                        using (Stream stream = response.GetResponseStream())//原始
                        {
                            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                pageSource = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
                //Log2Txt.logError(Log2Txt.GetMethodInfo() + e.Message + "Url:" + uri + "proxy:" + proxy);
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return pageSource;

        }

        public HttpWebResponse GetResponse(string uri,string proxy)
        {
            HttpWebResponse response = null;
            try
            {
                HttpWebRequest request = getRequest(uri, proxy);
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
                //Log2Txt.logError(Log2Txt.GetMethodInfo() + e.Message + "Url:" + uri + "proxy:" + proxy);
                //throw e;
                throw;
            }
            return response;
        }

        private  HttpWebRequest getRequest(string uri, string proxy)
        {
            HttpWebRequest request = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(uri);
                if (proxy != null) request.Proxy = new WebProxy(proxy);//设置代理服务器IP，伪装请求地址
                if (uri.StartsWith("https"))
                {
                    //处理HttpWebRequest访问https有安全证书的问题（ 请求被中止: 未能创建 SSL/TLS 安全通道。）
                    ServicePointManager.ServerCertificateValidationCallback += (s, cert, chain, sslPolicyErrors) => true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                }
                request.Accept = "*/*";
                request.ServicePoint.Expect100Continue = false;//加快载入速度
                request.ServicePoint.UseNagleAlgorithm = false;//禁止Nagle算法加快载入速度
                request.AllowWriteStreamBuffering = false;//禁止缓冲加快载入速度
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");//定义gzip压缩页面支持
                request.ContentType = "application/x-www-form-urlencoded";//定义文档类型及编码
                request.AllowAutoRedirect = true;//禁止自动跳转
                                                 //设置User-Agent，伪装成Google Chrome浏览器
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36";
                request.Timeout = 15000;//定义请求超时时间为5秒
                request.KeepAlive = true;//启用长连接
                request.Method = "GET";//定义请求方式为GET              

                request.ServicePoint.ConnectionLimit = int.MaxValue;//定义最大连接数
            }
            catch (Exception e)
            {
                //Log2Txt.logError(Log2Txt.GetMethodInfo() + e.Message + "Url:" + uri + "proxy:" + proxy);
                //throw e;
                throw;
            }          
            
            return request;
        }

        /// <summary>
        /// 根据XPath和Attibute获取html中的值。
        /// </summary>
        /// <param name="htmlDoc">html结构</param>
        /// <param name="xPath">抓取值得xPath路径</param>
        /// <param name="attribute">属性名字，可以为null</param>
        /// <returns>Attibute为空时返回html的内容List，不为空返回属性值List，xPath为空时返回null</returns>
        public List<string> getValuesByXPathAndAttribute(HtmlDocument htmlDoc,string xPath,string attribute=null)
        {
            List<string> list = new List<string>();
            HtmlNodeCollection htmlNodes = null;
            if (!string.IsNullOrEmpty(xPath))
            {
                htmlNodes = htmlDoc.DocumentNode.SelectNodes(xPath);
                if (htmlNodes == null)
                {
                    return list;
                }
                foreach (var item in htmlNodes)
                {
                    if (string.IsNullOrEmpty(attribute))
                    {
                        list.Add(item.InnerText);
                    }
                    else
                    {
                        list.Add(item.GetAttributeValue(attribute, ""));
                    }
                    
                }
            }
            return list;
        }

        /// <summary>
        /// 根据XPath和Attibute获取html中的值。
        /// </summary>
        /// <param name="htmlDoc">html结构</param>
        /// <param name="xPath">抓取值得xPath路径</param>
        /// <param name="attribute">属性名字，可以为null</param>
        /// <returns>Attibute为空时返回html的内容，不为空返回属性值，xPath为空时返回null</returns>
        public string getValueByXPathAndAttribute(HtmlDocument htmlDoc, string xPath, string attribute = null)
        {
            string value =string.Empty;
            HtmlNodeCollection htmlNodes = null;
            if (!string.IsNullOrEmpty(xPath))
            {
                htmlNodes = htmlDoc.DocumentNode.SelectNodes(xPath);
                if (htmlNodes == null)
                {
                    return value;
                }
                foreach (var item in htmlNodes)
                {
                    if (string.IsNullOrEmpty(attribute))
                    {
                        value=item.InnerText;
                    }
                    else
                    {
                        value = item.GetAttributeValue(attribute, "");
                    }              
                }
            }
            return value;
        }

        /// <summary>
        /// 判断网页是否存在
        /// </summary>
        /// <param name="uri">爬虫URL地址</param>
        /// <param name="proxy">代理服务器</param>
        /// <returns>网页源代码</returns>
        public bool isUrlExist(string uri,string proxy=null)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                response = GetResponse(uri,proxy);
            }
            catch (WebException ex)
            {
                if (ex.Status.ToString().Equals("NameResolutionFailure"))
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return true;

        }

    }
}
