using Microsoft.AspNetCore.Authorization;
using OrchardCore.Security;
using OrchardCore.Security.AuthorizationHandlers;
using System.Threading.Tasks;

namespace EasyOC.Infrastructure.Security;

public class EocPermissionHandler : PermissionHandler
{
    private readonly IPermissionGrantingService _permissionGrantingService;

    public EocPermissionHandler(IPermissionGrantingService permissionGrantingService) : base(permissionGrantingService)
    {
        _permissionGrantingService = permissionGrantingService;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {

        if (context.HasSucceeded || !(context?.User?.Identity?.IsAuthenticated ?? false))
        {
            return Task.CompletedTask;
        }
        else if (_permissionGrantingService.IsGranted(requirement, context.User.Claims))
        {
            context.Succeed(requirement);
        }

        return base.HandleRequirementAsync(context, requirement);
    }
}
