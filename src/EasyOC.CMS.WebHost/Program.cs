﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrchardCore.Logging;
using System;
using System.Threading.Tasks;

namespace EasyOC.CMS.WebHost
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            try
            {
                _ = Task.Run(() => {
                    try
                    {
                        DefaultUsing.Remove("<CppImplementationDetails>");
                        NatashaInitializer.Preheating();
                    }
                    catch (Exception)
                    {

                        throw;
                    }   
                });//Natasha 预热
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return BuildWebHost(args).RunAsync();
        }

        public static IHost BuildWebHost(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging => logging.ClearProviders())
                .ConfigureWebHostDefaults(webBuilder => webBuilder
                    .UseStartup<Startup>()
                    .UseNLogWeb()
                ).Build();

    }
}
