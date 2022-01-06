using AutoMapper;
using EasyOC.Core.Application;
using EasyOC.DynamicWebApi.Attributes;
using EasyOC.OrchardCore.OpenApi.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentFields.Indexing.SQL;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.ContentManagement.Records;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Indexing;
using OrchardCore.Settings;
using OrchardCore.Users;
using OrchardCore.Users.Indexes;
using OrchardCore.Users.Models;
using OrchardCore.Users.Services;
using OrchardCore.Users.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using YesSql;
using YesSql.Services;
using Permissions = OrchardCore.Users.Permissions;

namespace EasyOC.OrchardCore.OpenApi.Services
{
    [EOCAuthorization("View Users")]
    public class UsersAppService : AppServcieBase, IUsersAppService
    {
        private readonly UserManager<IUser> _userManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly SignInManager<IUser> _signInManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly INotifier _notifier;
        private readonly IContentManager _contentManager;

        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersAppService(
            SignInManager<IUser> signInManager,
            IAuthorizationService authorizationService,
            UserManager<IUser> userManager,
            IUserService userService,
            INotifier notifier,
            ISiteService siteService, IMapper mapper, IContentManager contentManager, IContentDefinitionManager contentDefinitionManager)
        {
            _signInManager = signInManager;
            _authorizationService = authorizationService;
            _userManager = userManager;
            _notifier = notifier;
            _userService = userService;
            _mapper = mapper;
            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;
        }

