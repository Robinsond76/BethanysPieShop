using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BethanysPieShop.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BethanysPieShop
{
    public class Startup
    {
        //This constructor injection allows access to properties set in appsettings.json
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //Add the DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IPieRepository, PieRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IOrderRepository, OrderRepository>();

            //Creates a scoped shopping cart for each user that sends a request. The getCart method will be invoked
            //with each request. This checks if the cartId is already in the session and if not, create one
            services.AddScoped<ShoppingCart>(sp => ShoppingCart.GetCart(sp));

            //Support for the HTTP Context accessor needed for the shopping cart
            services.AddHttpContextAccessor();

            //Support for sessions
            services.AddSession();

            //register framework services
            services.AddControllersWithViews(); //Support for MVC
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection(); //redirects regular http reqs to use https
            app.UseStaticFiles(); //Will serve static files. Not default
            app.UseSession(); // Must be before UseRouting()
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
