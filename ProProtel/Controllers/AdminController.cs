using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using ProPortel.Models;
using ProPortel.Repositories.IRepositories;
using ProPortel.Utility;
using System.ComponentModel.DataAnnotations;

namespace ProPortel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(
        IUnitOfWork _unitOfWork,
        UserManager<ApplicationUser> _userManager,
        SignInManager<ApplicationUser> _signInManager,
        IJWTService _jwtService
        ) : ControllerBase
    {

        [HttpPost("Login")]
        public async Task<IActionResult> Login(ILoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return Unauthorized("Invalid email or password");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    return Unauthorized("Invalid email or password");
                }


                return Ok(new { token = _jwtService.GenerateTokenForAdmin(user) });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }

        }

    }
    public interface ILoginModel
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
