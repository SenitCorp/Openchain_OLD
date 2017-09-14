using OpenchainPlatform.Core.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenchainPlatform.Core.Data.Models;
using OpenchainPlatform.Core.Data;
using System.Threading;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace OpenchainPlatform.Core.Managers
{
    public class AccountManager
    {
        private readonly OpenchainService _openchainService;
        private readonly ILogger _logger;
        private readonly IAccountStore _accountStore;
        private readonly HttpContext _context;
        private CancellationToken CancellationToken => _context?.RequestAborted ?? CancellationToken.None;

        public AccountManager(IAccountStore accountStore, OpenchainService openchainService, IHttpContextAccessor contextAccessor, ILoggerFactory loggerFactory)
        {
            _accountStore = accountStore;
            _openchainService = openchainService;
            _context = contextAccessor?.HttpContext;

            _logger = loggerFactory.CreateLogger<AccountManager>();
        }

        public async Task<Account> CreateAsync(string passphrase, Asset asset, string label)
        {
            var accountModel = await _openchainService.CreateAccountAsync(passphrase, asset.Path);

            var account = new Account
            {
                Id = accountModel.Base58Address,
                Alias = label,
                Balance = accountModel.Balance,
                Path = accountModel.Path,
                Seed = accountModel.Seed,
                Chain = accountModel.Chain,
                AssetId = asset.Id
            };

            var result = await _accountStore.CreateAsync(account, CancellationToken);

            if(!result.Succeeded)
            {
                throw new Exception(result.Errors.FirstOrDefault()?.Description);
            }

            return account;
        }

        public async Task<Account> FindByIdAsync(string accountId)
        {
            var account = await _accountStore.FindByIdAsync(accountId);

            return account;
        }

        public async Task<Account> SyncAsync(Account account, string assetPath)
        {
            account.Balance = await _openchainService.GetAccountBalanceAsync(account.Path, assetPath);

            var result = await _accountStore.UpdateAsync(account, CancellationToken);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.FirstOrDefault()?.Description);
            }

            return account;
        }

        public async Task<ICollection<Account>> FindAll()
        {
            return await _accountStore.FindAll();
        }
    }
}
