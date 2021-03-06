using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace DutchTreat
{
  public class Startup
  {
    private readonly IConfiguration config;

    public Startup(IConfiguration config)
    {
      this.config = config;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddIdentity<StoreUser, IdentityRole>(cfg =>
      {
        cfg.User.RequireUniqueEmail = true;
      }).AddEntityFrameworkStores<DutchContext>();

      services.AddDbContext<DutchContext>(cfg =>
      {
        cfg.UseSqlServer(config.GetConnectionString("DutchConnectionString"));
      });

      services.AddTransient<IMailService, NullMailService>();
      // Support for real mail service

      services.AddTransient<DutchSeeder>();

      services.AddAutoMapper(Assembly.GetExecutingAssembly());

      services.AddScoped<IDutchRepository, DutchRepository>();

      services.AddMvc()
        .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if(env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseStaticFiles();
      app.UseNodeModules();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseRouting();

      app.UseEndpoints(cfg =>
      {
        cfg.MapControllerRoute("Fallback",
          "{controller}/{action}/{id?}",
          new {controller = "App", action = "Index"});
      });
    }
  }
}
