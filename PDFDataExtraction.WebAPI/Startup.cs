using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JNysteen.FileTypeIdentifier;
using JNysteen.FileTypeIdentifier.Interfaces;
using JNysteen.FileTypeIdentifier.MagicNumbers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PDFDataExtraction.Generic;
using PDFDataExtraction.PDF2Txt;
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
            services.AddMvc(config =>
            {
                config.RespectBrowserAcceptHeader = true;
                config.ReturnHttpNotAcceptable = true;
                
                config.InputFormatters.Add(new XmlSerializerInputFormatter());
                config.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            var maxFileSizeInMB = 20 * 1024 * 1024;
            // Configure the web API to be able to receive as large files as possible
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = maxFileSizeInMB;
                options.MemoryBufferThreshold = maxFileSizeInMB;
                options.ValueLengthLimit = maxFileSizeInMB; 
                options.MultipartBodyLengthLimit = maxFileSizeInMB;
                options.MultipartBoundaryLengthLimit = maxFileSizeInMB;
            });

            var magicNumbers = new MagicNumberMapping();
            magicNumbers.AddMagicNumbers(DocumentMagicNumbers.PDFMagicNumbers, DocumentMagicNumbers.PDF);
            var fileTypeIdentifier = new FileTypeIdentifier(magicNumbers);
            services.AddSingleton<IFileTypeIdentifier>(fileTypeIdentifier);
            
            var pdf2TextWrapper = new PDF2TxtWrapper(); 
            services.AddSingleton<IPDF2TxtWrapper>(pdf2TextWrapper);
            
            services.AddScoped<ValidateInputPDFAttribute, ValidateInputPDFAttribute>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}