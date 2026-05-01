using ECommerce527.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;
using System.Runtime.InteropServices;

namespace ECommerce527.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<ApplicationUserOtp> _applicationUserOtpRepository;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, IRepository<ApplicationUserOtp> applicationUserOtpRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _applicationUserOtpRepository = applicationUserOtpRepository;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) { 
                return View(registerVM);
            }
            ApplicationUser user = new ApplicationUser()
            {
                Name = registerVM.Name , 
                Address = registerVM.Address ,
                Email = registerVM.Email ,
                UserName = registerVM.UserName ,
            };
            var result = await _userManager.CreateAsync(user , registerVM.Password);
            if (!result.Succeeded)
            {
                foreach( var error in result.Errors)
                {
                    ModelState.AddModelError( "", error.Description); 
                }
                return View(registerVM);
            }
            TempData["Success-Notification"] = "user Registered Successfully ";
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user); 
            var link = Url.Action(nameof(ConfirmEmail) , "Account" , new { area = "Identity" , userId  = user.Id , token } , Request.Scheme ); 
            await _emailSender.SendEmailAsync(
                registerVM.Email ,
                "Ecommerce527 Confirm Email" , 
                $"<h1>click <a href={link}> here </a> to confirm Your Email </h1>"); 
            return RedirectToAction(nameof(Login));
        }
        public async Task<IActionResult> ConfirmEmail(string userId , string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                TempData["Error-Notification"] = "invalid user";
                return RedirectToAction(nameof(Login));
            }
            var result =  await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                TempData["Error-Notification"] = "Cant confirm Email";
                return RedirectToAction(nameof(Login));
            }
            TempData["Success-Notification"] = "Confirmed Email Successfully ";
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public async Task<IActionResult> ResendEmailConfirmation()
        {
            return View(); 
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationVM.UserNameOrEmail)
               ?? await _userManager.FindByNameAsync(resendEmailConfirmationVM.UserNameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError("", "invalid userName Or Password");
                return View(resendEmailConfirmationVM);
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = "Identity", userId = user.Id, token }, Request.Scheme);
            await _emailSender.SendEmailAsync(
                user.Email,
                "Ecommerce527 Confirm Email",
                $"<h1>click <a href={link}> here </a> to confirm Your Email </h1>");
            TempData["Success-Notification"] = "Resend Email ConfirmationSuccessfully ";
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            var user = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail)
                ?? await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);

            if(user is null)
            {
                ModelState.AddModelError("", "invalid userName Or Password"); 
                return View(loginVM); 
            }
            var result = await _signInManager.PasswordSignInAsync(user , loginVM.Password , loginVM.RememberMe , true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "to many attempts please try again Later");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "please Confirm Your Email First");
                }
                else
                {
                    ModelState.AddModelError("", "invalid userName Or Password");
                }
                return View(loginVM);
            }
            TempData["Success-Notification"] = "Login Successfully ";
            return RedirectToAction("Index" , "Home" , new { area = "Customer"});
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordVM forgetPasswordVM)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordVM.UserNameOrEmail)
             ?? await _userManager.FindByNameAsync(forgetPasswordVM.UserNameOrEmail);

            if (user is null)
            {
                ModelState.AddModelError("", "invalid userName Or Password");
                return View(forgetPasswordVM);
            }
            var applicationUserotps = await _applicationUserOtpRepository.GetAsync(e=>e.ApplicationUserId == user.Id);
            var count = applicationUserotps.Count(e=>(DateTime.UtcNow -  e.CreatedAt).TotalHours <= 24 );
            if(count >=5)
            {
                ModelState.AddModelError("", "To many Attempts please try again Later"); 
                return View(forgetPasswordVM); 
            }
            var otp = new Random().Next(1000 , 9999).ToString();
            var applicationUserOtp = new ApplicationUserOtp(user.Id , otp);
            await _applicationUserOtpRepository.AddAsync(applicationUserOtp);
            await _applicationUserOtpRepository.CommitAsync(); 
            await _emailSender.SendEmailAsync(
               user.Email,
               "Ecommerce527 Forget Password",
               $"<h1> use this otp <span style=\"color: red\">{otp}</span> to Reset your password</h1>");
            return RedirectToAction(nameof(ValidateOTP) , new { userId  = user.Id});
        }
        [HttpGet]
        public IActionResult ValidateOTP(string userId )
        {
            return View( new ValidateOTPVM() { UserId = userId });
        }
        [HttpPost]
        public async Task<IActionResult> ValidateOTP(ValidateOTPVM validateOTPVM)
        {
            if (!ModelState.IsValid)
            {
                return View(validateOTPVM); 
            }
            var user = await _userManager.FindByIdAsync(validateOTPVM.UserId); 

            if (user is null)
            {
                ModelState.AddModelError("", "invalid user ");
                return View(validateOTPVM);
            }

            var otps = await _applicationUserOtpRepository.GetAsync(e=>
                e.ApplicationUserId == user.Id && 
                e.IsValid == true && 
                e.ValidTo >= DateTime.UtcNow
            );
            var otp = otps.OrderByDescending(e => e.CreatedAt).FirstOrDefault();
            if(otp is null || otp.OTP != validateOTPVM.OTP)
            {
                ModelState.AddModelError("", "invalid / Expired OTP ");
                return View(validateOTPVM); 
            }
            otp.IsValid = false;
            await _applicationUserOtpRepository.CommitAsync();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            TempData["token"] = token; 
            return RedirectToAction(nameof(NewPassword) , new { userId = user.Id });
        }
        [HttpGet]
        public IActionResult NewPassword(string userId)
        {
            var token = TempData["token"] as string; 
            if (token is null )
            {
                return RedirectToAction(nameof(Login));
            }
            return View(new NewPasswordVM() { UserId = userId , Token  = token });
        }
        [HttpPost]
        public async Task<IActionResult> NewPassword(NewPasswordVM newPasswordVM)
        {
            if (newPasswordVM.Token is null)
            {
                return RedirectToAction(nameof(Login));
            }
            var user = await _userManager.FindByIdAsync(newPasswordVM.UserId);
            if (user is null)
            {
                ModelState.AddModelError("", "invalid user ");
                return View(newPasswordVM);
            }
            var result = await _userManager.ResetPasswordAsync(user  , newPasswordVM.Token, newPasswordVM.Password);
            if(!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description); 
                }
                return View(newPasswordVM); 
            }
            return RedirectToAction(nameof(Login)); 
        }
    }
}
