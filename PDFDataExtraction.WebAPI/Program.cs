using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PDFDataExtraction.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                {
//                    options.Limits.MinRequestBodyDataRate =
//                        new MinDataRate(bytesPerSecond: 80, gracePeriod: TimeSpan.FromSeconds(20));
//                    options.Limits.MinResponseDataRate =
//                        new MinDataRate(bytesPerSecond: 80, gracePeriod: TimeSpan.FromSeconds(20));
//                    options.Limits.MaxRequestBodySize = null;
                })
                .UseStartup<Startup>();
    }
}