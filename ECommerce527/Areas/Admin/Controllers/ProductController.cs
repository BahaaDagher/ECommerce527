using ECommerce527.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Drawing;

namespace ECommerce527.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(FilterProductVM filter)
        {
            var products = _context.Products.AsQueryable();
            products = products.Include(p => p.Category).Include(p => p.Brand);
            // filter 
            if (filter.ProductName != null)
            {
                products = products.Where(p => p.Name.Contains(filter.ProductName));
                ViewBag.ProductName = filter.ProductName;
            }
            if (filter.MinPrice > 0)
            {
                products = products.Where(p => p.Price - (p.Price * p.Discount / 100) >= filter.MinPrice);
                ViewBag.MinPrice = filter.MinPrice;

            }
            if (filter.MaxPrice > 0)
            {
                products = products.Where(p => p.Price - (p.Price * p.Discount / 100) >= filter.MaxPrice);
                ViewBag.MaxPrice = filter.MaxPrice;

            }
            if (filter.CategoryId > 0)
            {
                products = products.Where(p => p.CategoryId == filter.CategoryId);
                ViewBag.CategoryId = filter.CategoryId;

            }
            if (filter.BrandId > 0)
            {
                products = products.Where(p => p.BrandId == filter.BrandId);
                ViewBag.BrandId = filter.BrandId;

            }
            if (filter.IsLowQuantity)
            {
                products = products.OrderBy(p => p.Quantity);
                ViewBag.IsLowQuantity = filter.IsLowQuantity;

            }
            //ViewData["Categories"] = _context.Categories.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            //ViewBag.Brands = _context.Brands.ToList();
            ViewData["Brands"] = _context.Brands.ToList();
            ViewBag.TotalPages = (int)Math.Ceiling(products.Count() / 8.0);
            ViewBag.CurrentPage = filter.Page;

            products = products.Skip((filter.Page - 1) * 8).Take(8);

            return View(products.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();   
            var brands = _context.Brands.ToList();   
            return View(new ProductVM()
            {
                Categories = categories  ,
                Brands = brands
            });
        }
        [HttpPost]
        public IActionResult Create(Product product , IFormFile ImgFile , List<IFormFile> SubImgFiles , List<string> Colors)
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
                product.MainImg = fileName; 
            }
            var savedProduct = _context.Products.Add(product);
            _context.SaveChanges();

            if(SubImgFiles is not null && SubImgFiles.Count>0)
            {
                foreach(var image in SubImgFiles)
                {
                    if (image != null && image.Length > 0)
                    {
                        //var fileName = Guid.NewGuid().ToString()  + Path.GetExtension(ImgFile.FileName) ; 
                        var fileName = Guid.NewGuid().ToString() + "-" + image.FileName;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\productSubImages", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            image.CopyTo(stream);
                        }
                        //product.MainImg = fileName;
                        _context.ProductSubImages.Add(new ProductSubImage()
                        {
                            ProductId = savedProduct.Entity.Id, 
                            Img = fileName 
                        });
                    }
                }
            }
            if(Colors is not null && Colors.Count>0)
            {
                foreach ( var color in Colors)
                {
                    _context.ProductColors.Add(new ProductColor()
                    {
                        ProductId = savedProduct.Entity.Id,
                        Color = color
                    });
                }
            }

            _context.SaveChanges(); 

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Update(int id )
        {
            var product = _context.Products.FirstOrDefault(c=>c.Id == id); 
            if(product == null)
            {
                return RedirectToAction("NotFoundPage" , "Home"); 
            }
            return View(new ProductVM()
            {
                Product = product,
                Categories = _context.Categories.ToList(),
                Brands = _context.Brands.ToList(),
                ProductSubImages = _context.ProductSubImages.Where(ps=>ps.ProductId == id).ToList(),
                ProductColors = _context.ProductColors.Where(pc => pc.ProductId == id).ToList()
            }); 
        }
        [HttpPost]
        public IActionResult Update(Product product ,  IFormFile ImgFile ,  List<IFormFile> SubImgFiles, List<string> Colors)
        {
            var ProductInDb = _context.Products.AsNoTracking().FirstOrDefault(b => b.Id == product.Id);
  

            if (ImgFile != null && ImgFile.Length > 0)
            {
                //var fileName = Guid.NewGuid().ToString()  + Path.GetExtension(ImgFile.FileName) ; 
                var fileName = Guid.NewGuid().ToString() + "-" + ImgFile.FileName;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    ImgFile.CopyTo(stream);
                }
                product.MainImg = fileName;

                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", ProductInDb.MainImg);

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath); 
                }
            }
            else
            {
                product.MainImg = ProductInDb.MainImg; 
            }
            _context.Products.Update(product);
            _context.SaveChanges();

            // Sub images

            if (SubImgFiles is not null && SubImgFiles.Count > 0)
            {
                var oldImages = _context.ProductSubImages.Where(ps=>ps.ProductId == ProductInDb.Id);
                // Remove from DB 
                _context.ProductSubImages.RemoveRange(oldImages);
                // Remove from wwwroot
                foreach (var item in oldImages) {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\productSubImages", item.Img);

                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                foreach (var image in SubImgFiles)
                {
                    if (image != null && image.Length > 0)
                    {
                        // insert in WWWROOT
                        //var fileName = Guid.NewGuid().ToString()  + Path.GetExtension(ImgFile.FileName) ; 
                        var fileName = Guid.NewGuid().ToString() + "-" + image.FileName;
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\productSubImages", fileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            image.CopyTo(stream);
                        }
                        // insert in DB
                        _context.ProductSubImages.Add(new ProductSubImage()
                        {
                            ProductId = ProductInDb.Id,
                            Img = fileName
                        });
                    }
                }
            }
            if (Colors is not null && Colors.Count > 0)
            {
                var oldColors = _context.ProductColors.Where(pc=>pc.ProductId == ProductInDb.Id);
                _context.ProductColors.RemoveRange(oldColors); 
                foreach (var color in Colors)
                {
                    _context.ProductColors.Add(new ProductColor()
                    {
                        ProductId = ProductInDb.Id,
                        Color = color
                    });
                }
            }
            _context.SaveChanges(); 
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", product.MainImg);

            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }

            var oldImages = _context.ProductSubImages.Where(ps => ps.ProductId == id);
            // Remove from wwwroot
            foreach (var item in oldImages)
            {
                var oldSumPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\productSubImages", item.Img);

                if (System.IO.File.Exists(oldSumPath))
                {
                    System.IO.File.Delete(oldSumPath);
                }
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index)); 
        }
    }
}
