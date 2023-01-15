using System;
using System.Reflection;
using Topshelf;
using WIK.ServiceOrderMES.Config;
using WIK.ServiceOrderMES.Util;

namespace WIK.ServiceOrderMES
{
    class Program
    {
        static void Main(string[] _)
        {
            var exitCode = HostFactory.Run(x =>
            {
                AppSettings.AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                ConnectionNetwork();
                x.Service<StreamFile>(s =>
                {
                    s.ConstructUsing(streamfile => new StreamFile());
                    s.WhenStarted(streamfile => {
                        streamfile.Start();
                        EventLogUtil.LogEvent("WIK Service Order MES started successfully", System.Diagnostics.EventLogEntryType.Information, 3);
                    });
                    s.WhenStopped(streamfile => {
                        streamfile.Stop();
                        EventLogUtil.LogEvent("WIK Service Order MES stopped successfully", System.Diagnostics.EventLogEntryType.Information, 3);
                    });
                });
                x.RunAsLocalSystem();
                x.SetServiceName("WIK Service Order MES");
                x.SetDisplayName("WIK Service Order MES");
                x.SetDescription("This is service for MES WIK Service Order MES");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }

        public static void ConnectionNetwork()
        {
            try
            {
                NetworkUNC.Connect();
            }
            catch (Exception ex)
            {
                EventLogUtil.LogErrorEvent(AppSettings.AssemblyName == ex.Source ? MethodBase.GetCurrentMethod().Name : MethodBase.GetCurrentMethod().Name + "." + ex.Source, ex);
            }
        }
    }
}
