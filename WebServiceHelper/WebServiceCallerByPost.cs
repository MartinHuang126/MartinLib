using System.Collections;
using System.Net;
using System.Text;
using System.Xml;

namespace WebServiceHelper
{
    /// <summary>
    /// Post方式访问
    /// </summary>
    public class WebServiceCallerByPost : WebServiceCaller
    {
        public override XmlDocument RequestWebService(ResquestParams rp)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(rp.URL + "/" + rp.MethodName);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            SetWebRequest(request);
            byte[] data = EncodePars(rp.Parames);
            WriteRequestData(request, data);
            return ReadXmlResponse(request.GetResponse());
        }

        /// <summary>
        /// 参数编码
        /// </summary>
        /// <param name="Pars"></param>
        /// <returns></returns>
        private static byte[] EncodePars(Hashtable Pars)
        {
            return Encoding.UTF8.GetBytes(ParsToString(Pars));
        }

        /// <summary>
        /// 添加请求参数
        /// </summary>
        /// <param name="request"></param>
        /// <param name="data"></param>
        private static void WriteRequestData(HttpWebRequest request, byte[] data)
        {
            request.ContentLength = data.Length;
            System.IO.Stream writer = request.GetRequestStream();
            writer.Write(data, 0, data.Length);
            writer.Close();
        }

    }
}
