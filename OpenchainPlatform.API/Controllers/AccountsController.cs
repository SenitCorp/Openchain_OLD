using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenchainPlatform.API.ViewModels;
using System.ComponentModel.DataAnnotations;
using OpenchainPlatform.Core.Managers;
using Microsoft.Extensions.Logging;
using OpenchainPlatform.Core.Data;
using OpenchainPlatform.Core.EntityFrameworkCore;
using OpenchainPlatform.API.ViewModels.Extensions;
using OpenchainPlatform.API.Constsants;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OpenchainPlatform.API.Controllers
{
    [Route("accounts")]
    public class AccountsController : Controller
    {
        private readonly AccountManager _accountManager;
        private readonly AccountAddressManager _accountAddressManager;
        private readonly AssetManager _assetManager;
        private readonly ILogger _logger;

        public AccountsController(AccountManager accountManager, AccountAddressManager accountAddressManager, AssetManager assetManager, ILoggerFactory loggerFactory)
        {
            _accountManager = accountManager;
            _accountAddressManager = accountAddressManager;
            _assetManager = assetManager;

            _logger = loggerFactory.CreateLogger<AccountsController>();
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await _accountManager.FindAll();
            return Ok(accounts.ToList().ToViewModel());
        }

        [HttpGet, Route("{accountId}")]
        public async Task<IActionResult> GetAccount(string accountId)
        {
            var account = await _accountManager.FindByIdAsync(accountId);

            if (account == null)
            {
                return NotFound(new ErrorViewModel()
                {
                    Error = ErrorCode.AccountNotFound,
                    ErrorDescription = $"No account with Id='{accountId}' was found."
                });
            }

            return Ok(account.ToViewModel());
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorViewModel()
                {
                    Error = ErrorCode.InvalidRequest,
                    ErrorDescription = ModelState.Values.FirstOrDefault(m => m.ValidationState == ModelValidationState.Invalid)?.Errors.FirstOrDefault()?.ErrorMessage
                });
            }

            var asset = await _assetManager.FindByIdAsync(model.AssetId);

            if (asset == null)
            {
                return NotFound(new ErrorViewModel()
                {
                    Error = ErrorCode.AssetNotFound,
                    ErrorDescription = $"No asset with Id='{model.AssetId}' was found."
                });
            }

            var account = await _accountManager.CreateAsync(model.Passphrase, asset, model.Label);

            return Ok(account.ToViewModel());
        }

        [HttpPost, Route("addresses")]
        public async Task<IActionResult> CreateAccountAddress([FromBody] CreateAccountAddressViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorViewModel()
                {
                    Error = ErrorCode.InvalidRequest,
                    ErrorDescription = ModelState.Values.FirstOrDefault(m => m.ValidationState == ModelValidationState.Invalid)?.Errors.FirstOrDefault()?.ErrorMessage
                });
            }

            var account = await _accountManager.FindByIdAsync(model.AccountId);

            if (account == null)
            {
                return NotFound(new ErrorViewModel()
                {
                    Error = ErrorCode.AccountNotFound,
                    ErrorDescription = $"No account with Id='{model.AccountId}' was found."
                });
            }

            var address = await _accountAddressManager.CreateAsync(account, model.Passphrase);

            return Ok(address.ToViewModel());
        }

        [HttpGet, Route("addresses")]
        public async Task<IActionResult> GetAccountAddresses()
        {
            var addresses = await _accountAddressManager.FindAll();

            return Ok(addresses.ToList().ToViewModel());
        }
    }
}
