using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using JNysteen.FileTypeIdentifier;
using JNysteen.FileTypeIdentifier.Interfaces;
using JNysteen.FileTypeIdentifier.MagicNumbers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PDFDataExtraction.GhostScript;
using PDFDataExtraction.PDF2Txt;
using PDFDataExtraction.WebAPI.Extensions;
using PDFDataExtraction.WebAPI.Helpers;

namespace PDFDataExtraction.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddControllers();

            services.ConfigureSwaggerGen();
            
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = new[] { "application/json" };
                //                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            var magicNumbers = new MagicNumberMapping();
            magicNumbers.AddMagicNumbers(DocumentMagicNumbers.PDFMagicNumbers, DocumentMagicNumbers.PDF);
            var fileTypeIdentifier = new FileTypeIdentifier(magicNumbers);
            services.AddSingleton<IFileTypeIdentifier>(fileTypeIdentifier);

            var pdf2TextWrapper = new PDF2TxtWrapper();
            services.AddSingleton<IPDF2TxtWrapper>(pdf2TextWrapper);

            services.AddScoped<IPdfToImagesConverter, GhostScriptWrapper>();

            services.AddScoped<ValidateInputPDFAttribute, ValidateInputPDFAttribute>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureSwaggerUI();
            
            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}