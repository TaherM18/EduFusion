using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Repositories.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminApiController : ControllerBase
    {

        private readonly IUserInterface _userRepo;
        #region Get
        // GET: api/student/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetAdmin(int id)
        {
            System.Console.WriteLine(id);
            var admin = await _userRepo.GetOne(id);
            if (admin == null)
            {
                return NotFound(new { message = "Admin not found" });
            }
            return Ok(admin);
        }
        #endregion



    }
}