using Microsoft.AspNetCore.Builder;
using SRT.Core.Domain;
using SRT.Core.Interfaces;

namespace SRT.Core.Swaggers
{
    public class SwaggerBasicConfigurations : ISRTBasicConfigurations
    {
        public int Order => 50;
        public string FeatureName => SRTFeatureNames.Swagger;

        public Task<WebApplicationBuilder> SRT_ConfigBuilder(WebApplicationBuilder builder, AppSetting configApp)
        {
            if (configApp is not IHasSwaggerConfig swaggerApp || swaggerApp.SwaggerConfig is null)
                return Task.FromResult(builder);

            builder.SRT_AddSwaggerGen(configApp, swaggerApp.SwaggerConfig);
            return Task.FromResult(builder);
        }

        public Task<WebApplication> SRT_ConfigApp(WebApplication app, AppSetting configApp)
        {
            if (configApp is not IHasSwaggerConfig swaggerApp || swaggerApp.SwaggerConfig is null)
            {
                configApp.StartupReport.Off("swagger", "no SwaggerConfig");
                return Task.FromResult(app);
            }

            var config = swaggerApp.SwaggerConfig;
            var (_, enabled) = app.SRT_AddSwaggerUI(configApp, config);
            var versions = string.Join(",", config.lstVersions.Select(v => v.Version));
            if (enabled)
                configApp.StartupReport.Ok("swagger", $"ui={(config.RoutePrefix ? "/swagger" : "/")} versions=[{versions}] inProduction={config.SwaggerInProduction}");
            else
                configApp.StartupReport.Off("swagger", $"disabled in {configApp.CoreSystemENV.environment} (SwaggerInProduction=false)");
            return Task.FromResult(app);
        }
    }
}
