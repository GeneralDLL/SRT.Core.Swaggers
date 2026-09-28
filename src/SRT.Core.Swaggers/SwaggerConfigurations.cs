// Ignore Spelling: yyyy mm dd rahimzadeh srt

using Microsoft.AspNetCore.Builder;
using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;
using SRT.Core.Swaggers.Domain;
using SRT.Core.SRTExtensions;
using Microsoft.OpenApi;

namespace SRT.Core.Swaggers
{
    public static class SwaggerConfigurations
    {
        /// <summary>
        /// Builder , isApplyed
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configApp"></param>
        /// <returns> Builder , isApplyed </returns>
        public static (WebApplicationBuilder, bool) SRT_AddSwaggerGen(this WebApplicationBuilder builder, AppSetting configApp)
        {
            if (configApp.SwaggerConfig is null ||
                (IsProduction(configApp) && !configApp.SwaggerConfig.SwaggerInProduction))
            {
                return (builder, false);
            }

            var config = configApp.SwaggerConfig;
            var SQLConnection = configApp.ConnectionStrings;

            var v = "<h3><b>Environment: " + configApp.CoreSystemENV.environment.SRT_Enum_GetDescription() + "</b></h3><br />";

            v += GetDescription(SQLConnection.DefaultConnection);

            v += GetDescription(SQLConnection.SRTCore_CacheSQL);
            v += GetDescription(SQLConnection.SRTCore_CacheRedis);
            v += GetDescription(SQLConnection.SRTCore_ErrorSQL);
            v += GetDescription(SQLConnection.SRTCore_RabbitMQ);

            #region Create Descriptions
            var Description = $@"<pre>
{v}
</pre>
";

            Description = Description.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Description = Description.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Description = Description.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Description = Description.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            #endregion

            #region Add Swagger in Builder
            builder.Services.AddSwaggerGen(option =>
            {
                #region Add Versions
                config.lstVersions.ForEach(ver =>
                {
                    ver.Title = ver.Title + " - " + DateTime.Now.ToString("yyyy/MM/dd - HH:mm");
                    ver.Description += Description;

                    option.SwaggerDoc(ver.Version, ver);
                });
                #endregion

                #region Include XML comments
                try
                {
                    var xmlFile = $"{builder.Environment.ApplicationName}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    option.IncludeXmlComments(xmlPath);
                }
                catch { }
                #endregion

                // Use the version API explorer to create Swagger documents
                #region Filter View Of versions
                option.DocInclusionPredicate((version, apiDescription) =>
                {
                    var CurrentVersion = double.Parse(version.ToLower()
                                                             .Split('-')[0]
                                                             .Trim()
                                                             .Replace("v", ""));

                    var versionsNormal = apiDescription.ActionDescriptor
                        .EndpointMetadata
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(v => v.Versions)
                        .Select(v => double.Parse(v.ToString()))
                        .ToImmutableList();

                    if (versionsNormal.Any(v => v == CurrentVersion))
                        return true;

                    var versionsMap = apiDescription.ActionDescriptor
                        .EndpointMetadata
                        .OfType<MapToApiVersionAttribute>()
                        .SelectMany(v => v.Versions)
                        .Select(v => double.Parse(v.ToString()))
                        .ToImmutableList();

                    if (versionsMap.Any(v => v == CurrentVersion))
                        return true;

                    return versionsMap.Count == 0 && versionsNormal.Count == 0;
                });
                #endregion

                // تنظیمات احراز هویت با توکن
                #region Authentication of swagger UI for request

                #region Default => Bear JWT
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = $@"JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
                #endregion

                config.lstOpenApiSecurityScheme.ForEach(scheme =>
                {
                    option.AddSecurityDefinition(scheme.openApiSecurityScheme.Scheme, scheme.openApiSecurityScheme);
                    option.AddSecurityRequirement(scheme.openApiSecurityRequirement);
                });

                option.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                #endregion

                #region Header
                // Register the custom header operation filter
                config.HeaderStructure?.ApplySetting(option);
                #endregion

                #region Show Summery in swagger ui
                try
                {
                    var xmlCommentsfile = $"{builder.Environment.ApplicationName}.xml";
                    var xmlCommentsfilePath = Path.Combine(AppContext.BaseDirectory, xmlCommentsfile);

                    option.IncludeXmlComments(xmlCommentsfilePath);
                }
                catch { }
                #endregion
            });
            #endregion

            // builder.Services.AddHttpContextAcceCoreMatchr();

            return (builder, true);
        }

