using System;
using System.Configuration;


namespace RedisHelper
{
    public class RedisConfigInfo
    {
    //    <add key = "WriteServerList" value="127.0.0.1:6379"/>
    //<add key = "ReadServerList" value="127.0.0.1:6379"/>
    //<add key = "DefaultDb" value="0"/>
    //<add key = "MaxWritePoolSize" value="60"/>
    //<add key = "MaxReadPoolSize" value="60"/>
    //<add key = "AutoStart" value="true"/>
    //<add key = "LocalCacheTime" value="1800"/>
    //<add key = "RecordeLog" value="false"/>
        public static long DefaultDb = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultDb"]);
        public static string WriteServerList = ConfigurationManager.AppSettings["WriteServerList"];
        public static string ReadServerList = ConfigurationManager.AppSettings["ReadServerList"];
        public static int MaxWritePoolSize = Convert.ToInt32(ConfigurationManager.AppSettings["MaxWritePoolSize"]);
        public static int MaxReadPoolSize = Convert.ToInt32(ConfigurationManager.AppSettings["MaxReadPoolSize"]);
        public static int LocalCacheTime = Convert.ToInt32(ConfigurationManager.AppSettings["LocalCacheTime"]);
        public static bool AutoStart = ConfigurationManager.AppSettings["AutoStart"].Equals("true") ? true : false;
    }
}
