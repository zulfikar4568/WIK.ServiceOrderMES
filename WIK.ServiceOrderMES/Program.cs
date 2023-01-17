using Autofac;
using System;
using System.Collections.Specialized;
using System.Reflection;
using Topshelf;
using Topshelf.Autofac;
using WIK.ServiceOrderMES.Config;
using WIK.ServiceOrderMES.Util;
using Autofac.Extras.Quartz;
using Quartz;
using System.Threading;
using System.Threading.Tasks;
using Quartz.Logging;

namespace WIK.ServiceOrderMES
{
    class Program
    {
        static void Main(string[] _)
        {
            AppSettings.AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            // Setup Network
            ConnectionNetwork();

            // Setup DI
            var containerBuilder = DependendyInjectionBuilder(new ContainerBuilder());
            var container = containerBuilder.Build();

            #if DEBUG
                Console.WriteLine("Start TopSelf");
            #endif

            var exitCode = HostFactory.Run(x =>
            {
                // Pass it to Topshelf
                x.UseAutofacContainer(container);

                x.Service<ServiceMain>(s =>
                {
                    s.ConstructUsingAutofacContainer();
                    s.WhenStarted(service => {
                        service.Start();
                        EventLogUtil.LogEvent("WIK Service Order MES started successfully", System.Diagnostics.EventLogEntryType.Information, 3);
                    });
                    s.WhenStopped(service => {
                        service.Stop();
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

        private static ContainerBuilder DependendyInjectionBuilder(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule(new Driver.Driver());
            containerBuilder.RegisterModule(new Repository.Repository());
            containerBuilder.RegisterModule(new UseCase.UseCase());
            containerBuilder.RegisterModule(new Util.Util());
            containerBuilder.RegisterType<ServiceMain>().As<ServiceMain>();

            // configure and register Quartz
            var schedulerConfig = new NameValueCollection {
                { "quartz.scheduler.instanceName", "MyScheduler" },
                { "quartz.jobStore.type", "Quartz.Simpl.RAMJobStore, Quartz" },
                { "quartz.threadPool.threadCount", "3" }
            };

            // Log for Quartz
            LogProvider.SetCurrentLogProvider(new Job.ConsoleLogProvider());

            containerBuilder.RegisterModule(new QuartzAutofacFactoryModule
            {
                ConfigurationProvider = ctx => schedulerConfig
            });

            containerBuilder.RegisterModule(new QuartzAutofacJobsModule(typeof(Job.OrderJob).Assembly));
            containerBuilder.RegisterModule(new QuartzAutofacJobsModule(typeof(Job.OrderBOMJob).Assembly));
            return containerBuilder;
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