        public async Task<PagedResult<UserListItemDto>> GetAllAsync(GetAllUserInput input)
        {
            var authUser = new User();
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ViewUsers, authUser))
            {
                throw new UnauthorizedAccessException();
            }
            var users = YesSession.Query<User, UserIndex>();
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                var normalizedSearchUserName = _userManager.NormalizeName(input.Filter);
                var normalizedSearchEMail = _userManager.NormalizeEmail(input.Filter);
                users = users.Where(u => u.NormalizedUserName.Contains(normalizedSearchUserName) || u.NormalizedEmail.Contains(normalizedSearchEMail));
                //users = users.With<TextFieldIndex>(x => x.ContentType== "UserProfiles" &&  )
            }
            if (input.HasOrder())
            {
                switch (input.SortField.ToLower())
                {
                    case "username":
                        input.SortField = "NormalizedUserName";
                        break;
                    case "email":
                        input.SortField = "NormalizedEmail";
                        break;
                    default:
                        break;
                }
                users = users.OrderBy(input.GetOrderStr());
            }
            //users = users.With<ContentItemIndex>(x => x.ContentType == "UserProfiles")
            //                  .Where(x => x.DisplayText.Contains(input.Filter));
            var count = await users.CountAsync();
            var results = await users
                .Page(input)
                .ListAsync();
            return new PagedResult<UserListItemDto>(count,
                ObjectMapper.Map<IEnumerable<UserListItemDto>>(results));
        }


        [EOCAuthorization(OCPermissions.ManageUsers)]
        public async Task BulkActionAsync(UsersBulkActionInput bulkActionInput)
        { 
            if (bulkActionInput.ItemIds?.Count() > 0)
            {
                var checkedContentItems = await YesSession.Query<User, UserIndex>()
                    .Where(x => x.UserId.IsIn(bulkActionInput.ItemIds)).ListAsync();
                switch (bulkActionInput.BulkAction)
                {
                    case UsersBulkAction.None:
                        break;
                    case UsersBulkAction.Approve:
                        foreach (var item in checkedContentItems)
                        {
                            var token = await _userManager.GenerateEmailConfirmationTokenAsync(item);
                            await _userManager.ConfirmEmailAsync(item, token);
                            await _notifier.SuccessAsync(H["User {0} successfully approved.", item.UserName]);
                        }
                        break;
                    case UsersBulkAction.Delete:
                        foreach (var item in checkedContentItems)
                        {
                            await _userManager.DeleteAsync(item);
                            await _notifier.SuccessAsync(H["User {0} successfully deleted.", item.UserName]);
                        }
                        break;
                    case UsersBulkAction.Disable:
                        foreach (var item in checkedContentItems)
                        {
                            item.IsEnabled = false;
                            await _userManager.UpdateAsync(item);
                            await _notifier.SuccessAsync(H["User {0} successfully disabled.", item.UserName]);
                        }
                        break;
                    case UsersBulkAction.Enable:
                        foreach (var item in checkedContentItems)
                        {
                            item.IsEnabled = true;
                            await _userManager.UpdateAsync(item);
                            await _notifier.SuccessAsync(H["User {0} successfully enabled.", item.UserName]);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            //return RedirectToAction("Index");
        }

        [EOCAuthorization(OCPermissions.ManageUsers)]
        public async Task NewUserAsync(UserDetailsDto input)
        {

            var user = await _userService.CreateUserAsync(ObjectMapper.Map<User>(input), null, async (key, value) =>
              {
                  await _notifier.ErrorAsync(H[$"{key}:{value}"]);
              });
            if (user != null)
            {
                await _notifier.SuccessAsync(H["User created successfully."]);
            }
            //return RedirectToAction(nameof(Index));
        }

        public async Task<UserDetailsDto> GetUserAsync(string id)
        {
            // When no id is provided we assume the user is trying to edit their own profile.
            if (String.IsNullOrEmpty(id))
            {
                id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            else
            {
                if (!await _authorizationService.AuthorizeAsync(User, CommonPermissions.ViewUsers))
                {
                    throw new AppFriendlyException("User not found.", StatusCodes.Status403Forbidden);
                }
            }

            var user = await _userManager.FindByIdAsync(id) as User;
            if (user == null)
            {
                throw new AppFriendlyException("User not found.", StatusCodes.Status403Forbidden);
            }

            return _mapper.Map<UserDetailsDto>(user);
        }



        [EOCAuthorization(OCPermissions.ManageUsers)]
        [HttpPost]
        public async Task UpdateAsync(UserDetailsDto userDto)
        {
            // When no id is provided we assume the user is trying to edit their own profile.
            var id = userDto.UserId;
            var editingOwnUser = false;
            if (String.IsNullOrEmpty(userDto.UserId))
            {
                editingOwnUser = true;
                id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageOwnUserInformation))
                {
                    throw new AppFriendlyException(SimpleError.PermissionDenied);
                }
            }

            var user = await _userManager.FindByIdAsync(id) as User;
            if (user == null)
            {
                throw new AppFriendlyException(SimpleError.ResourceNotFound);
            }

            if (!editingOwnUser && !await _authorizationService.AuthorizeAsync(User, Permissions.ViewUsers, user))
            {
                throw new AppFriendlyException(SimpleError.PermissionDenied);
            }

            _mapper.Map(userDto, user);
            var result = await _userManager.UpdateAsync(user);
            if (result.Errors.Count() == 0)
            {
                if (String.Equals(User.FindFirstValue(ClaimTypes.NameIdentifier), user.UserId, StringComparison.OrdinalIgnoreCase))
                {
                    await _signInManager.RefreshSignInAsync(user);
                }
                await _notifier.SuccessAsync(H["User updated successfully."]);

            }
            else
            {
                foreach (var error in result.Errors)
                {
                    await Notifier.ErrorAsync(H[error.Description]);
                }
            }
        }
        [EOCAuthorization(OCPermissions.ManageUsers)]
        [HttpPost]
        public async Task DeleteAsync(string id)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageUsers))
            {
                throw new UnauthorizedAccessException();

            }

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

        [EOCAuthorization(OCPermissions.ManageUsers)]
        [HttpPost]
        public async Task EditPasswordAsync(ResetUserPasswordtInput model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email) as User;

            if (user == null)
            {
                throw new AppFriendlyException($"Uesr:{user}Not Fount", StatusCodes.Status404NotFound);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var validationResult = new List<string>();
            if (await _userService.ResetPasswordAsync(model.Email, token, model.NewPassword,
                  (a, b) =>
                        {
                            validationResult.Add(H[$"{a}:{b}"].Value);
                        }
            ))
            {
                await _notifier.SuccessAsync(H["Password updated correctly."]);
            }
            if (validationResult.Count > 0)
            {
                throw new AppFriendlyException(
                    string.Join('\n', validationResult), StatusCodes.Status412PreconditionFailed);
            }
        }


        public IEnumerable<ContentTypeDefinitionDto> GetUserSettingsTypes()
        {
            return ObjectMapper.Map<IEnumerable<ContentTypeDefinitionDto>>(GetUserSettingsTypeDefinitions());
        }

        [NonDynamicMethod]
        public IEnumerable<ContentTypeDefinition> GetUserSettingsTypeDefinitions()
            => _contentDefinitionManager
                .ListTypeDefinitions()
                .Where(x => x.GetSettings<ContentTypeSettings>().Stereotype == "CustomUserSettings");

        [NonDynamicMethod]
        public async Task<ContentItem> GetUserSettingsAsync(User user, string settingsTypeName)
        {
            JToken property;
            ContentItem contentItem;
            if (user.Properties.TryGetValue(settingsTypeName, out property))
            {
                var existing = property.ToObject<ContentItem>();

                // Create a new item to take into account the current type definition.
                contentItem = await _contentManager.NewAsync(existing.ContentType);
                contentItem.Merge(existing);
            }
            else
            {
                contentItem = await _contentManager.NewAsync(settingsTypeName);
            }
            return contentItem;

        }
        //[AllowAnonymous]
        //public async Task<ContentItemDto> GetUserSettingsAsync(string userId, string settingsTypeName)
        //{
        //    if (String.IsNullOrEmpty(userId))
        //    {
        //        userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    }
        //    else
        //    {
        //        if (!await _authorizationService.AuthorizeAsync(User, CommonPermissions.ViewUsers))
        //        {
        //            throw new AppFriendlyException("Permission Denied.", StatusCodes.Status403Forbidden);
        //        }
        //    }

        //    var user = await _userManager.FindByIdAsync(userId) as User;
        //    if (user == null)
        //    {
        //        throw new AppFriendlyException("User not found.", StatusCodes.Status404NotFound);
        //    }
        //    var contentItem = await GetUserSettingsAsync(user, settingsTypeName);
        //    return contentItem.ToDto();
        //}


    }
}
