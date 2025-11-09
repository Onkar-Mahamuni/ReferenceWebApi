using Microsoft.AspNetCore.Mvc;

namespace ReferenceWebApi.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
    }
}
