using System.Collections;

namespace WebServiceHelper
{
    public class ResquestParams
    {
        /// <summary>
        /// WebService地址
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// web方法
        /// </summary>
        public string MethodName { get; set; }
        /// <summary>
        /// 方法参数
        /// </summary>
        public Hashtable Parames { get; set; }
    }
}
