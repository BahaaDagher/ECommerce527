using ECommerce527.Data;
using ECommerce527.Models;
using ECommerce527.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ECommerce527.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        ApplicationDbContext _context = new ApplicationDbContext(); 
        public IActionResult Index(FilterProductVM filter)
        {
            var products = _context.Products.AsQueryable();
            products = products.Include(p => p.Category);
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
            if (filter.IsHot)
            {
                products = products.Where(p => p.Discount >= 50);
                ViewBag.IsHot = filter.IsHot;

            }
            //ViewData["Categories"] = _context.Categories.ToList();
            ViewBag.Categories = _context.Categories.ToList();
            //ViewBag.Brands = _context.Brands.ToList();
            ViewData["Brands"] = _context.Brands.ToList();
            ViewBag.TotalPages = (int)Math.Ceiling(products.Count() / 8.0); 
            ViewBag.CurrentPage = filter.Page;

            products = products.Skip((filter.Page - 1 )*8).Take(8);




            return View(products.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public ViewResult Welcome()
        {
            return View(); 
        }
        public ViewResult PersonalInfo(decimal salary  , string name)
        {
            var personsInDB = new List<Person>()
            {
                new Person(){ Id=1 , Name="Bahaa" , Address = "Cairo" , Salary = 10000} , 
                new Person(){ Id=2 , Name="Omar" , Address = "Giza" , Salary = 20000} , 
                new Person(){ Id=3 , Name="Mona" , Address = "Alex" , Salary = 30000} , 
            };
            // filter 
            var persons = personsInDB.Where(p => p.Salary >= salary); 
            if (name != null )
            {
                persons = personsInDB.Where(p => p.Name.Contains(name)); 
            }

            var count = persons.Count();
            return View(new PersonVM()
            {
                Persons = persons.ToList() ,
                Count = count
            }); 
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
