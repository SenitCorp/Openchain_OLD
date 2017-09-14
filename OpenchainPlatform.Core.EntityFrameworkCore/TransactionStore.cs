using Microsoft.EntityFrameworkCore;
using OpenchainPlatform.Core.Data;
using OpenchainPlatform.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.EntityFrameworkCore
{
    public class TransactionStore : TransactionStore<DbContext>
    {
        public TransactionStore(DbContext context, OperationErrorDescriber describer = null) : base(context, describer)
        {
        }
    }

    public class TransactionStore<TContext> : ITransactionStore where TContext : DbContext
    {
        public TContext Context { get; private set; }

        private DbSet<Transaction> Transactions { get { return Context.Set<Transaction>(); } }

        public OperationErrorDescriber ErrorDescriber { get; set; }

        public TransactionStore(TContext context, OperationErrorDescriber describer = null)
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

        public async Task<Transaction> FindByIdAsync(string transactionId)
        {
            return await Transactions.FindAsync(transactionId);
        }

        public async Task<OperationResult> CreateAsync(Transaction transaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            Context.Add(transaction);

            await SaveChanges(cancellationToken);

            return OperationResult.Success;
        }
    }
}
