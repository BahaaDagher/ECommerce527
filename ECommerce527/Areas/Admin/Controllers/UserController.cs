using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce527.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{CD.SUPER_ADMIN_ROLE}  , {CD.ADMIN_ROLE} ")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users; 
            return View(users);
        }
        public async Task<IActionResult> LockUnLock(string id)
        { 
            var user = await _userManager.FindByIdAsync(id); 
            if (user is null )
            {
                return NotFound(); 
            }
            bool isSuperAdmin = await _userManager.IsInRoleAsync(user  ,CD.SUPER_ADMIN_ROLE);
            if(isSuperAdmin)
            {
                TempData["Error-Notification"] = "You can't lock SuperAdmin !!!";
                return RedirectToAction(nameof(Index)); 
            }
            // user is unLocked
            if (user.LockoutEnd is null || DateTime.UtcNow > user.LockoutEnd) {
                await _userManager.SetLockoutEndDateAsync(user  , DateTime.UtcNow.AddMinutes(10));
                TempData["Success-Notification"] = "User Locked Successfully";
            }
            else
            {
                await _userManager.SetLockoutEndDateAsync(user , null);
                TempData["Success-Notification"] = "User UnLocked Successfully";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
