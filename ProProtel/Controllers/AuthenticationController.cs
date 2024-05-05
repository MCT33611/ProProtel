using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProPortel.Models;
using ProPortel.Repositories.IRepositories;
using ProPortel.Utility.ISevices;
using ProPortel.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using ProPortel.Models.VModels;
using Google.Apis.Auth;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace ProPortel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        IJWTService jwsService,
        IEmailSender emailSender,
        IOTPService otp,
        IConfiguration config
            ) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IJWTService _jwtService = jwsService;
        private readonly IOTPService _otp = otp;
        private readonly IConfiguration _config = config;

        [HttpPost("Register")]
        public async Task<IActionResult> Registration(RegisterViewModel model)
        {
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Worker)).GetAwaiter().GetResult();
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                if (_unitOfWork.user.Get(u => u.Email == model.Email) != null)
                {
                    return Conflict($"Email '{model.Email}' already exists in the database.");
                }

                ApplicationUser? user = new()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email,

                };
                var result = await _userManager.CreateAsync(user, model.Password!);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return BadRequest($"Failed to create user: {errors}");
                }

                user = await _userManager.FindByEmailAsync(model.Email!);
                if (user == null)
                {
                    return BadRequest("Failed to create user");
                }

                result = await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                if (!result.Succeeded)
                {
                    return BadRequest("Failed to assign role to user");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPut("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(OtpModel model)
        {
            try
            {



                if (await _otp.ValidateOTPAsync(model.Email!, model.OTP!))
                {
                    ApplicationUser? user = await _userManager.FindByEmailAsync(model.Email!);
                    if (user != null)
                    {
                        user.EmailConfirmed = true;
                        _unitOfWork.user.Update(user);
                        await _unitOfWork.Save();
                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return BadRequest("Invalid OTP.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
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



                if (user.IsBlocked)
                {
                    return Unauthorized("User is Blocked for some reason");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    return Unauthorized("Invalid email or password");
                }


                return Ok(new { token = _jwtService.GenerateJwtToken(user) });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }

        }

        [HttpPost("LoginWithGoogle")]
        public async Task<IActionResult> LoginWithGoogle( string credential)
        {
            

            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Worker)).GetAwaiter().GetResult();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = [_config["GoogleSettings:GoogleClientId"]!]
                };
                Payload payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);

                ApplicationUser user = new()
                {
                    Email = payload.Email,
                    FirstName = payload.Name,
                    UserName = payload.Email,
                    ProfilePicture = payload.Picture,
                    EmailConfirmed = true,
                    Role = SD.Role_Customer,
                };
                ApplicationUser getUser = _unitOfWork.user.Get(u => u.Email == user.Email);
                if (getUser != null)
                {
                    await _userManager.UpdateAsync(user);
                    return Ok(new { token = _jwtService.GenerateJwtToken(getUser) });
                }

                var result = await _userManager.CreateAsync(user, _config["GoogleSettings:Secret"]!);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return BadRequest($"Failed to create user: {errors}");
                }
                if (user.IsBlocked)
                {
                    return Unauthorized("User is Blocked for some reason");
                }
                result = await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                if (!result.Succeeded)
                {
                    return BadRequest("Failed to assign role to user");
                }
                return Ok(new { token = _jwtService.GenerateJwtToken(_unitOfWork.user.Get(u => u.Email == user.Email)) });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }

        }

        [HttpPost("ForgotPassword/{Email}")]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return NotFound("No user with the provided email address was found.");
            }

            if (!(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return BadRequest("The provided email address has not been confirmed. Please verify your email address.");
            }

            return Ok(new {restToken = await _userManager.GeneratePasswordResetTokenAsync(user) });
        }

        [HttpPut("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (model== null)
            {
                return BadRequest("Email, token, and new password are required for password reset.");
            }

            var user = await _userManager.FindByEmailAsync(model.Email!);
            if (user == null)
            {
                return NotFound($"User with email '{model.Email}' not found.");
            }



            var result = await _userManager.ResetPasswordAsync(user, model.Token!, model.Password!);
            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Failed to reset password.");
            }
        }

        [HttpPost("SentOtp/{Email}")]
        public async Task<IActionResult> SentOtp(string Email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                var otpCode = await _otp.GenerateOTPAsync(Email);
                await _emailSender.SendEmailAsync(user!.Email!, "Confirm your email", $"Your Otp is <a>{otpCode}</a>.");
                return Ok();
            }
            else
            {
                return NotFound($"user not found by {Email}");
            }
        }
    }
}
