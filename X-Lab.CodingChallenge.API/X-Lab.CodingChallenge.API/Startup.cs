// <copyright file="Startup.cs" company="Bruno DUVAL">
// Copyright (c) Bruno DUVAL.</copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace XLab.CodingChallenge.API
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public static string CorsPolicyName = "XLabCodingChallengeOriginPolicy";
        public static string SwaggerEndpointVersion = "v1";
        public static string SwaggerEndpointName = "XLab Coding Challenge API";
        public static string SwaggerEndpoint = $"/swagger/{SwaggerEndpointVersion}/swagger.json";
        public static string HealthEndpoint = "/health";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Setup DI.

            // Add extra service for Diagnostic & Performance
            services.AddHealthChecks();
            services.AddLogging();
            services.AddMemoryCache();

            // Add basic Security & Regulation support
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            // Add API controllers
            services.AddControllers();

            // Add API Online help.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(SwaggerEndpointVersion,
                    new OpenApiInfo { Title = SwaggerEndpointName, Version = SwaggerEndpointVersion });
                c.EnableAnnotations();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors(CorsPolicyName);

            app.UseRouting();

            app.UseAuthorization();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(SwaggerEndpoint, SwaggerEndpointName);
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks(HealthEndpoint);
                endpoints.MapSwagger();
            });
        }
    }
}