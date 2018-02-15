using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LeaveSystem.Presentation.Services;
using LeaveSystem.Data.Model;
using LeaveSystem.Data;
using LeaveSystem.Business;
using LeaveSystem.Data.Uow;
using AutoMapper;
using NonFactors.Mvc.Grid;
using LeaveSystem.Business.Interfaces;
using AspNet.Security.OpenIdConnect.Primitives;
using LeaveSystem.Infrastructure;
using AppPermissions = LeaveSystem.Infrastructure.ApplicationPermissions;
using LeaveSystem.Presentation.Authorization;

namespace LeaveSystem.Presentation
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),b=> b.MigrationsAssembly("LeaveSystem.Presentation")));

            services.AddIdentity<Employee, Role>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IEmployeeManager, EmployeeManager>();
            services.AddScoped<ILeaveManager, LeaveManager>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());

            //services.Configure<IdentityOptions>(options =>
            //{
            //    // User settings
            //    options.User.RequireUniqueEmail = true;


            //    options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
            //    options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
            //    options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            //});
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy(Authorization.Policies.ViewAllUsersPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.ViewUsers));
            //    options.AddPolicy(Authorization.Policies.ManageAllUsersPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.ManageUsers));

            //    options.AddPolicy(Authorization.Policies.ViewAllRolesPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.ViewRoles));
            //    options.AddPolicy(Authorization.Policies.ViewRoleByRoleNamePolicy, policy => policy.Requirements.Add(new ViewRoleAuthorizationRequirement()));
            //    options.AddPolicy(Authorization.Policies.ManageAllRolesPolicy, policy => policy.RequireClaim(CustomClaimTypes.Permission, AppPermissions.ManageRoles));

            //    options.AddPolicy(Authorization.Policies.AssignAllowedRolesPolicy, policy => policy.Requirements.Add(new AssignRolesAuthorizationRequirement()));
            //});

            services.AddMvc();
            services.AddMvcGrid();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
