using LeaveManagement.Application.Authentication.Commands;
using LeaveManagement.WebUI.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.WebUI.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IMediator mediator, ILogger<AccountController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null) 
        {
            var model = new AccountViewModel
            {
                ReturnUrl = returnUrl ?? Url.Content("~/")
            };
            return View(model);
        }       


        [HttpPost]
        public async Task<IActionResult> Login(AccountViewModel model)
        {
            try
            {
                var command = new LoginCommand
                {
                    Username = model.Username,
                    Password = model.Password,
                    ReturnUrl = model.ReturnUrl
                };

                var token = await _mediator.Send(command);

                HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax
                });

                var returnUrl = command.ReturnUrl;

                // ✅ Đặt logger ở đây sau khi token được tạo thành công
                _logger.LogInformation("Đăng nhập thành công với user: {Username}, ReturnUrl = {ReturnUrl}", model.Username, returnUrl);                                

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    _logger.LogInformation("ReturnUrl = {ReturnUrl}", command.ReturnUrl);
                    Console.WriteLine("✅ Redirecting to: " + returnUrl);
                    return LocalRedirect(returnUrl);
                }               
                _logger.LogInformation("❌ Invalid or missing returnUrl");               

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                _logger.LogWarning("Đăng nhập thất bại với user: {Username}", model.Username);
                ModelState.AddModelError("", "Login failed");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Xoá cookie jwt
            Response.Cookies.Delete("jwt");
            // (Optional) Xoá thêm các thông tin session nếu có

            _logger.LogInformation("Đã logout user.");

            // Chuyển hướng về trang đăng nhập (hoặc Index tuỳ ý)
            return RedirectToAction("Login", "Account");
        }
    }
}
