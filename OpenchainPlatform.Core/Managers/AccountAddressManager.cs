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
using OpenchainPlatform.Core.Services.Models;

namespace OpenchainPlatform.Core.Managers
{
    public class AccountAddressManager
    {
        private readonly OpenchainService _openchainService;
        private readonly ILogger _logger;
        private readonly IAccountAddressStore _accountAddressStore;
        private readonly HttpContext _context;
        private CancellationToken CancellationToken => _context?.RequestAborted ?? CancellationToken.None;

        public AccountAddressManager(IAccountAddressStore accountAddressStore, OpenchainService openchainService, IHttpContextAccessor contextAccessor, ILoggerFactory loggerFactory)
        {
            _accountAddressStore = accountAddressStore;
            _openchainService = openchainService;
            _context = contextAccessor?.HttpContext;

            _logger = loggerFactory.CreateLogger<AccountManager>();
        }

        public async Task<AccountAddress> CreateAsync(Account account, string passphrase)
        {
            var accountCredentials = new AccountCredentialsModel
            {
                Seed = account.Seed,
                Passphrase = passphrase,
                Index = 0,
                Chain = account.Chain
            };

            uint index = 0;

            var addresses = await _accountAddressStore.FindAll();

            var last = addresses.OrderByDescending(a => a.Index).Take(1).FirstOrDefault();

            if(last != null)
            {
                index = last.Index + 1;
            }

            var accountModel = await _openchainService.CreateAccountAddressAsync(accountCredentials, index);

            var accountAddress = new AccountAddress
            {
                AccountId = account.Id,
                Address = accountModel.Base58Address,
                Chain = accountModel.Chain,
                Index = accountModel.Index
            };

            var result = await _accountAddressStore.CreateAsync(accountAddress, CancellationToken);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.FirstOrDefault()?.Description);
            }

            return accountAddress;
        }

        public async Task<AccountAddress> FindByAddressAsync(string accountId)
        {
            var account = await _accountAddressStore.FindByAddressAsync(accountId);

            return account;
        }

        public async Task<ICollection<AccountAddress>> FindAll()
        {
            return await _accountAddressStore.FindAll();
        }
    }
}
