using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenchainPlatform.Core.Data;
using OpenchainPlatform.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.EntityFrameworkCore
{
    public static class OpenchainPlatformEntityFrameworkBuilderExtension
    {
        /// <summary>
        /// Adds an Entity Framework implementation of openchain platform information stores.
        /// </summary>
        /// <typeparam name="TContext">The Entity Framework database context to use.</typeparam>
        /// <param name="builder">The <see cref="OpenchainPlatformBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IdentityBuilder"/> instance this method extends.</returns>
        public static OpenchainPlatformBuilder AddEntityFrameworkStores<TContext>(this OpenchainPlatformBuilder builder)
            where TContext : DbContext
        {
            AddStores(builder.Services, typeof(TContext));

            return builder;
        }

        private static void AddStores(IServiceCollection services, Type contextType)
        {
            services.TryAddScoped(typeof(IAssetStore), typeof(AssetStore<>).MakeGenericType(contextType));
            services.TryAddScoped(typeof(IAccountStore), typeof(AccountStore<>).MakeGenericType(contextType));
            services.TryAddScoped(typeof(IAccountAddressStore), typeof(AccountAddressStore<>).MakeGenericType(contextType));
            services.TryAddScoped(typeof(ITransactionStore), typeof(TransactionStore<>).MakeGenericType(contextType));
        }
    }
}
