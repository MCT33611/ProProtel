using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProPortel.Models;
using ProPortel.Models.VModels;
using ProPortel.Repositories.IRepositories;
using ProPortel.Utility;
using Razorpay.Api;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProPortel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController(IUnitOfWork unitOfWork,UserManager<ApplicationUser> _userManager, IConfiguration confing, IJWTService _jwtService) : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IConfiguration _confing = confing;

        [HttpGet("Payment/Init")]
        public IActionResult InitSubscribe(string UserId,int PlanId)
        {
            ApplicationUser user = _unitOfWork.user.Get(p => p.Id == UserId);
            if (user == null)
            {
                return BadRequest(ModelState);
            }
            Models.Plan plan = _unitOfWork.plan.Get(p => p.Id == PlanId);
            if(plan == null) { 
                return BadRequest(ModelState);
            }
            RazorpayClient razorpayClient = new(_confing["RazorpaySettings:key_id"], _confing["RazorpaySettings:key_secret"]);
            Dictionary<string, object> options = new()
            {
                { "amount", plan.Price * 100 },
                { "currency", "INR" },
                { "receipt", "order_rcptid_11_" },
                { "payment_capture", 1 }
            };

            Order order = razorpayClient.Order.Create(options);
            return Ok(new { orderId = order["id"].ToString() });


        }

        [HttpGet("Payment/Plan")]
        public IActionResult Plan( int PlanId)
        {
            Models.Plan plan = _unitOfWork.plan.Get(p => p.Id == PlanId);
            if (plan == null)
            {
                return BadRequest(ModelState);
            }

            return Ok(new {plan});

        }

        [HttpPost("Subscribe")]
        public IActionResult Subscribe(SubscriptionViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RazorpayClient razorpayClient = new(_confing["RazorpaySettings:key_id"], _confing["RazorpaySettings:key_secret"]);
                    Payment payment = razorpayClient.Payment.Fetch(model.PaymentId);
                    if (payment["status"].ToString() != "captured")
                    {
                        return BadRequest("Payment verification failed.");
                    }

                    Models.Plan plan = _unitOfWork.plan.Get((p)=>p.Id == model.PlanId);
                    if(plan == null)
                        return BadRequest(ModelState);
                    Models.Subscription subscription = new()
                    {
                        PaymentId = model.PaymentId,
                        Status = model.Status == "captured" ? SD.Payment_Success : SD.Payment_Failed,
                        WorkerId = model.WorkerId,
                        PlanId = model.PlanId,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddMonths(plan.DurationMonths),
                    };
                    _unitOfWork.subscription.Add(subscription);
                    _unitOfWork.user.ChangeUserRole(model.WorkerId!,SD.Role_Worker);
                    _unitOfWork.Save();
                    ApplicationUser? user = _userManager.FindByIdAsync(model.WorkerId!).GetAwaiter().GetResult();
                    if (user == null)
                    {
                        return BadRequest("Failed to create user");
                    }
                    return Ok(new { token = _jwtService.GenerateJwtToken(user) });
                }
                return BadRequest(ModelState);
                       
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            
        }

    }
}
