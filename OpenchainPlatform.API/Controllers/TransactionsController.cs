using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenchainPlatform.API.ViewModels;
using System.ComponentModel.DataAnnotations;
using OpenchainPlatform.Core.Managers;
using Microsoft.Extensions.Logging;
using OpenchainPlatform.Core.Data.Models;
using OpenchainPlatform.API.ViewModels.Extensions;
using OpenchainPlatform.API.Constsants;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OpenchainPlatform.API.Controllers
{
    [Route("transactions")]
    public class TransactionsController : Controller
    {
        private readonly TransactionManager _transactionManager;
        private readonly ILogger _logger;
        private readonly AccountManager _accountManager;
        private readonly AccountAddressManager _accountAddressManager;
        private readonly AssetManager _assetManager;

        public TransactionsController(TransactionManager transactionManager, AccountManager accountManager, AccountAddressManager accountAddressManager, AssetManager assetManager, ILoggerFactory loggerFactory)
        {
            _transactionManager = transactionManager;
            _accountManager = accountManager;
            _accountAddressManager = accountAddressManager;
            _assetManager = assetManager;

            _logger = loggerFactory.CreateLogger<TransactionsController>();
        }

        [HttpGet, Route("{transactionId}")]
        public async Task<IActionResult> GetTransaction(string transactionId)
        {
            var transaction = await _transactionManager.FindByIdAsync(transactionId);

            if (transaction == null)
            {
                return NotFound(new ErrorViewModel()
                {
                    Error = ErrorCode.TransactionNotFound,
                    ErrorDescription = $"No transaction with Id='{transactionId}' was found."
                });
            }

            return Ok(transaction.ToViewModel());
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> TransferAsset([FromBody] CreateTransactionViewModel model)
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

            var account = await _accountManager.FindByIdAsync(model.AccountId);

            if (account == null)
            {
                return NotFound(new ErrorViewModel()
                {
                    Error = ErrorCode.AccountNotFound,
                    ErrorDescription = $"No account with Id='{model.AccountId}' was found."
                });
            }

            var recipients = new List<KeyValuePair<Account, long>>();

            foreach (var recipient in model.Recipients)
            {
                var recipientAccount = await _accountManager.FindByIdAsync(recipient.AccountId);

                if (recipientAccount == null)
                {

                    var accountAddress = await _accountAddressManager.FindByAddressAsync(recipient.AccountId);

                    if (accountAddress == null)
                    {
                        return NotFound(new ErrorViewModel()
                        {
                            Error = ErrorCode.AccountNotFound,
                            ErrorDescription = $"No account with Id='{recipient.AccountId}' was found."
                        });
                    }

                    recipientAccount = accountAddress.Account;
                }

                recipients.Add(new KeyValuePair<Account, long>(recipientAccount, recipient.Amount));
            }

            var transaction = await _transactionManager.TransferAssetAsync(account, model.Passphrase, recipients, asset);

            return Ok(transaction.ToViewModel());
        }

        [HttpPost, Route("issue_asset")]
        public async Task<IActionResult> IssueAsset([FromBody] IssueAssetViewModel model)
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

            var account = await _accountManager.FindByIdAsync(model.AccountId);

            if (account == null)
            {
                return NotFound(new ErrorViewModel()
                {
                    Error = ErrorCode.AccountNotFound,
                    ErrorDescription = $"No account with Id='{model.AccountId}' was found."
                });
            }

            var transaction = await _transactionManager.IssueAssetAsync(account, asset, model.Amount);

            return Ok(transaction.ToViewModel());
        }

        [HttpPost, Route("retire_asset")]
        public async Task<IActionResult> RetireAsset([FromBody] RetireAssetViewModel model)
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

            var account = await _accountManager.FindByIdAsync(model.AccountId);

            if (account == null)
            {
                return NotFound(new ErrorViewModel()
                {
                    Error = ErrorCode.AccountNotFound,
                    ErrorDescription = $"No account with Id='{model.AccountId}' was found."
                });
            }

            var transaction = await _transactionManager.RetireAssetAsync(account, asset, model.Amount);

            return Ok(transaction.ToViewModel());
        }
    }
}
