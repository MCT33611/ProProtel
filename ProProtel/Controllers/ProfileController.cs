using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProPortel.Models;
using ProPortel.Models.VModels;
using ProPortel.Repositories.IRepositories;
using ProPortel.Utility;
using ProPortel.Utility.ISevices;

namespace ProPortel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = SD.Role_Customer)]
    public class ProfileController(
        UserManager<ApplicationUser> userManager,
        ICloudinaryService cloudinaryService,
        IUnitOfWork unitOfWork
            ) : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ICloudinaryService _cloudinaryService = cloudinaryService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID '{id}' not found.");
                }

                var userProfile = new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,

                    user.StreetAdderss,
                    user.City,
                    user.State,
                    user.PostalCode,

                    user.Bio,
                    user.ProfilePicture,
                    user.EmailConfirmed
                };

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while fetching user profile: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserProfileUpdateModel model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID '{id}' not found.");
                }

                user.FirstName = model.FirstName ?? user.FirstName;
                user.LastName = model.LastName ?? user.LastName; ;
                user.StreetAdderss = model.StreetAdderss ?? user.StreetAdderss; ;
                user.City = model.City ?? user.City; ;
                user.State = model.State ?? user.State; ;
                user.PostalCode = model.PostalCode ?? user.PostalCode; ;
                user.Bio = model.Bio ?? user.Bio; ;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return BadRequest($"Failed to update user profile: {errors}");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating user profile: {ex.Message}");
            }
        }

        [HttpPut("picture/{id}")]
        public async Task<IActionResult> UploadProfilePicture(string id, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                var maxFileSizeInBytes = 10 * 1024 * 1024; // 10 MB
                if (file.Length > maxFileSizeInBytes)
                {
                    return BadRequest("File size exceeds the maximum allowed limit.");
                }

                var imageUrl = await _cloudinaryService.UploadImageAsync(file, "profile_pictures");

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound($"User with ID '{id}' not found.");
                }

                user.ProfilePicture = imageUrl;
                await _userManager.UpdateAsync(user);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while uploading profile picture: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("user not found");
            await _userManager.DeleteAsync(user);
            return Ok();
        }


        [HttpGet("plans")]
        public IActionResult Plans()
        {
            return Ok(new {plans =  _unitOfWork.plan.GetALL() });
        }
    }
}
