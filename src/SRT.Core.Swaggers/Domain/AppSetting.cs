// Ignore Spelling: Ltag env

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SRT.Core.Swaggers.Domain;

namespace SRT.Core.Swaggers.Domain
{
    public class AppSetting : SRT.Core.Domain.AppSetting
    {
        public AppSetting(List<string>? lstDatabaseNamespace = null) : base(lstDatabaseNamespace)
        { }

        public AppSetting(IWebHostEnvironment _env, AppSetting _base) : base(_env, _base)
        {
            SwaggerConfig = _base.SwaggerConfig;
        }

        public string APPName => SwaggerConfig.swaggerTitle;

        public SwaggerDomain SwaggerConfig { get; set; } = null!;
    }
}
