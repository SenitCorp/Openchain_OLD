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
    public class AccountStore : AccountStore<DbContext>
    {
        public AccountStore(DbContext context, OperationErrorDescriber describer = null) : base(context, describer)
        {
        }
    }

    public class AccountStore<TContext> : IAccountStore where TContext : DbContext
    {
        public TContext Context { get; private set; }

        private DbSet<Account> Accounts { get { return Context.Set<Account>(); } }

        public OperationErrorDescriber ErrorDescriber { get; set; }

        public AccountStore(TContext context, OperationErrorDescriber describer = null)
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

        public async Task<OperationResult> CreateAsync(Account account, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            Context.Add(account);

            await SaveChanges(cancellationToken);

            return OperationResult.Success;
        }

        public async Task<OperationResult> UpdateAsync(Account account, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            Context.Attach(account);
            Context.Update(account);

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

        public async Task<Account> FindByIdAsync(string accountId)
        {
            return await Accounts.FindAsync(accountId);
        }

        public async Task<ICollection<Account>> FindAll()
        {
            var accounts = await Accounts.ToListAsync();

            return accounts.AsReadOnly();
        }
    }
}
