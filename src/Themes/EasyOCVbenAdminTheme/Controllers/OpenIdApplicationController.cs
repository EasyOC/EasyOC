using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OpenIddict.Abstractions;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Environment.Shell.Descriptor.Models;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.OpenId;
using OrchardCore.OpenId.Abstractions.Descriptors;
using OrchardCore.OpenId.Abstractions.Managers;
using OrchardCore.OpenId.Services;
using OrchardCore.OpenId.Settings;
using OrchardCore.OpenId.ViewModels;
using OrchardCore.OpenId.YesSql.Indexes;
using OrchardCore.Security.Services;
using OrchardCore.Settings;
using YesSql;

namespace EasyOCVbenAdminTheme.Controllers
{
    [Admin, Feature(OpenIdConstants.Features.Management)]
    public class OpenIdApplicationController:   Controller
    {
        private readonly ISession _session;
        public OpenIdApplicationController(ISession session)
        {
            _session = session;
        }

        public async Task<IActionResult> GetApplicatinPermissionSettings(string identifer)
        {
            _session.Query<OpenIdDocument,OpenIdApplicationIndex>()
            return null;
        }

    }
}
