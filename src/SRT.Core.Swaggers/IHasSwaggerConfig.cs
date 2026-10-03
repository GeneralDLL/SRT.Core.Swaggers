using SRT.Core.Swaggers.Domain;

namespace SRT.Core.Swaggers
{
    public interface IHasSwaggerConfig
    {
        SwaggerDomain? SwaggerConfig { get; set; }
    }
}
