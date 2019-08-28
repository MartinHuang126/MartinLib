using System.Net;
using System.Xml;
namespace WebServiceHelper
{
    /// <summary>
    /// Get方式访问
    /// </summary>
    public class WebServiceCallerByGet : WebServiceCaller
    {
        public override XmlDocument RequestWebService(ResquestParams rp)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(rp.URL + "/" + rp.MethodName + "?" + ParsToString(rp.Parames));
            request.Method = "GET";
            request.ContentType = "text/xml; charset=utf-8";
            SetWebRequest(request);
            return ReadXmlResponse(request.GetResponse());
        }
    }
}
