using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using ProPortel.Models;
using ProPortel.Models.VModels;
using ProPortel.Repositories.IRepositories;
using ProPortel.Utility;
using System.ComponentModel.DataAnnotations;

namespace ProPortel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles =SD.Role_Admin)]
    public class AdminController(
        IUnitOfWork _unitOfWork,
        UserManager<ApplicationUser> _userManager,
        SignInManager<ApplicationUser> _signInManager,
        IJWTService _jwtService
        ) : ControllerBase
    {
        /*Admin Login*/
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

        /*Block or Unblock for all users exept admin*/
        [HttpPut("User/{id}")]
        public async Task<IActionResult> AccessChange(string id, bool Access)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null || user.Role == SD.Role_Admin)
                {
                    return NotFound($"User with ID '{id}' not found.");
                }
                user.IsBlocked = Access;
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


        /*Customer mangement*/
        [HttpGet("Customer/GetAll")]
        public IActionResult GetAllCustomers()
        {
            IList<ApplicationUser> users =  _unitOfWork.user.GetALL((u)=>u.Role == SD.Role_Customer).ToList();
            return Ok(new { users });
        }

        [HttpGet("Customer/{id}")]
        public IActionResult GetCustomer(string Id)
        {
            ApplicationUser user = _unitOfWork.user.Get(u=>u.Id == Id);
            return Ok(new { user });
        }


        /*Worker mangement*/
        [HttpGet("Worker/GetAll")]
        public IActionResult GetAllWorkers()
        {
            IList<ApplicationUser> users = _unitOfWork.user.GetALL((u) => u.Role == SD.Role_Customer).ToList();
            return Ok(new { users });
        }

        [HttpGet("Worker/{id}")]
        public IActionResult GetWorkers(string Id)
        {
            ApplicationUser user = _unitOfWork.user.Get(u => u.Id == Id);
            return Ok(new { user });
        }

        [HttpPut("Worker/Approvel/{id}")]
        public async Task<IActionResult> Approvel(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null || user.Role == SD.Role_Worker)
                {
                    return NotFound($"User with ID '{id}' not found.");
                }
                user.IsAproved = true;
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


        /*Plan Management*/
        [HttpGet("Plan/GetAll")]
        public IActionResult GetAllPlans()
        {
            IList<Plan> plans = [.._unitOfWork.plan.GetALL()];
            return Ok(new { plans });
        }

        [HttpGet("Plan/{id}")]
        public IActionResult GetPlan(int id)
        {
            Plan plan = _unitOfWork.plan.Get((p)=>p.Id == id);
            return Ok(new { plan});
        }

        [HttpPost("Plan")]
        public IActionResult CreatePlan(Plan plan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _unitOfWork.plan.Add(plan);
            _unitOfWork.Save();
            return Ok();
        }

        [HttpPut("Plan")]
        public IActionResult UpdatePlan(Plan plan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _unitOfWork.plan.Update(plan);
            _unitOfWork.Save();
            return Ok();
        }
        [HttpDelete("Plan/{Id}")]
        public IActionResult DeletePlan(int id)
        {
            Plan plan = _unitOfWork.plan.Get(p=>p.Id == id);
            if(plan == null)
            {
                return BadRequest(ModelState);
            }
            _unitOfWork.plan.Remove(plan);
            _unitOfWork.Save();
            return Ok();
        }


        /*Subscription Management*/
        [HttpGet("Subscription/GetAll")]
        public IActionResult GetAllSubscription()
        {
            IList<Subscription> plans = [.. _unitOfWork.subscription.GetALL()];
            return Ok(new { plans });
        }

        [HttpGet("Subscription/{id}")]
        public IActionResult GetSubscription(int id)
        {
            Plan plan = _unitOfWork.plan.Get((p) => p.Id == id);
            return Ok(new { plan });
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
