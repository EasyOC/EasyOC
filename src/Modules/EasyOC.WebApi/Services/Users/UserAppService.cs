using AutoMapper;
using EasyOC.Core.Application;
using EasyOC.WebApi.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.AuditTrail.Indexes;
using OrchardCore.AuditTrail.Models;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Settings;
using OrchardCore.Users;
using OrchardCore.Users.AuditTrail.Registration;
using OrchardCore.Users.AuditTrail.Services;
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

namespace EasyOC.WebApi.Services
{
    public class UserAppService : AppServcieBase, IUserAppService
    {
        private readonly UserManager<IUser> _userManager;
        private readonly SignInManager<IUser> _signInManager;
        private readonly ISession _session;
        private readonly IAuthorizationService _authorizationService;
        private readonly ISiteService _siteService;
        private readonly INotifier _notifier;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserAppService(
            SignInManager<IUser> signInManager,
            IAuthorizationService authorizationService,
            ISession session,
            UserManager<IUser> userManager,
            IUserService userService,
            INotifier notifier,
            ISiteService siteService, IMapper mapper)
        {
            _signInManager = signInManager;
            _authorizationService = authorizationService;
            _session = session;
            _userManager = userManager;
            _notifier = notifier;
            _siteService = siteService;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<PagedResultDto<UserDto>> GetAllAsync(GetAllUserInput input)
        {
            var authUser = new User();
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ViewUsers, authUser))
            {
              throw new UnauthorizedAccessException();
            }
           

            var users = YesSession.Query<User, UserIndex>();

            //switch (options.Filter)
            //{
            //    case UsersFilter.Approved:
            //        users = users.Where(u => u.RegistrationStatus == UserStatus.Approved);
            //        break;
            //    case UsersFilter.Pending:
            //        users = users.Where(u => u.RegistrationStatus == UserStatus.Pending);
            //        break;
            //    case UsersFilter.EmailPending:
            //        //users = users.Where(u => u.EmailStatus == UserStatus.Pending);
            //        break;
            //}

            if (!string.IsNullOrWhiteSpace(input.SearchText))
            {
                var normalizedSearchUserName = _userManager.NormalizeName(input.SearchText);
                var normalizedSearchEMail = _userManager.NormalizeEmail(input.SearchText);

                users = users.Where(u => u.NormalizedUserName.Contains(normalizedSearchUserName) || u.NormalizedEmail.Contains(normalizedSearchEMail));
            }

            switch (input.Order)
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
                .Skip(input.GetStartIndex())
                .Take(input.PageSize)
                .ListAsync();

            YesSession.Query<AuditTrailEvent, AuditTrailEventIndex>()
                //.IsIn<>(x=>x.)
                .Where(x => x.Category == UserRegistrationAuditTrailEventConfiguration.User
                && x.Name == UserAuditTrailEventConfiguration.Created);

            return new PagedResultDto<UserDto>(count, _mapper.Map<List<UserDto>>(results));
        }



        public async Task BulkActionAsync(UsersBulkActionInput bulkActionInput)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ViewUsers))
            {
                throw new UnauthorizedAccessException();
            }

            if (bulkActionInput.ItemIds?.Count() > 0)
            {
                var checkedContentItems = await _session.Query<User, UserIndex>()
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


        [HttpPost]
        public async Task CreateUserAsync(UserDto input)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageUsers))
            {
                throw new UnauthorizedAccessException();
            }

            var user = await _userService.CreateUserAsync(_mapper.Map<User>(input), null, async (key, value) =>
              {
                  await _notifier.ErrorAsync(H[$"{key}:{value}"]);
              });
            if (user != null)
            {
                await _notifier.SuccessAsync(H["User created successfully."]);
            }
            //return RedirectToAction(nameof(Index));
        }

        public async Task<UserDto> GetUserAsync(string id)
        {
            // When no id is provided we assume the user is trying to edit their own profile.
            if (String.IsNullOrEmpty(id))
            {
                id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageOwnUserInformation))
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else
            {
                if (!await _authorizationService.AuthorizeAsync(User, CommonPermissions.ViewUsers))
                {
                    throw new EntryPointNotFoundException();
                }
            }

            var user = await _userManager.FindByIdAsync(id) as User;
            if (user == null)
            {
                throw new EntryPointNotFoundException();
            }

            return _mapper.Map<UserDto>(user);
        }

        [HttpPost]
        public async Task UpdateAsync(UserDto userDto)
        {
            string id;
            // When no id is provided we assume the user is trying to edit their own profile.
            if (userDto.Id.HasValue)
            {
                id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageOwnUserInformation))
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageUsers))
            {
                throw new EntryPointNotFoundException();
            }

            var user = await _userManager.FindByIdAsync(userDto.Id.ToString()) as User;
            if (user == null)
            {
                throw new ArgumentException($"Uesr:{user}Not Fount");
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



        [HttpPost]
        public async Task EditPasswordAsync(ResetUserPasswordtInput model)
        {
            if (!await _authorizationService.AuthorizeAsync(User, Permissions.ManageUsers))
            {
                throw new UnauthorizedAccessException("Not Fount");

            }

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
            }
        }


    }
}
