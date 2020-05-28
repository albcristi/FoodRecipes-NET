using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodRecipes.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FoodRecipes.Controllers
{
    [Route("api/log-out")]
    [ApiController]
    public class LogOutController : ControllerBase
    {
        private UserService userService;

        private IConfiguration configuration;
        public LogOutController(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.userService = new UserService(configuration);
        }

        [HttpPost("{user_name}")]
        public ActionResult<Boolean> logOut(String user_name)
        {
            Int32 user_id = userService.getUserId(user_name);
            Boolean res = userService.RemoveSession(user_id);
            if (res)
            {
                return Ok(true);
            }
            return BadRequest(false);

        }
    }
}
