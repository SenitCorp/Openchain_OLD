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
    public class AssetStore : AssetStore<DbContext>
    {
        public AssetStore(DbContext context, OperationErrorDescriber describer = null) : base(context, describer)
        {
        }
    }

    public class AssetStore<TContext> : IAssetStore where TContext : DbContext
    {
        public TContext Context { get; private set; }

        private DbSet<Asset> Assets { get { return Context.Set<Asset>(); } }

        public OperationErrorDescriber ErrorDescriber { get; set; }

        public AssetStore(TContext context, OperationErrorDescriber describer = null)
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

        public async Task<OperationResult> CreateAsync(Asset asset, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (asset == null)
            {
                throw new ArgumentNullException(nameof(asset));
            }

            Context.Add(asset);

            await SaveChanges(cancellationToken);

            return OperationResult.Success;
        }

        public async Task<OperationResult> UpdateAsync(Asset asset, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (asset == null)
            {
                throw new ArgumentNullException(nameof(asset));
            }

            Context.Attach(asset);
            Context.Update(asset);

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

        public async Task<Asset> FindByIdAsync(string assetId)
        {
            return await Assets.FindAsync(assetId);
        }

        public async Task<ICollection<Asset>> FindAll()
        {
            var assets = await Assets.ToListAsync();

            return assets.AsReadOnly();
        }
    }
}
