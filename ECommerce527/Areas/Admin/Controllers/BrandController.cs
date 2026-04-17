using ECommerce527.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace ECommerce527.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var brands = _context.Brands.AsQueryable(); 
            // filter 
            return View(brands.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Brand brand , IFormFile ImgFile)
        {
            if(ImgFile != null && ImgFile.Length > 0 )
            {
                //var fileName = Guid.NewGuid().ToString()  + Path.GetExtension(ImgFile.FileName) ; 
                var fileName = Guid.NewGuid().ToString()  + "-" +ImgFile.FileName ; 
                var filePath = Path.Combine(Directory.GetCurrentDirectory() , "wwwroot\\images" , fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    ImgFile.CopyTo(stream);
                }
                brand.Img = fileName; 
            }
            _context.Brands.Add(brand);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int id )
        {
            var brand = _context.Brands.FirstOrDefault(c=>c.Id == id); 
            if(brand == null)
            {
                return RedirectToAction("NotFoundPage" , "Home"); 
            }
            return View(brand);
        }
        [HttpPost]
        public IActionResult Update(Brand brand ,  IFormFile ImgFile)
        {
            var BrandInDb = _context.Brands.AsNoTracking().FirstOrDefault(b => b.Id == brand.Id);
  

            if (ImgFile != null && ImgFile.Length > 0)
            {
                //var fileName = Guid.NewGuid().ToString()  + Path.GetExtension(ImgFile.FileName) ; 
                var fileName = Guid.NewGuid().ToString() + "-" + ImgFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    ImgFile.CopyTo(stream);
                }
                brand.Img = fileName;

                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", BrandInDb.Img);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath); 
                }
            }
            else
            {
                brand.Img = BrandInDb.Img; 
            }
            _context.Brands.Update(brand);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var brand = _context.Brands.FirstOrDefault(c => c.Id == id);
            if (brand == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", brand.Img);

            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            _context.Brands.Remove(brand);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index)); 
        }
    }
}
