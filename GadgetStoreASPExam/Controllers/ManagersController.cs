using GadgetStoreASPExam.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using GadgetStoreASPExam.Roles;

namespace GadgetStoreASPExam.Controllers
{
    [ApiController, Authorize]
    [Route("api/[controller]")]
    public class ManagersController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public ManagersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        [Route("delManager")]
        public async Task<IActionResult> DelManager([FromBody] Register model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return NotFound($"User with username {model.UserName} not found");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok($"User with username {model.UserName} deleted successfully");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while deleting user with username {model.UserName}");
            }
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Admin)]
        [Route("selectManager")]
        public async Task<IResult> Get()
        {
            return Results.Json(_userManager.Users);
        }
    }
}
