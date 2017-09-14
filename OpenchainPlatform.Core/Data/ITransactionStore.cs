using OpenchainPlatform.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.Data
{
    public interface ITransactionStore
    {
        Task<Transaction> FindByIdAsync(string transactionId);
        Task<OperationResult> CreateAsync(Transaction transaction, CancellationToken cancellationToken);
    }
}
