# SRT.Core.Swaggers — Cookbook

```xml
<ProjectReference Include="..\path\SRT.Core.Swaggers\src\SRT.Core.Swaggers\SRT.Core.Swaggers.csproj" />
```

```csharp
using Microsoft.OpenApi.Models;
using SRT.Core.Domain;
using SRT.Core.Swaggers;
using SRT.Core.Swaggers.Domain;

public class MyAppSetting : AppSetting, IHasSwaggerConfig
{
    public SwaggerDomain? SwaggerConfig { get; set; }
}

// In BasicProjectConfig.SRT_ConfigController:
return new MyAppSetting
{
    SwaggerConfig = new SwaggerDomain
    {
        swaggerTitle = "My API",
        RoutePrefix = true,
        SwaggerInProduction = false,
        lstVersions = { new OpenApiInfo { Version = "v1", Title = "My API" } },
    },
};
```

Leave `SwaggerConfig` null (and do not implement a non-null property) if you do not want the Swagger feature requested.
