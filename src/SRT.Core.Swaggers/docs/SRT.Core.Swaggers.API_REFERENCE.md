# SRT.Core.Swaggers — API Reference

Reviewed: Swaggers repo `d47383c`, Core repo `95b0a5b`.

---

## `IHasSwaggerConfig`

```csharp
public interface IHasSwaggerConfig
{
    SwaggerDomain? SwaggerConfig { get; set; }
}
```

Core `HasSwaggerConfig` is reflection on property name `"SwaggerConfig"` with a non-null value.

---

## `SwaggerBasicConfigurations`

- `Order => 50`, `FeatureName => SRTFeatureNames.Swagger`
- Builder/App: if `configApp is IHasSwaggerConfig` and `SwaggerConfig != null` → `SRT_AddSwaggerGen` / `SRT_AddSwaggerUI`

---

## `SwaggerDomain` (`SRT.Core.Swaggers.Domain`)

| Member | Default |
|--------|---------|
| `lstVersions` | empty `List<OpenApiInfo>` |
| `lstOpenApiSecurityScheme` | empty |
| `HeaderStructure` | null |
| `RoutePrefix` | `true` (false → UI `RoutePrefix = ""`) |
| `SwaggerInProduction` | `false` |
| `swaggerTitle` | `"Swagger Title"` |
| `DocExpansion` | `DocExpansion.None` |
| `lstSwaggerUIAction` / `lstSwaggerAction` | empty |

`OpenApiSecurity`: required `openApiSecurityScheme` and `openApiSecurityRequirement` (the requirement object is **not** used in `SwaggerConfigurations`). Extra schemes keyed by `scheme.openApiSecurityScheme.Scheme`.

`SwaggerInterface.ApplySetting(SwaggerGenOptions)` optional header/filter hook.

---

## `SwaggerConfigurations`

```csharp
public static (WebApplicationBuilder, bool) SRT_AddSwaggerGen(this WebApplicationBuilder builder, AppSetting configApp, SwaggerDomain swaggerConfig)
public static (WebApplication, bool) SRT_AddSwaggerUI(this WebApplication app, AppSetting configApp, SwaggerDomain swaggerConfig)
```

Returns `false` and skips when production **and** `!SwaggerInProduction`. Production = `IsProduction()` **or** `CoreSystemENV.environment` is `Production` / `ProductionIIS`.

Gen: Bearer ApiKey header `Authorization`; XML `{ApplicationName}.xml` (errors swallowed); version filter on `ApiVersionAttribute` / `MapToApiVersionAttribute`; unmatched included if no version attributes; `ResolveConflictingActions` → `First()`. Mutates each `OpenApiInfo.Title` with timestamp and appends HTML of connection hosts (no passwords).

UI: `DocumentTitle = swaggerTitle`; endpoints `/swagger/{ver.Version}/swagger.json`; `persistAuthorization=true`.