        private static bool IsProduction(AppSetting configApp, WebApplication? app = null)
        {
            if (app != null && app.Environment.IsProduction())
                return true;

            if (configApp.CoreSystemENV.environment is
                    ENV.SystemENV.EnvironmentType.Production or
                    ENV.SystemENV.EnvironmentType.ProductionIIS)
                return true;

            return false;
        }

        /// <summary>
        /// APP , isApplyed
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configApp"></param>
        /// <returns> APP , isApplyed </returns>
        public static (WebApplication, bool) SRT_AddSwaggerUI(this WebApplication app, AppSetting configApp)
        {
            if (configApp.SwaggerConfig is null ||
                (IsProduction(configApp, app) && !configApp.SwaggerConfig.SwaggerInProduction))
                return (app, false);

            // app.UseMiddleware<SwaggerBasePathMiddleware>();

            var config = configApp.SwaggerConfig;
            var SQLConnection = configApp.ConnectionStrings;

            #region Swagger rahimzadeh
            // Enable middle ware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(option =>
            {
                config.lstSwaggerAction.ForEach(q => q.Invoke(option));
            });

            // Enable middle ware to serve swagger-ui (HTML, JS, CSS, etc.)
            app.UseSwaggerUI(option =>
            {
                // Retrieve the base path from HttpContext
                // var basePath = app.Services.GetRequiredService<IHttpContextAcceCoreMatchr>().HttpContext?.Items["SwaggerBasePath"] as string;
                // if (basePath == null)
                // var basePath = configApp.CoreSystemENV.environment == ENV.SystemENV.EnvironmentType.Production
                //                            ? "http://localhost:8999/api/MainServer"
                //                            : "http://37.221.22.254:8400/api/MainServer";

                var basePath = "";

                option.DocumentTitle = config.swaggerTitle;

                config.lstVersions.ForEach(ver => option.SwaggerEndpoint($"{basePath}/swagger/{ver.Version}/swagger.json", GetVersionName(ver)));

                config.lstSwaggerUIAction.ForEach(q => q.Invoke(option));

                if (!config.RoutePrefix)
                    option.RoutePrefix = string.Empty;

                // Close All Documentation Accordion Panel
                option.DocExpansion(config.DocExpansion);

                //// custom style for swagger UI
                //option.InjectStylesheet("cssPath");

                // Store authentication for next lunch
                option.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
            });
            #endregion

            return (app, true);
        }

        #region Private
        private static string GetVersionName(OpenApiInfo ver)
        {
            if (!string.IsNullOrWhiteSpace(ver.Description))
                return ver.Description;

            if (!string.IsNullOrWhiteSpace(ver.Title))
                return ver.Title;

            return "Version" + ver.Version + " - API";
        }

        private static string? GetDescription(SRT.Core.Domain.GeneralConnectionString connection, [CallerArgumentExpression("connection")] string? instanceName = null)
        {
            if (connection == null) return null;

            var lst = new List<string>();

            lst.Add($"{instanceName}=> ");

            if (!string.IsNullOrWhiteSpace(connection.host))
                lst.Add($"host:{connection.host}");
            if (!string.IsNullOrWhiteSpace(connection.port))
                lst.Add($"port:{connection.port}");
            if (!string.IsNullOrWhiteSpace(connection.dbName))
                lst.Add($"dbName:{connection.dbName}");
            if (!string.IsNullOrWhiteSpace(connection.user))
                lst.Add($"user:{connection.user}");

            return string.Join(" - ", lst)
                         .Replace("=>  - ", " => ")
                         .Replace("SQLConnection.", "")
                      + "<br />";
        }
        #endregion
    }
}
