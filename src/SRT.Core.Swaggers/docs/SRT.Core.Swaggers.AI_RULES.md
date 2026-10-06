# SRT.Core.Swaggers — AI Rules (canonical)

MUST:

- Implement `IHasSwaggerConfig` on `MyAppSetting` when using Swagger.
- Project-reference this package when `SwaggerConfig` is non-null.
- Keep Swagger DI/UI in the plugin, not `Program.cs`.

MUST NOT:

- Set `SwaggerConfig` without shipping `SRT.Core.Swaggers` (Core throws).
- Call `UseAuthorization` / `MapControllers` / Swagger middleware in `Program.cs`.
- Assume `openApiSecurityRequirement` is applied (the type field is unused in `SwaggerConfigurations`).

Working rule: read docs first; update them if code differs. Keep `SRT.Core.Swaggers.` prefixes.
