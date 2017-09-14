using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenchainPlatform.Core.Managers;
using OpenchainPlatform.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OpenchainPlatform.Core
{
    public static class OpenchainPlatformServiceCollectionExtension
    {
        public static OpenchainPlatformBuilder AddOpenchainPlatform(this IServiceCollection services, Action<OpenchainPlatformOptions> setupAction)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddScoped<OperationErrorDescriber>();
            services.TryAddScoped<OpenchainService>();

            services.TryAddTransient<AssetManager>();
            services.TryAddTransient<AccountManager>();
            services.TryAddTransient<AccountAddressManager>();
            services.TryAddTransient<TransactionManager>();

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            return new OpenchainPlatformBuilder(services);
        }
    }
}
