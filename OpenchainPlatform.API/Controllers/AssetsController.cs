using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenchainPlatform.API.ViewModels;
using OpenchainPlatform.Core.Managers;
using Microsoft.Extensions.Logging;
using OpenchainPlatform.API.ViewModels.Extensions;
using OpenchainPlatform.API.Constsants;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OpenchainPlatform.API.Controllers
{
    [Route("assets")]
    public class AssetsController : Controller
    {
        private readonly AccountManager _accountManager;
        private readonly AssetManager _assetManager;
        private readonly ILogger _logger;

        public AssetsController(AssetManager assetManager, AccountManager accountManager, ILoggerFactory loggerFactory)
        {
            _assetManager = assetManager;
            _accountManager = accountManager;

            _logger = loggerFactory.CreateLogger<AssetsController>();
        }

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> GetAsset(string id)
        {
            var asset = await _assetManager.FindByIdAsync(id);

            if (asset == null)
            {
                return BadRequest(new ErrorViewModel()
                {
                    Error = ErrorCode.AssetNotFound,
                    ErrorDescription = $"No asset with Id='{id}' was found."
                });
            }

            return Ok(asset.ToViewModel());
        }

        [HttpPost, Route("")]
        public async Task<IActionResult> CreateAsset([FromBody] CreateAssetViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorViewModel()
                {
                    Error = ErrorCode.InvalidRequest,
                    ErrorDescription = ModelState.Values.FirstOrDefault(m => m.ValidationState == ModelValidationState.Invalid)?.Errors.FirstOrDefault()?.ErrorMessage
                });
            }

            var asset = await _assetManager.FindByIdAsync(model.Id);

            if (asset != null)
            {
                return BadRequest(new ErrorViewModel()
                {
                    Error = ErrorCode.EntityExists,
                    ErrorDescription = $"An asset with Id='{model.Id}' already exist."
                });
            }

            asset = await _assetManager.CreateAsync(model.Id, model.Name, model.Symbol, model.Precision);

            return Ok(asset.ToViewModel());
        }
    }
}
