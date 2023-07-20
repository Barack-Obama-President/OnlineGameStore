using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineGameStore.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using OnlineGameStore.Models;

namespace OnlineGameStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddDbContext<OnlineGameStoreContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("OnlineGameStoreContext")));

            services.AddIdentity<ApplicationUser,ApplicationRole>()
                 .AddDefaultUI()
                 .AddEntityFrameworkStores<OnlineGameStoreContext>()
                 .AddDefaultTokenProviders();

            services.AddMvc()
             .AddRazorPagesOptions(options =>
             {
                 // options.Conventions.AllowAnonymousToFolder("/Games");
                 // options.Conventions.AuthorizePage("/Games/Create");
                 // options.Conventions.AuthorizeAreaPage("Identity", "/Manage/Accounts");
                 options.Conventions.AuthorizeFolder("/Games");
             });

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 1;
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;
                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // options.Cookie.Name = "YourCookieName";
                // options.Cookie.Domain=
                // options.LoginPath = "/Account/Login";
                // options.LogoutPath = "/Account/Logout";
                // options.AccessDeniedPath = "/Account/AccessDenied";

                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromSeconds(500);
                options.SlidingExpiration = true;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            app.UseStatusCodePages("text/html", "<h1>Status code page</h1> <h2>Status Code: {0}</h2>");
            app.UseExceptionHandler("/Error");
                
            //}

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
         
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
