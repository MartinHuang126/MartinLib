using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PhishCrawlSVC
{
    partial class PhishCrawlService : ServiceBase
    {
        private Process GatherProcess;

        public PhishCrawlService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            string programPath = ConfigurationManager.AppSettings["ProgramPath"];
            if (File.Exists(programPath))
            {
                //EventLog.WriteEntry("RecruitGatherSerivce Start."); 
                ProcessStartInfo info = new ProcessStartInfo(programPath);
                info.UseShellExecute = false;
                info.RedirectStandardError = true;
                info.RedirectStandardInput = true;
                info.RedirectStandardOutput = true;
                info.CreateNoWindow = false;
                info.ErrorDialog = false;
                info.WindowStyle = ProcessWindowStyle.Hidden;

                GatherProcess = Process.Start(info);
            }

            else
            {
                EventLog.WriteEntry("PhishCrawlService ConfigKey(ProgramPath) Is Empty Or {0} is not exists.", programPath);
            }
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            EventLog.WriteEntry("PhishCrawlService Stop.");
            if (GatherProcess != null && !GatherProcess.HasExited)
            {
                GatherProcess.Kill();
            }
        }
    }
}
