using OpenchainPlatform.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.Data
{
    public interface IAssetStore
    {
        Task<OperationResult> CreateAsync(Asset asset, CancellationToken cancellationToken);

        Task<OperationResult> UpdateAsync(Asset asset, CancellationToken cancellationToken);

        Task<Asset> FindByIdAsync(string assetId);

        Task<ICollection<Asset>> FindAll();
    }
}
