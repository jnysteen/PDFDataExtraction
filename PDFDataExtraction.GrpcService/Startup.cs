using JNysteen.FileTypeIdentifier;
using JNysteen.FileTypeIdentifier.Interfaces;
using JNysteen.FileTypeIdentifier.MagicNumbers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PDFDataExtraction.Generic;
using PDFDataExtraction.PDF2Txt;
using PDFDataExtraction.PdfImageConversion;

namespace PDFDataExtraction.GrpcService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddSingleton<IFileTypeIdentifier>(_ => CreateFileTypeIdentifier());
            services.AddScoped<IPDFTextExtractor, PDF2TxtWrapper>();
            services.AddSingleton<IPDFMetadataProvider, PDFMetadataProvider>();
            services.AddScoped<IPDFToImagesConverter, GhostScriptWrapper>();
            services.AddScoped<IPDFDataExtractionService, PDFDataExtractionService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<Services.PDFDataExtractionGrpcService>();

                endpoints.MapGet("/",
                    async context =>
                    {
                        await context.Response.WriteAsync(
                            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                    });
            });
        }
        
        private static FileTypeIdentifier CreateFileTypeIdentifier()
        {
            var magicNumbers = new MagicNumberMapping();
            magicNumbers.AddMagicNumbers(DocumentMagicNumbers.PDFMagicNumbers, DocumentMagicNumbers.PDF);
            var fileTypeIdentifier = new FileTypeIdentifier(magicNumbers);
            return fileTypeIdentifier;
        }
    }
}