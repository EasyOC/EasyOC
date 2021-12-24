using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EasyOC.Core.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Navigation;
using OrchardCore.Routing;
using OrchardCore.Settings;
using OrchardCore.Users.Indexes;
using OrchardCore.Users.Models;
using OrchardCore.Users.Services;
using YesSql;
using YesSql.Services;
using System.Linq.Dynamic.Core;
using EasyOC.WebApi.Dto;
using OrchardCore.Users.ViewModels;
using OrchardCore.ContentManagement.GraphQL;
using OrchardCore.Users;

namespace EasyOC.WebApi.Services
{
    public class UserAppService : AppServcieBase, IUserAppService
    {
        private readonly UserManager<IUser> _userManager;
        private readonly SignInManager<IUser> _signInManager;
        private readonly ISession _session;
        private readonly IAuthorizationService _authorizationService;
        private readonly ISiteService _siteService;
        private readonly IDisplayManager<User> _userDisplayManager;
        private readonly INotifier _notifier;
        private readonly IUserService _userService;
        private readonly IUpdateModelAccessor _updateModelAccessor;

        private readonly dynamic New;
        private readonly IHtmlLocalizer H;
        private readonly IStringLocalizer S;

        public UserAppService(
            IDisplayManager<User> userDisplayManager,
            SignInManager<IUser> signInManager,
            IAuthorizationService authorizationService,
            ISession session,
            UserManager<IUser> userManager,
            IUserService userService,
            INotifier notifier,
            ISiteService siteService,
            IShapeFactory shapeFactory,
            IHtmlLocalizer<UserAppService> htmlLocalizer,
            IStringLocalizer<UserAppService> stringLocalizer,
            IUpdateModelAccessor updateModelAccessor)
        {
            _userDisplayManager = userDisplayManager;
            _signInManager = signInManager;
            _authorizationService = authorizationService;
            _session = session;
            _userManager = userManager;
            _notifier = notifier;
            _siteService = siteService;
            _userService = userService;
            _updateModelAccessor = updateModelAccessor;

            New = shapeFactory;
            H = htmlLocalizer;
            S = stringLocalizer;
        }


        public async Task<PagedResultDto<User>> Index(UserIndexOptions options, PagerParameters pagerParameters)
        {
            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageUsers))
            //{
            //    return Forbid();
            //}

            var siteSettings = await _siteService.GetSiteSettingsAsync();
            var pager = new Pager(pagerParameters, siteSettings.PageSize);

            var users = _session.Query<User, UserIndex>();

            switch (options.Filter)
            {
                case UsersFilter.Approved:
                    //users = users.Where(u => u.RegistrationStatus == UserStatus.Approved);
                    break;
                case UsersFilter.Pending:
                    //users = users.Where(u => u.RegistrationStatus == UserStatus.Pending);
                    break;
                case UsersFilter.EmailPending:
                    //users = users.Where(u => u.EmailStatus == UserStatus.Pending);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(options.SearchText))
            {
                var normalizedSearchUserName = _userManager.NormalizeName(options.SearchText);
                var normalizedSearchEMail = _userManager.NormalizeEmail(options.SearchText);

                users = users.Where(u => u.NormalizedUserName.Contains(normalizedSearchUserName) || u.NormalizedEmail.Contains(normalizedSearchEMail));
            }

            switch (options.Order)
            {
                case UsersOrder.Name:
                    users = users.OrderBy(u => u.NormalizedUserName);
                    break;
                case UsersOrder.Email:
                    users = users.OrderBy(u => u.NormalizedEmail);
                    break;

            }

            var count = await users.CountAsync();

            var results = await users
                .Skip(pager.GetStartIndex())
                .Take(pager.PageSize)
                .ListAsync();


            return new PagedResultDto<User>(count, results);
        }



