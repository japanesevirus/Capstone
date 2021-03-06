﻿using AutoMapper;
using Capstone.Data;
using Capstone.Data.Infrastructrure;
using Capstone.Data.Repositories;
using Capstone.Mappings;
using Capstone.Model;
using Capstone.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Capstone
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
            services.AddDbContext<CapstoneEntities>();

            #region DI

            services.AddScoped<IDbFactory, DbFactory>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //WorkFlow
            services.AddTransient<IWorkFlowRepository, WorkFlowRepository>();
            services.AddTransient<IWorkFlowService, WorkFlowService>();

            //ActionType
            services.AddTransient<IActionTypeRepository, ActionTypeRepository>();
            services.AddTransient<IActionTypeService, ActionTypeService>();

            //Group
            services.AddTransient<IGroupRepository, GroupRepository>();
            services.AddTransient<IGroupService, GroupService>();

            //UserGroup
            services.AddTransient<IUserGroupRepository, UserGroupRepository>();
            services.AddTransient<IUserGroupService, UserGroupService>();

            //Role
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IRoleService, RoleService>();

            //RoleOfGroup
            services.AddTransient<IRoleOfGroupRepository, RoleOfGroupRepository>();
            services.AddTransient<IRoleOfGroupService, RoleOfGroupService>();

            //Permission
            services.AddTransient<IPermissionRepository, PermissionRepository>();
            services.AddTransient<IPermissionService, PermissionService>();

            //PermissionOfRole
            services.AddTransient<IPermissionOfRoleRepository, PermissionOfRoleRepository>();
            services.AddTransient<IPermissionOfRoleService, PermissionOfRoleService>();

            //UserRole
            services.AddTransient<IUserRoleRepository, UserRoleRepository>();
            services.AddTransient<IUserRoleService, UserRoleService>();

            #endregion

            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });

            // Add identity
            var authBuilder = services.AddIdentityCore<User>(o =>
            {
                // Configure identity options
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
            });
            authBuilder = new IdentityBuilder(authBuilder.UserType, typeof(IdentityRole), authBuilder.Services);
            authBuilder.AddEntityFrameworkStores<CapstoneEntities>().AddDefaultTokenProviders();

            //services.AddDbContext<CapstoneEntities>(opt => opt.UseSqlServer("CapstoneEntities"));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Capstone API", Version = "v1" });
                c.AddSecurityDefinition("Bearer",
                new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                { "Bearer", Enumerable.Empty<string>() },
            });
            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable jwt
            app.UseAuthentication();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // Specifying the swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Capstone API V1");
                c.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // the default hsts value is 30 days. you may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
