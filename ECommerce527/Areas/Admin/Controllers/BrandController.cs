using ECommerce527.Data;
using ECommerce527.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace ECommerce527.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        //ApplicationDbContext _context = new ApplicationDbContext();
        IRepository<Brand> _brandRepository;// = new Repository<Brand>(); 

        public BrandController(IRepository<Brand> brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<IActionResult> Index()
        {
            //var brands = _context.Brands.AsQueryable(); 
            var brands = await _brandRepository.GetAsync(); 
            // filter 
            return View(brands.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Brand brand , IFormFile ImgFile)
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
            //_context.Brands.Add(brand);
            //_context.SaveChanges();
            await _brandRepository.AddAsync(brand);
            await  _brandRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id )
        {
            //var brand = _context.Brands.FirstOrDefault(c=>c.Id == id); 
            var brand = await _brandRepository.GetOneAsync(c => c.Id == id); 
            if(brand == null)
            {
                return RedirectToAction("NotFoundPage" , "Home"); 
            }
            return View(brand);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Brand brand ,  IFormFile ImgFile)
        {
            //var BrandInDb = _context.Brands.AsNoTracking().FirstOrDefault(b => b.Id == brand.Id);
            var BrandInDb = await _brandRepository.GetOneAsync(b => b.Id == brand.Id ,  tracked: false);
  

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
            //_context.Brands.Update(brand);
            //_context.SaveChanges();
            _brandRepository.Update(brand);
            await _brandRepository.CommitAsync(); 
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            //var brand = _context.Brands.FirstOrDefault(c => c.Id == id);
            var brand = await _brandRepository.GetOneAsync(c => c.Id == id);
            if (brand == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", brand.Img);

            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            //_context.Brands.Remove(brand);
            //_context.SaveChanges();
            _brandRepository.Delete(brand);
            await _brandRepository.CommitAsync(); 
            return RedirectToAction(nameof(Index)); 
        }
    }
}
