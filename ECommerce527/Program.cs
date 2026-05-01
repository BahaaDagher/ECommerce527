using ECommerce527.Data;
using ECommerce527.Repositories;
using ECommerce527.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace ECommerce527
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var connectionString =
                    builder.Configuration.GetConnectionString("DefaultConnection")
                        ?? throw new InvalidOperationException("Connection string"
                        + "'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true; 
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders(); 

            // Transient  , Scoped  ,SingleTone
            //builder.Services.AddSingleton<IRepository<Category> ,Repository<Category> >(); 
            builder.Services.AddScoped<IRepository<Category> ,Repository<Category> >(); 
            builder.Services.AddScoped<IRepository<Brand> ,Repository<Brand>>(); 
            builder.Services.AddScoped<IRepository<Product> ,Repository<Product> >(); 
            builder.Services.AddScoped<IRepository<ApplicationUserOtp> ,Repository<ApplicationUserOtp> >(); 
            builder.Services.AddScoped<IProductSubImageRepository ,ProductSubImageRepository>(); 
            builder.Services.AddScoped<IProductColorRepository ,ProductColorRepository>(); 
            builder.Services.AddTransient<IEmailSender,EmailSender>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Identity}/{controller=Account}/{action=Login}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
