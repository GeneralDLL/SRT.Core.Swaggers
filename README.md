# SRT.Core.Swaggers

Swagger/OpenAPI plugin for SRT hosts (Swashbuckle + API versioning).

**Target:** .NET 10

Package docs: **[src/SRT.Core.Swaggers/docs/SRT.Core.Swaggers.README.md](src/SRT.Core.Swaggers/docs/SRT.Core.Swaggers.README.md)**

## Consume

```xml
<ProjectReference Include="..\SRT.Core.Swaggers\src\SRT.Core.Swaggers\SRT.Core.Swaggers.csproj" />
```

`MyAppSetting` must implement `IHasSwaggerConfig`. A **non-null** `SwaggerConfig` makes Core require this plugin. Do not register Swagger in `Program.cs`.

## Quick start

```csharp
public class MyAppSetting : AppSetting, IHasSwaggerConfig
{
    public SwaggerDomain? SwaggerConfig { get; set; }
}
```

Recipes: [COOKBOOK](src/SRT.Core.Swaggers/docs/SRT.Core.Swaggers.COOKBOOK.md).
