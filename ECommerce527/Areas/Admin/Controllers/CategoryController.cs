using ECommerce527.Data;
using ECommerce527.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace ECommerce527.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        //ApplicationDbContext _context = new ApplicationDbContext();
        IRepository<Category> _categoryRepository; // = new Repository<Category>(); 

        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            //var categories = _context.Categories.AsQueryable(); 
            var categories = await _categoryRepository.GetAsync(); 
            // filter 
            return View(categories.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View( new Category());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error-Notification"] = "invalid Data";
                return View(category);
            }
            //_context.Categories.Add(category);
            //_context.SaveChanges();
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.CommitAsync(); 
            //Response.Cookies.Append("Success-Notification", "category Created Successfully "); 
            TempData["Success-Notification"] = "category Created Successfully "; 
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id )
        {
            //var category = _context.Categories.FirstOrDefault(c=>c.Id == id); 
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id); 
            if(category == null)
            {
                return RedirectToAction("NotFoundPage" , "Home"); 
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Category category)
        {
            //_context.Categories.Update(category);
            //_context.SaveChanges();
            _categoryRepository.Update(category);
            await _categoryRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);

            if (category == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            //_context.Categories.Remove(category);
            //_context.SaveChanges();
            _categoryRepository.Delete(category);
            await _categoryRepository.CommitAsync(); 
            return RedirectToAction(nameof(Index)); 
        }
    }
}
