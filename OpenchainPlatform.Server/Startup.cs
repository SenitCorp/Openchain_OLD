using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Openchain;
using Openchain.EntityFrameworkCore;
using Openchain.Infrastructure;
using Openchain.Server.Models;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OpenchainPlatform.Server
{
    public class Startup
    {
        private static readonly string version = "0.7.1";
        private List<Task> runningTasks = new List<Task>();
        private readonly IConfiguration configuration;

        public Startup(IHostingEnvironment application)
        {
            // Setup Configuration
            configuration = new ConfigurationBuilder()
                .SetBasePath(application.ContentRootPath)
                .AddJsonFile("data/config.json")
                .AddEnvironmentVariables()
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureServicesAsync(services).Wait();
        }

        /// <summary>
        /// Adds services to the dependency injection container.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        public async Task ConfigureServicesAsync(IServiceCollection services)
        {
            services.BuildServiceProvider().GetService<ILoggerFactory>().AddConsole();

            services.AddSingleton<IConfiguration>(_ => this.configuration);

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            /*
            services.AddDbContext<OpenchainStorageDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(migrationsAssembly));
            });

            services.AddDbContext<OpenchainAnchorStateDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(migrationsAssembly));
            });*/

            services.AddDbContext<OpenchainDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(migrationsAssembly));
            });

            // Setup ASP.NET MVC
            services
                .AddMvcCore()
                .AddViews()
                .AddJsonFormatters();

            // Logger
            services.AddTransient<ILogger>(ConfigurationParser.CreateLogger);

            LogStartup(services.BuildServiceProvider().GetService<ILogger>(), services.BuildServiceProvider().GetService<IHostingEnvironment>());

            // CORS Headers
            services.AddCors();

            // Ledger Store
            /*
            services.AddScoped<IStorageEngine>(await ConfigurationParser.CreateStorageEngine(services.BuildServiceProvider()));

            services.AddScoped<ILedgerQueries>(ConfigurationParser.CreateLedgerQueries);

            services.AddScoped<ILedgerIndexes>(ConfigurationParser.CreateLedgerIndexes);

            services.AddScoped<IAnchorState>(await ConfigurationParser.CreateAnchorState(services.BuildServiceProvider()));

            */

            services.AddEntityFrameworkCoreStorage<OpenchainDbContext>();

            services.AddEntityFrameworkCoreAnchorState<OpenchainDbContext>();

            services.AddScoped<IAnchorRecorder>(await ConfigurationParser.CreateAnchorRecorder(services.BuildServiceProvider()));

            services.AddScoped<IMutationValidator>(await ConfigurationParser.CreateRulesValidator(services.BuildServiceProvider()));

            services.AddScoped<TransactionValidator>(ConfigurationParser.CreateTransactionValidator);

            services.AddSingleton<GlobalSettings>(ConfigurationParser.CreateGlobalSettings);

            // Transaction Stream Subscriber
            services.AddSingleton<TransactionStreamSubscriber>(ConfigurationParser.CreateStreamSubscriber);

            // Anchoring
            services.AddSingleton<LedgerAnchorWorker>(ConfigurationParser.CreateLedgerAnchorWorker);
        }

        private static void LogStartup(ILogger logger, IHostingEnvironment environment)
        {
            logger.LogInformation($"Starting Openchain v{version}");
            logger.LogInformation(" ");
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory, IConfiguration configuration, IStorageEngine store)
        {
            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .WithExposedHeaders("Content-Range", "Content-Length", "Content-Encoding"));

            app.Map("/stream", managedWebSocketsApp =>
            {
                if (bool.Parse(configuration["enable_transaction_stream"]))
                {
                    managedWebSocketsApp.UseWebSockets();
                    managedWebSocketsApp.Use(next => new TransactionStreamMiddleware(next).Invoke);
                }
            });

            // Add MVC to the request pipeline.
            app.UseMvc();

            // Verify the transaction validator
            app.ApplicationServices.GetService<TransactionValidator>();

            // Activate singletons
            TransactionStreamSubscriber subscriber = app.ApplicationServices.GetService<TransactionStreamSubscriber>();
            if (subscriber != null)
                runningTasks.Add(subscriber.Subscribe(CancellationToken.None));

            app.ApplicationServices.GetService<IMutationValidator>();

            LedgerAnchorWorker anchorWorker = app.ApplicationServices.GetService<LedgerAnchorWorker>();
            if (anchorWorker != null)
                runningTasks.Add(anchorWorker.Run(CancellationToken.None));
        }
    }
}
