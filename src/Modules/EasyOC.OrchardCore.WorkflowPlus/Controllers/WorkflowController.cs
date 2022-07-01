using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrchardCore.Deployment;
using OrchardCore.Deployment.Core.Services;
using OrchardCore.Deployment.Services;
using OrchardCore.Recipes.Models;
using OrchardCore.Routing;
using OrchardCore.Workflows;
using OrchardCore.Workflows.Services;
using OrchardCore.Workflows.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using DeploymentPermissions = OrchardCore.Deployment.Permissions;

namespace EasyOC.OrchardCore.WorkflowPlus.Controllers
{
    public class WorkflowController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IDeploymentManager _deploymentManager;
        private readonly IWorkflowTypeStore _workflowTypeStore;

        public WorkflowController(IAuthorizationService authorizationService, IDeploymentManager deploymentManager,
            IWorkflowTypeStore workflowTypeStore)
        {
            _authorizationService = authorizationService;
            _deploymentManager = deploymentManager;
            _workflowTypeStore = workflowTypeStore;
        }

        [HttpGet]
        public async Task<IActionResult> BulkExport([FromQuery] string ids)
        {
            if (!await _authorizationService.AuthorizeAsync(User, DeploymentPermissions.Export))
            {
                return Forbid();
            }

            string archiveFileName;
            var itemIds = ids.Split(',').Select(x => int.Parse(x));
            using (var fileBuilder = new TemporaryFileBuilder())
            {
                 archiveFileName = fileBuilder.Folder + ".zip";
                 var recipeDescriptor = new RecipeDescriptor();
                var deploymentPlanResult = new DeploymentPlanResult(fileBuilder, recipeDescriptor);
                var data = new JArray();
                deploymentPlanResult.Steps.Add(new JObject(
                    new JProperty("name", "WorkflowType"),
                    new JProperty("data", data)
                ));
                //Do filter
                foreach (var workflow in await _workflowTypeStore.GetAsync(itemIds))
                {
                    var objectData = JObject.FromObject(workflow);

                    // Don't serialize the Id as it could be interpreted as an updated object when added back to YesSql
                    objectData.Remove(nameof(workflow.Id));
                    data.Add(objectData);
                }

                await deploymentPlanResult.FinalizeAsync();
                // ZipFile.CreateFromDirectory(fileBuilder.Folder, archiveFileName);
                //TODO Export files to a zip file
                return new PhysicalFileResult(Path.Combine(fileBuilder.Folder,"Recipe.json"), "application/json")
                {
                    FileDownloadName = "Recipe.json"
                };
            }

            
        }
    }
}