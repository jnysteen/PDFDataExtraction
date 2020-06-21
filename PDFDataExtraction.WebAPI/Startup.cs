using System.IO;
using System.IO.Compression;
using System.Reflection;
using JNysteen.FileTypeIdentifier;
using JNysteen.FileTypeIdentifier.Interfaces;
using JNysteen.FileTypeIdentifier.MagicNumbers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PDFDataExtraction.Generic;
using PDFDataExtraction.OCRMyPDF;
using PDFDataExtraction.PDF2Txt;
using PDFDataExtraction.PdfImageConversion;
using PDFDataExtraction.WebAPI.Extensions;
using PDFDataExtraction.WebAPI.Helpers;

namespace PDFDataExtraction.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                
            });
            services.AddControllers().AddNewtonsoftJson(c =>
                {
                    c.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                    c.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.All;
                });

            services.ConfigureSwaggerGen();
            
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.AddResponseCompression(options =>
            {
                options.MimeTypes = new[] { "application/json" };
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddSingleton<IFileTypeIdentifier>(_ => CreateFileTypeIdentifier());
            services.AddSingleton<IPDFMetadataProvider, PDFMetadataProvider>(services =>
            {
                var scriptsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Scripts/");
                return new PDFMetadataProvider(scriptsFolder);
            });

            services.AddScoped<IPDFTextExtractor, PDF2TxtWrapper>();
            services.AddScoped<IPDFToImagesConverter, GhostScriptWrapper>();
            services.AddScoped<IOCRPDFWrapper, OCRPDFWrapper>();

            services.AddScoped<ValidateInputPDFAttribute, ValidateInputPDFAttribute>();
        }

        private static FileTypeIdentifier CreateFileTypeIdentifier()
        {
            var magicNumbers = new MagicNumberMapping();
            magicNumbers.AddMagicNumbers(DocumentMagicNumbers.PDFMagicNumbers, DocumentMagicNumbers.PDF);
            var fileTypeIdentifier = new FileTypeIdentifier(magicNumbers);
            return fileTypeIdentifier;
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