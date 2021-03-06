﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore;
using NLog.Web;
using System;
using MQTTnet.AspNetCore;
using Yu.Data.Infrasturctures;

namespace Yu.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 创建nlog捕捉异常
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("Program start!");
                var host = CreateHostBuilder(args).Build();
                host.InitialDataBase();
                host.Run();
            }
            catch (Exception ex)
            {
                // 捕捉启动异常
                logger.Error(ex, "Program stopped because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseKestrel(o =>
                        {
                            o.ListenAnyIP(1883, l => l.UseMqtt());
                            o.ListenAnyIP(5001, l => l.UseHttps());
                            o.ListenAnyIP(5000);
                        })
                        .UseStartup<Startup>();
                    })
                .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(LogLevel.Trace);
                    })
                .UseNLog();
    }
}
