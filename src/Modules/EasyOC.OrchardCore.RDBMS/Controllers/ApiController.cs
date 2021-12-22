using EasyOC.OrchardCore.RDBMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using OrchardCore.Deployment.Services;
using OrchardCore.Mvc.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp;

namespace OrchardCore.RelationDb.Controllers
{
    [Route("api/Deployment")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Api"), IgnoreAntiforgeryToken, AllowAnonymous]
    public class ApiController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        private IStringLocalizer S;
        private readonly IDeploymentManager _deploymentManager;
        public ApiController(
            IAuthorizationService authorizationService
, IDeploymentManager deploymentManager, IStringLocalizer<ApiController> s)
        {
            _authorizationService = authorizationService;
            _deploymentManager = deploymentManager;
            S = s;
        }
        [HttpPost("ImportPackage")]
        public async Task<bool> ImportDeploymentPackageAsync([FromBody] ImportJsonInupt model)
        {
            if (!model.Json.IsJson())
            {
                throw new UserFriendlyException(S["The recipe is written in an incorrect json format."]);
            }

            var tempArchiveFolder = PathExtensions.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                Directory.CreateDirectory(tempArchiveFolder);
                System.IO.File.WriteAllText(Path.Combine(tempArchiveFolder, "Recipe.json"), model.Json);

                await _deploymentManager.ImportDeploymentPackageAsync(new PhysicalFileProvider(tempArchiveFolder));


            }
            finally
            {
                if (Directory.Exists(tempArchiveFolder))
                {
                    Directory.Delete(tempArchiveFolder, true);
                }
            }
            return true;
        }
    }
}