        [HttpPost]
        public async Task BulkAction(UserIndexOptions options, IEnumerable<string> itemIds)
        {
            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageUsers))
            //{
            //    return Forbid();
            //}

            if (itemIds?.Count() > 0)
            {
                var checkedContentItems = await _session.Query<User, UserIndex>().Where(x => x.UserId.IsIn(itemIds)).ListAsync();
                switch (options.BulkAction)
                {
                    case UsersBulkAction.None:
                        break;
                    case UsersBulkAction.Approve:
                        foreach (var item in checkedContentItems)
                        {
                            var token = await _userManager.GenerateEmailConfirmationTokenAsync((IUser)item);
                            await _userManager.ConfirmEmailAsync((IUser)item, token);
                            await _notifier.SuccessAsync(H["User {0} successfully approved.", item.UserName]);
                        }
                        break;
                    case UsersBulkAction.Delete:
                        foreach (var item in checkedContentItems)
                        {
                            await _userManager.DeleteAsync((IUser)item);
                            _notifier.Success(H["User {0} successfully deleted.", item.UserName]);
                        }
                        break;
                    case UsersBulkAction.Disable:
                        foreach (var item in checkedContentItems)
                        {
                            item.IsEnabled = false;
                            await _userManager.UpdateAsync((IUser)item);
                            await _notifier.SuccessAsync(H["User {0} successfully disabled.", item.UserName]);
                        }
                        break;
                    case UsersBulkAction.Enable:
                        foreach (var item in checkedContentItems)
                        {
                            item.IsEnabled = true;
                            await _userManager.UpdateAsync((IUser)item);
                            await _notifier.SuccessAsync(H["User {0} successfully enabled.", item.UserName]);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            //return RedirectToAction("Index");
        }
        public async Task<IShape> Create()
        {
            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageUsers))
            //{
            //    return Forbid();
            //}

            var shape = await _userDisplayManager.BuildEditorAsync(new User(), updater: _updateModelAccessor.ModelUpdater, isNew: true);

            return shape;
        }

        [HttpPost]
        [ActionName(nameof(Create))]
        public async Task CreatePost(User user)
        {
            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageUsers))
            //{
            //    return Forbid();
            //}

            await _userService.CreateUserAsync(user, null, null);



            await _notifier.SuccessAsync(H["User created successfully."]);

            //return RedirectToAction(nameof(Index));
        }

        public async Task<User> GetUserAsync(string id)
        {
            // When no id is provided we assume the user is trying to edit their own profile.
            if (String.IsNullOrEmpty(id))
            {
                id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageOwnUserInformation))
                //{
                //    return Forbid();
                //}
            }
            else
            {
                if (!await _authorizationService.AuthorizeAsync(User, Permissions.ApiViewContent))
                {
                    throw new ArgumentException("Not Fount");
                }
            }

            var user = await _userManager.FindByIdAsync(id) as User;
            if (user == null)
            {
                throw new ArgumentException("Not Fount");
            }

            return user;
        }

        [HttpPost]
        //[ActionName(nameof(Edit))]
        public async Task EditPost(string id, string returnUrl)
        {
            // When no id is provided we assume the user is trying to edit their own profile.
            var editingOwnUser = false;
            if (String.IsNullOrEmpty(id))
            {
                editingOwnUser = true;
                id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageOwnUserInformation))
                //{
                //    return Forbid();
                //}
            }
            //else if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageUsers))
            //{
            //    return Forbid();
            //}

            var user = await _userManager.FindByIdAsync(id) as User;
            if (user == null)
            {
                throw new ArgumentException($"Uesr:{user}Not Fount");
            }

            var shape = await _userDisplayManager.UpdateEditorAsync(user, updater: _updateModelAccessor.ModelUpdater, isNew: false);

            //if (!ModelState.IsValid)
            //{
            //    return shape;
            //}

            var result = await _userManager.UpdateAsync(user);

            foreach (var error in result.Errors)
            {
                await Notifier.ErrorAsync(H[error.Description]);
            }

            //if (!ModelState.IsValid)
            //{
            //    return shape;
            //}

            if (String.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier), user.UserId, StringComparison.OrdinalIgnoreCase))
            {
                await _signInManager.RefreshSignInAsync(user);
            }

            await _notifier.SuccessAsync(H["User updated successfully."]);

            //if (editingOwnUser)
            //{
            //    if (!String.IsNullOrEmpty(returnUrl))
            //    {
            //        return LocalRedirect(returnUrl);
            //    }

            //    return RedirectToAction(nameof(Edit));
            //}
            //else
            //{
            //    return RedirectToAction(nameof(Index));
            //}
        }

        [HttpPost]
        public async Task Delete(string id)
        {
            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageUsers))
            //{
            //    return Forbid();
            //}

            var user = await _userManager.FindByIdAsync(id) as User;

            if (user == null)
            {
                throw new ArgumentException($"Uesr:{user}Not Fount");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                await _notifier.SuccessAsync(H["User deleted successfully."]);
            }
            else
            {
                //_session.Cancel();

                await _notifier.ErrorAsync(H["Could not delete the user."]);

                foreach (var error in result.Errors)
                {
                    await _notifier.ErrorAsync(H[error.Description]);
                }
            }

            //return RedirectToAction(nameof(Index));
        }

        public async Task EditPassword(string id)
        {
            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageUsers))
            //{
            //    return Forbid();
            //}

            var user = await _userManager.FindByIdAsync(id) as User;

            if (user == null)
            {
                throw new ArgumentException($"Uesr:{user}Not Fount");
            }
        }

        [HttpPost]
        public async Task EditPassword(UserDto model)
        {
            //if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageUsers))
            //{
            //    return Forbid();
            //}

            var user = await _userManager.FindByEmailAsync(model.Email) as User;

            if (user == null)
            {
                throw new ArgumentException($"Uesr:{user}Not Fount");
            }

            //if (ModelState.IsValid)
            //{
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (await _userService.ResetPasswordAsync(model.Email, token, model.NewPassword,
                async (a, b) =>
                        { 
                            await _notifier.ErrorAsync(H[$"{a}:{b}"]);
                        }

            ))
            {
                await _notifier.SuccessAsync(H["Password updated correctly."]);
                //return RedirectToAction(nameof(Index));
            }
            //}
        }


    }
}
