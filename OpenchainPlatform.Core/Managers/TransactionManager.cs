using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OpenchainPlatform.Core.Data;
using OpenchainPlatform.Core.Data.Models;
using OpenchainPlatform.Core.Services;
using OpenchainPlatform.Core.Services.Models;
using OpenchainPlatform.Core.Services.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenchainPlatform.Core.Managers
{
    public class TransactionManager
    {
        private readonly OpenchainService _openchainService;
        private readonly ILogger _logger;
        private readonly ITransactionStore _transactionStore;
        private readonly HttpContext _context;
        private readonly AssetManager _assetManager;
        private readonly AccountManager _accountManager;

        private CancellationToken CancellationToken => _context?.RequestAborted ?? CancellationToken.None;

        public TransactionManager(ITransactionStore transactionStore, AccountManager accountManager, AssetManager assetManager, OpenchainService openchainService, IHttpContextAccessor contextAccessor, ILoggerFactory loggerFactory)
        {
            _transactionStore = transactionStore;
            _openchainService = openchainService;
            _accountManager = accountManager;
            _assetManager = assetManager;

            _context = contextAccessor?.HttpContext;

            _logger = loggerFactory.CreateLogger<TransactionManager>();
        }

        public async Task<Transaction> FindByIdAsync(string transactionId)
        {
            var transaction = await _transactionStore.FindByIdAsync(transactionId);

            return transaction;
        }

        public async Task<Transaction> IssueAssetAsync(Account account, Asset asset, long amount)
        {
            var transactionModel = await _openchainService.IssueAssetAsync(account.Path, new AssetModel
            {
                Path = asset.Path,
                Index = (uint)asset.Index
            }, amount);

            var transaction = transactionModel.ToEntity();

            var result = await _transactionStore.CreateAsync(transaction, CancellationToken);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.FirstOrDefault()?.Description);
            }

            await _assetManager.SyncAsync(asset);
            await _accountManager.SyncAsync(account, asset.Path);

            return transaction;
        }

        public async Task<Transaction> RetireAssetAsync(Account account, Asset asset, long amount)
        {
            var accountBalance = await _openchainService.GetAccountBalanceAsync(account.Path, asset.Path);

            if (accountBalance < amount)
            {
                throw new Exception("InsufficientBalance");
            }

            var transactionModel = await _openchainService.RetireAssetAsync(account.Path, new AssetModel
            {
                Path = asset.Path,
                Index = (uint)asset.Index
            }, amount);
            
            var transaction = transactionModel.ToEntity();

            var result = await _transactionStore.CreateAsync(transaction, CancellationToken);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.FirstOrDefault()?.Description);
            }

            await _assetManager.SyncAsync(asset);
            await _accountManager.SyncAsync(account, asset.Path);

            return transaction;
        }

        public async Task<Transaction> TransferAssetAsync(Account account, string passphrase,List< KeyValuePair<Account, long>> recipients, Asset asset)
        {
            var sum = recipients.ToList().Sum(r => r.Value);

            var accountBalance = await _openchainService.GetAccountBalanceAsync(account.Path, asset.Path);

            if (accountBalance < sum)
            {
                throw new Exception("InsufficientBalance");
            }

            var accountCredentials = new AccountCredentialsModel
            {
                Seed = account.Seed,
                Passphrase = passphrase,
                Index = 0,
                Chain = account.Chain
            };

            var recipientEntries = recipients.ToList().Select(r =>
            {
                return new TransactionEntryModel { Path = r.Key.Path, Value = r.Value };
            }).ToList();

            var transactionModel = await _openchainService.TransferAssetAsync(accountCredentials, recipientEntries, asset.Path);

            var transaction = transactionModel.ToEntity();

            var result = await _transactionStore.CreateAsync(transaction, CancellationToken);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.FirstOrDefault()?.Description);
            }

            await _assetManager.SyncAsync(asset);

            foreach (var recipient in recipients.ToList())
            {
                await _accountManager.SyncAsync(recipient.Key, asset.Path);
            }

            await _accountManager.SyncAsync(account, asset.Path);

            return transaction;
        }
    }
}
