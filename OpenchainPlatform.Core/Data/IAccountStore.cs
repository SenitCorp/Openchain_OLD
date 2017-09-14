using OpenchainPlatform.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.Data
{
    public interface IAccountStore
    {
        Task<OperationResult> CreateAsync(Account account, CancellationToken cancellationToken);

        Task<OperationResult> UpdateAsync(Account account, CancellationToken cancellationToken);

        Task<Account> FindByIdAsync(string accountId);

        Task<ICollection<Account>> FindAll();
    }
}
