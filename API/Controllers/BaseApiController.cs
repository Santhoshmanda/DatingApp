using API.Helpers;
using Microsoft.AspNetCore.Mvc;
namespace API.Controllers
{   [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]// is responsible for binding form body to dto and also for validaitons. if this is absent then we need to keep [FromBody]
    [Route("api/[controller]")]

    public class BaseApiController : ControllerBase
    {
        
    }
}