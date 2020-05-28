using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FoodRecipes.Models;
using FoodRecipes.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Ocsp;

namespace FoodRecipes.Controllers
{
    [Route("api/recipe")]
    [ApiController]
    public class RecipeController : ControllerBase
    {

        private RecipeService service = new RecipeService();

        private UserService userService;

        private IConfiguration configuration;
        public RecipeController(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.userService = new UserService(this.configuration);

        }

        [HttpGet]
        public ActionResult<List<RecipeModel>> GetAll()
        {
            string tok = (string) Request.Headers["Authorization"];
          
            UserModel usr = userService.GetUserBySessionToken(tok);
            if (usr.id == 0)
                return BadRequest(new List<RecipeModel>());
            if (!userService.SessionIsValid(tok, DateTime.Now))
            {
                return BadRequest(new List<RecipeModel>());
            }
            userService.UpdateSessionTtl(usr.id, DateTime.Now.AddMinutes(30));


            try
            {
                return Ok(service.getAll());
            }
            catch (Exception)
            {
                return Ok(new List<RecipeModel>());
            }
           
        }

        [HttpGet("{rec_type}")]
        public ActionResult<List<RecipeModel>> getAllOfType(String rec_type)
        {

            string tok = (string)Request.Headers["Authorization"];
            UserModel usr = userService.GetUserBySessionToken(tok);
            if (usr.id == 0)
                return BadRequest(new List<RecipeModel>());
            if (!userService.SessionIsValid(tok, DateTime.Now))
            {
                return BadRequest(new List<RecipeModel>());
            }
            userService.UpdateSessionTtl(usr.id, DateTime.Now.AddMinutes(30));

            try
            {
                return  Ok(service.getAllOfType(rec_type));
            }
            catch(Exception)
            {
                return Ok(new List<RecipeModel>());
            }
        }

        [HttpDelete("{rec_id}")]
        public ActionResult<Boolean> deleteRecipe(Int32 rec_id)
        {
            string tok = (string)Request.Headers["Authorization"];
            UserModel usr = userService.GetUserBySessionToken(tok);
            if (usr.id == 0)
                return BadRequest(false);
            if (!userService.SessionIsValid(tok, DateTime.Now))
            {
                return BadRequest(false);
            }
            userService.UpdateSessionTtl(usr.id, DateTime.Now.AddMinutes(30));

            Boolean res = service.RemoveRecipe(rec_id);
            return Ok(true);
        }

        [HttpPut]
        public ActionResult<Boolean> addRec(RecipeModelDto rec)
        {

            string tok = (string)Request.Headers["Authorization"];
            UserModel usr = userService.GetUserBySessionToken(tok);
            if (usr.id == 0)
                return BadRequest(false);
            if (!userService.SessionIsValid(tok, DateTime.Now))
            {
                return BadRequest(false);
            }
            userService.UpdateSessionTtl(usr.id, DateTime.Now.AddMinutes(30));

            return Ok(service.AddRecipe(rec.name, rec.description, rec.steps, rec.typeRec, usr.id));
        }

        [HttpPut("{rec_id}")]
        public ActionResult<Boolean> updateRec(Int32 rec_id, RecipeModelDto rec)
        {

            string tok = (string)Request.Headers["Authorization"];
            UserModel usr = userService.GetUserBySessionToken(tok);
            if (usr.id == 0)
                return BadRequest(false);
            if (!userService.SessionIsValid(tok, DateTime.Now))
            {
                return BadRequest(false);
            }
            userService.UpdateSessionTtl(usr.id, DateTime.Now.AddMinutes(30));
            return Ok(service.UpdateRecipe(rec.id,usr.id,rec.description, rec.steps, rec.typeRec));
        }

        [HttpGet("rec-{rec_id}")]
        public ActionResult<RecipeModelDto> getRecipe(Int32 rec_id)
        {
            string tok = (string)Request.Headers["Authorization"];
            UserModel usr = userService.GetUserBySessionToken(tok);
            if (usr.id == 0)
                return BadRequest(new RecipeModelDto());
            if (!userService.SessionIsValid(tok, DateTime.Now))
            {
                return BadRequest(new RecipeModelDto());
            }
            userService.UpdateSessionTtl(usr.id, DateTime.Now.AddMinutes(30));
            return Ok(service.getRecipe(rec_id));
        }
    }

}
