using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Reflection;

namespace LogHelper
{
    public static class Log2Txt
    {
        private static string BasePath { get; set; }
        private static string LogInfoPath { get; set; }
        private static string LogErrorPath { get; set; }
        private static object oInfo = new object();
        private static object oError = new object();
        public static Encoding encoding { get; set; }

        static Log2Txt()
        {
            BasePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "log");
            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
            LogErrorPath = Path.Combine(BasePath, "logerror{0}.txt");
            LogInfoPath = Path.Combine(BasePath, "loginfo{0}.txt");
            encoding = Encoding.UTF8;
        }

        public static void logInfo(string info)
        {
            lock (oInfo)
            {
                writeLog(LogInfoPath.Replace("{0}", DateTime.Now.ToString("yyyy-MM-dd")), info);
            }
            
        }

        public static void logError(string error)
        {
            lock (oError)
            {
                writeLog(LogErrorPath.Replace("{0}", DateTime.Now.ToString("yyyy-MM-dd")), error);
            }
            
        }

        private static void writeLog(string filePath, string content)
        {
            try
            {
                File.AppendAllText(filePath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + content + "\r\n", encoding);
            }
            catch (IOException e)
            {
                logError(e.Message);
            }

        }
        public static string GetMethodInfo()
        {
            string str = "";
            StackTrace stackTrace = new StackTrace();
            try
            {
                //取得当前方法命名空间    
                //str += "命名空间名:" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace + "\n";
                str += "命名空间名:" + stackTrace.GetFrame(1).GetMethod().DeclaringType.Namespace + "\n";

                //取得当前方法类全名 包括命名空间    
                //str += "类名:" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "\n";
                str += "类名:" + stackTrace.GetFrame(1).GetMethod().ReflectedType.Name + "\n";

                //取得当前方法名    
                //str += "方法名:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\n";
                str += "方法名:" + stackTrace.GetFrame(1).GetMethod().Name + "\n";
                //str += "\n";
            }
            catch (Exception e)
            {
                logError("Method:GetMethodInfo.Exception:" + e.Message);
            }
            return str;
        }
    }
}
