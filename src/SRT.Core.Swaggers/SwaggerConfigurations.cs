// Ignore Spelling: yyyy mm dd rahimzadeh srt

using Microsoft.AspNetCore.Builder;
using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using Microsoft.Extensions.Hosting;
using System.Runtime.CompilerServices;
using SRT.Core.Domain;
using SRT.Core.Swaggers.Domain;
using SRT.Core.SRTExtensions;
using Microsoft.OpenApi;
using CoreEnv = SRT.Core.ENV;

namespace SRT.Core.Swaggers
{
    public static class SwaggerConfigurations
    {
        public static (WebApplicationBuilder, bool) SRT_AddSwaggerGen(
            this WebApplicationBuilder builder,
            AppSetting configApp,
            SwaggerDomain swaggerConfig)
        {
            if (IsProduction(configApp) && !swaggerConfig.SwaggerInProduction)
                return (builder, false);

            var config = swaggerConfig;
            var SQLConnection = configApp.ConnectionStrings;

            var v = "<h3><b>Environment: " + configApp.CoreSystemENV.environment.SRT_Enum_GetDescription() + "</b></h3><br />";
            v += GetDescription(SQLConnection.DefaultConnection);
            v += GetDescription(SQLConnection.SRTCore_CacheSQL);
            v += GetDescription(SQLConnection.SRTCore_CacheRedis);
            v += GetDescription(SQLConnection.SRTCore_ErrorSQL);
            v += GetDescription(SQLConnection.SRTCore_RabbitMQ);

            var Description = $@"<pre>
{v}
</pre>
";
            Description = Description.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Description = Description.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);
            Description = Description.Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine);

            builder.Services.AddSwaggerGen(option =>
            {
                config.lstVersions.ForEach(ver =>
                {
                    ver.Title = ver.Title + " - " + DateTime.Now.ToString("yyyy/MM/dd - HH:mm");
                    ver.Description += Description;
                    option.SwaggerDoc(ver.Version, ver);
                });

                try
                {
                    var xmlFile = $"{builder.Environment.ApplicationName}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    option.IncludeXmlComments(xmlPath);
                }
                catch (Exception)
                {
                }

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

                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = $@"JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                config.lstOpenApiSecurityScheme.ForEach(scheme =>
                {
                    option.AddSecurityDefinition(scheme.openApiSecurityScheme.Scheme, scheme.openApiSecurityScheme);
                });

                option.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                config.HeaderStructure?.ApplySetting(option);
            });

            return (builder, true);
        }

        private static bool IsProduction(AppSetting configApp, WebApplication? app = null)
        {
            if (app != null && app.Environment.IsProduction())
                return true;

            if (configApp.CoreSystemENV.environment is
                    CoreEnv.SystemENV.EnvironmentType.Production or
                    CoreEnv.SystemENV.EnvironmentType.ProductionIIS)
                return true;

            return false;
        }

        public static (WebApplication, bool) SRT_AddSwaggerUI(
            this WebApplication app,
            AppSetting configApp,
            SwaggerDomain swaggerConfig)
        {
            if (IsProduction(configApp, app) && !swaggerConfig.SwaggerInProduction)
                return (app, false);

            var config = swaggerConfig;

            app.UseSwagger(option =>
            {
                config.lstSwaggerAction.ForEach(q => q.Invoke(option));
            });

            app.UseSwaggerUI(option =>
            {
                var basePath = "";
                option.DocumentTitle = config.swaggerTitle;
                config.lstVersions.ForEach(ver => option.SwaggerEndpoint($"{basePath}/swagger/{ver.Version}/swagger.json", GetVersionName(ver)));
                config.lstSwaggerUIAction.ForEach(q => q.Invoke(option));

                if (!config.RoutePrefix)
                    option.RoutePrefix = string.Empty;

                option.DocExpansion(config.DocExpansion);
                option.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
            });

            return (app, true);
        }

        private static string GetVersionName(OpenApiInfo ver)
        {
            if (!string.IsNullOrWhiteSpace(ver.Description))
                return ver.Description;
            if (!string.IsNullOrWhiteSpace(ver.Title))
                return ver.Title;
            return "Version" + ver.Version + " - API";
        }

        private static string? GetDescription(GeneralConnectionString connection, [CallerArgumentExpression("connection")] string? instanceName = null)
        {
            if (connection == null) return null;

            var lst = new List<string> { $"{instanceName}=> " };
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
    }
}
