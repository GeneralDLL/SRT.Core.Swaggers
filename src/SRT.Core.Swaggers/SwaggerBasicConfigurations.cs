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
                return Task.FromResult(app);

            app.SRT_AddSwaggerUI(configApp, swaggerApp.SwaggerConfig);
            return Task.FromResult(app);
        }
    }
}
