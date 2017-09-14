using OpenchainPlatform.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.Data
{
    public interface IAccountAddressStore
    {
        Task<OperationResult> CreateAsync(AccountAddress account, CancellationToken cancellationToken);

        Task<OperationResult> UpdateAsync(AccountAddress account, CancellationToken cancellationToken);

        Task<AccountAddress> FindByAddressAsync(string address);

        Task<ICollection<AccountAddress>> FindAll();
    }
}
