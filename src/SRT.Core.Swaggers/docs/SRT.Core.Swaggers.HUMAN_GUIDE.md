# SRT.Core.Swaggers — Human Guide

Swagger/OpenAPI plugin (`SwaggerBasicConfigurations`, Order **50**, feature `"Swagger"`).

The host `AppSetting` subclass must implement `IHasSwaggerConfig` with a **non-null** `SwaggerConfig`. Core detects that property by name; if set, the Swaggers DLL must be in the output.

Do not call `AddSwaggerGen` / `UseSwagger` in `Program.cs`. Production (ASP.NET `IsProduction` or Core `Production` / `ProductionIIS`) skips Swagger unless `SwaggerInProduction` is true.

See [API_REFERENCE](SRT.Core.Swaggers.API_REFERENCE.md) and [COOKBOOK](SRT.Core.Swaggers.COOKBOOK.md).
