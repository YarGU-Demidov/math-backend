using System.Collections.Generic;
using MathSite.Api.Common.Attributes;
using MathSite.Api.Server.VersionsAttributes;
using Microsoft.AspNetCore.Mvc;

namespace MathSite.Api.Server.Controllers
{
    [V1]
    [ApiController]
    [DefaultApiRoute]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new[] {"value1", "value2"};
        }
    }
}