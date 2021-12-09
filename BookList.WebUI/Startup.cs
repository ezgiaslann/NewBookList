using BookList.Business.Abstract;
using BookList.Business.Concrete;
using BookList.DataAccess.Abstract;
using BookList.DataAccess.Concrete.EfCore;
using BookList.WebUI.Identity;
using BookList.WebUI.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookList.WebUI
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationIdentityDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("IdentityConnection")));


            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                //Password
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.AllowedForNewUsers = true;

                //options.User.AllowedUserNameCharacters = " ";
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
                options.AccessDeniedPath = "/account/accessdenied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true,
                    Name = ".BookList.Security.Cookie",
                    SameSite = SameSiteMode.Strict
                };
            });

            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddScoped<IProductDal, EfCoreProductDal>();
            services.AddScoped<ICategoryDal, EfCoreCategoryDal>();
            services.AddScoped<ICartDal, EfCoreCartDal>();
            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<ICartService, CartManager>();

            services.AddMvc()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                SeedDatabase.Seed();
            }
            app.UseStaticFiles();
            //app.CustomStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                // routes.MapRoute(
                //  name: "adminProducts",
                //  template: "admin/products",
                //  defaults: new
                //  {
                //      controller = "Admin",
                //      action = "ProductList"
                //  }
                //);

                routes.MapRoute(
                   name: "adminProducts",
                   template: "admin/products/{id?}",
                   defaults: new
                   {
                       controller = "Admin",
                       action = "EditProduct"
                   }
               );

                routes.MapRoute(
                    name: "cart",
                    template: "cart",
                    defaults: new
                    {
                        controller = "Cart",
                        action = "Index"
                    }
                );

                routes.MapRoute(
                    name: "checkout",
                    template: "checkout",
                    defaults: new
                    {
                        controller = "Cart",
                        action = "Checkout"
                    }
                );

                routes.MapRoute(
                  name: "products",
                  template: "products/{category?}",
                  defaults: new
                  {
                      controller = "Shop",
                      action = "List"
                  }
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );
            });

            SeedIdentity.Seed(userManager, roleManager, Configuration).Wait();
        }
    }
}
