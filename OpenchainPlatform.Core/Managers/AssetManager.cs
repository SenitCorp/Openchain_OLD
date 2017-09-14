using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenchainPlatform.Core.Data;
using OpenchainPlatform.Core.Data.Models;
using OpenchainPlatform.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.Managers
{
    public class AssetManager
    {
        private readonly OpenchainService _openchainService;
        private readonly ILogger _logger;
        private readonly IAssetStore _assetStore;
        private readonly HttpContext _context;
        private CancellationToken CancellationToken => _context?.RequestAborted ?? CancellationToken.None;

        public AssetManager(IAssetStore assetStore, OpenchainService openchainService, IHttpContextAccessor contextAccessor, ILoggerFactory loggerFactory)
        {
            _assetStore = assetStore;
            _openchainService = openchainService;
            _context = contextAccessor?.HttpContext;

            _logger = loggerFactory.CreateLogger<AssetManager>();
        }

        public async Task<Asset> FindByIdAsync(string assetId)
        {
            assetId = assetId.ToUpper();

            var asset = await _assetStore.FindByIdAsync(assetId);

            if(asset != null)
            {
                var chainAsset = await _openchainService.GetAssetAsync((uint)asset.Index);

                if (chainAsset == null)
                {
                    _logger.LogInformation($"An asset with Id='{assetId}' was found in the database but not in the chain.");

                    return null;
                }
            }

            return asset;
        }

        public async Task<Asset> CreateAsync(string assetId, string name, string symbol, int precision)
        {
            assetId = assetId.ToUpper();

            var definition = new AssetDefinition
            {
                Code = assetId,
                Name = name,
                Symbol = symbol,
                Precision = precision
            };

            var assets = await _assetStore.FindAll();

            uint index = (uint)assets.Count();

            var assetModel = await _openchainService.GetAssetAsync(index);

            if(assetModel == null)
            {
                assetModel = await _openchainService.CreateAssetAsync(index, definition);
            }
            else
            {
                throw new Exception($"An asset has already been defined at index '{index}'.");
            }

            var asset = new Asset
            {
                Id = assetId,
                Label = name,
                Path = assetModel.Path,
                IssuedUnits = assetModel.Balance,
                Index = (int)assetModel.Index,
                Chain = assetModel.Chain,
                AccountAddress = assetModel.AccountAddress
            };

            var result = await _assetStore.CreateAsync(asset, CancellationToken);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.FirstOrDefault()?.Description);
            }

            return asset;
        }

        public async Task<Asset> UpdateAsync(Asset asset)
        {
            await Task.Run(() => { });
            return new Asset { };
        }

        public async Task<Asset> SyncAsync(Asset asset)
        {
            asset.IssuedUnits = await _openchainService.GetAccountBalanceAsync(asset.Path, asset.Path);

            var result = await _assetStore.UpdateAsync(asset, CancellationToken);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.FirstOrDefault()?.Description);
            }

            return asset;
        }
    }
}
