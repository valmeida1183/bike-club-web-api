using System.Threading.Tasks;
using BikeClub.Models;
using Microsoft.AspNetCore.Mvc;

namespace BikeClub.Controllers
{
    [Route("v1/roles")]
    public class RoleController : ControllerBase
    {
        [HttpPost]
        public ActionResult<Role> Post ([FromBody] Role model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(model);
        }
    }
}