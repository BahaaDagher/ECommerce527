using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;

namespace ECommerce527.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user is null)
            {
                return NotFound();
            }
            //var applicationUserVM = new ApplicationUserVM();

            //applicationUserVM.Name = user.Name; 
            //applicationUserVM.Email = user.Email;
            //applicationUserVM.Address= user.Address;
            //applicationUserVM.PhoneNumber= user.PhoneNumber;

            var applicationUserVM = user.Adapt<ApplicationUserVM>();

            return View(applicationUserVM);
        }
        public async Task<IActionResult> UpdateProfile(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }
            //var userDb = applicationUserVM.Adapt<ApplicationUser>();
            user.Name = applicationUserVM.Name; 
            user.PhoneNumber = applicationUserVM.PhoneNumber; 
            user.Address = applicationUserVM.Address; 
            var result = await _userManager.UpdateAsync(user); 
            if (!result.Succeeded)
            {
                string errors = string.Join(" ,", result.Errors.Select(e=>e.Description));
                TempData["Error-Notification"] = errors;
                return View(nameof(Index), applicationUserVM); 
            }
            else
            {
                TempData["Success-Notification"] = "user Updated Successfully";
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> UpdatePassword(ApplicationUserVM applicationUserVM)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }
            var result = await _userManager.ChangePasswordAsync(user , applicationUserVM.CurrentPassword , applicationUserVM.NewPassword);
            if (!result.Succeeded)
            {
                string errors = string.Join(" ,", result.Errors.Select(e => e.Description));
                TempData["Error-Notification"] = errors;
                return View(nameof(Index), applicationUserVM);
            }
            else
            {
                TempData["Success-Notification"] = "password Changed Successfully";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
