using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRT.Core.Swaggers.Domain
{
    public interface SwaggerInterface
    {
        public void ApplySetting(SwaggerGenOptions options);
    }
}
