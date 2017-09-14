using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenchainPlatform.Core.Managers;
using OpenchainPlatform.Core.Services;
using OpenchainPlatform.Core;
using OpenchainPlatform.Core.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using OpenchainPlatform.Core.Data.Models;

namespace OpenchainPlatform.API
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);


            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<OpenchainPlatformDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("Platform"), b => b.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));
            });

            services.AddOpenchainPlatform(options => Configuration.GetSection("OpenchainPlatformOptions").Bind(options))
                .AddEntityFrameworkStores<OpenchainPlatformDbContext>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();

            //InitializeDatabase(app);
        }

        public void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<OpenchainPlatformDbContext>();
                var env = serviceScope.ServiceProvider.GetRequiredService<IHostingEnvironment>();

                var assetManager = serviceScope.ServiceProvider.GetRequiredService<AssetManager>(); 

                var assets = context.Set<Asset>();

                if(!assets.Any())
                {
                    var models = Config.GetAssets();

                    foreach(var model in models)
                    {
                        var task = assetManager.CreateAsync(model.Id, model.Name, model.Symbol, model.Precision);

                        task.Wait();

                        var result = task.Result;
                    }
                }
            }
        }
    }
}
