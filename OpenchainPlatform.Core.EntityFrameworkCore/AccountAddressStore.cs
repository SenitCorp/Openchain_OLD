using OpenchainPlatform.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenchainPlatform.Core.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace OpenchainPlatform.Core.EntityFrameworkCore
{
    public class AccountAddressStore : AccountAddressStore<DbContext>
    {
        public AccountAddressStore(DbContext context, OperationErrorDescriber describer = null) : base(context, describer)
        {
        }
    }

    public class AccountAddressStore<TContext> : IAccountAddressStore where TContext : DbContext
    {
        public TContext Context { get; private set; }

        private DbSet<AccountAddress> AccountAddresses { get { return Context.Set<AccountAddress>(); } }

        public OperationErrorDescriber ErrorDescriber { get; set; }

        public AccountAddressStore(TContext context, OperationErrorDescriber describer = null)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Context = context;
            ErrorDescriber = describer ?? new OperationErrorDescriber();
        }

        private Task SaveChanges(CancellationToken cancellationToken)
        {
            return Context.SaveChangesAsync(cancellationToken);
        }

        public async Task<OperationResult> CreateAsync(AccountAddress accountAddress, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (accountAddress == null)
            {
                throw new ArgumentNullException(nameof(accountAddress));
            }

            Context.Add(accountAddress);

            await SaveChanges(cancellationToken);

            return OperationResult.Success;
        }

        public async Task<OperationResult> UpdateAsync(AccountAddress accountAddress, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (accountAddress == null)
            {
                throw new ArgumentNullException(nameof(accountAddress));
            }

            Context.Attach(accountAddress);
            Context.Update(accountAddress);

            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return OperationResult.Failed(ErrorDescriber.DefaultError());
            }

            return OperationResult.Success;
        }

        public async Task<AccountAddress> FindByAddressAsync(string address)
        {
            return await AccountAddresses.Include(a => a.Account).FirstOrDefaultAsync(a => a.Address.Equals(address));
        }

        public async Task<ICollection<AccountAddress>> FindAll()
        {
            var accountAddresses = await AccountAddresses.ToListAsync();

            return accountAddresses.AsReadOnly();
        }
    }
}
