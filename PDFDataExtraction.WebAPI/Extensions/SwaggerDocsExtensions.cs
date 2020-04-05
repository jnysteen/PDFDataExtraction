using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
// ReSharper disable InconsistentNaming

namespace PDFDataExtraction.WebAPI.Extensions
{
    public static class SwaggerDocsExtensions
    {
        public const string ApiTitle = "PDF Data Extraction";
        public const string ApiVersion = "v1";

        public static void ConfigureSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(ApiVersion, new OpenApiInfo
                    {
                        Title = ApiTitle,
                        Version = ApiVersion
                    });

                    // Set the comments path for the Swagger JSON and UI.
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                }
            );
            services.AddSwaggerGenNewtonsoftSupport();
        }

        public static void ConfigureSwaggerUI(this IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                // TODO HACK To support NSwag client generation, we need to use Swagger JSON 2.0 - not anything never :(
                //  When this issue is closed, we can upgrade: https://github.com/RicoSuter/NSwag/issues/2419
                c.SerializeAsV2 = true; 
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{ApiTitle} ({ApiVersion})");
                c.RoutePrefix = "";
            });
        }
    }
}