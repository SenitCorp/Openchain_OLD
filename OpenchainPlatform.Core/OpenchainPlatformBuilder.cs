using Microsoft.Extensions.DependencyInjection;
using OpenchainPlatform.Core.Data;
using OpenchainPlatform.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core
{
    public class OpenchainPlatformBuilder
    {
        public IServiceCollection Services { get; private set; }

        public OpenchainPlatformBuilder(IServiceCollection services)
        {
            Services = services;
        }

        private OpenchainPlatformBuilder AddScoped(Type serviceType, Type concreteType)
        {
            Services.AddScoped(serviceType, concreteType);

            return this;
        }

        /// <summary>
        /// Adds an <see cref="OperationErrorDescriber"/>.
        /// </summary>
        /// <typeparam name="TDescriber">The type of the error describer.</typeparam>
        /// <returns>The current <see cref="OpenchainPlatformBuilder"/> instance.</returns>
        public virtual OpenchainPlatformBuilder AddErrorDescriber<TDescriber>() where TDescriber : OperationErrorDescriber
        {
            Services.AddScoped<OperationErrorDescriber, TDescriber>();

            return this;
        }

        /// <summary>
        /// Adds an <see cref="IAccountStore"/> for the <seealso cref="Account"/>.
        /// </summary>
        /// <typeparam name="T">The account type held in the store.</typeparam>
        /// <returns>The current <see cref="OpenchainPlatformBuilder"/> instance.</returns>
        public virtual OpenchainPlatformBuilder AddAccountStore<T>() where T : class
        {
            return AddScoped(typeof(IAccountStore), typeof(T));
        }

        /// <summary>
        /// Adds an <see cref="ITransactionStore"/> for the <seealso cref="Transaction"/>.
        /// </summary>
        /// <typeparam name="T">The transaction type held in the store.</typeparam>
        /// <returns>The current <see cref="OpenchainPlatformBuilder"/> instance.</returns>
        public virtual OpenchainPlatformBuilder AddTransactionStore<T>() where T : class
        {
            return AddScoped(typeof(ITransactionStore), typeof(T));
        }
    }
}
