using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodRecipes.Models;
using FoodRecipes.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FoodRecipes.Controllers
{
    [Route("api/log-in")]
    [ApiController]
    public class LogInController : ControllerBase
    {
        private UserService userService;

        private IConfiguration conf;

        public LogInController(IConfiguration con)
        {
            this.conf = con;
            userService = new UserService(con);
        }

        [HttpPost]
        public ActionResult<string> doLogIn(UserModel user)
        {
            if(userService.CheckUserCredentials(user.user_name, user.password))
            {
                string token = userService.generateJWebToken(user);
                userService.AddSession(userService.getUserId(user.user_name), token, DateTime.Now.AddMinutes(120));
                return Ok(token);
            }
            return BadRequest("UserName/Password not correct");
        }

    
      

    }

   
}
