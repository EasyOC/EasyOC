using AutoMapper;
using OrchardCore.Users.Models;
using System;
using System.Collections.Generic;

namespace EasyOC.OrchardCore.OpenApi.Dto
{
    [AutoMap(typeof(User))]
    public class UserListItemDto
    {
        public int? Id { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string Email { get; set; }
 

        public bool EmailConfirmed { get; set; }

        public bool IsEnabled { get; set; } = true;


        public bool IsLockoutEnabled { get; set; }

        public DateTime? LockoutEndUtc { get; set; }

        public int AccessFailedCount { get; set; }

        //public string ResetToken
        //{
        //    get;
        //    set;
        //}

        public IList<string> RoleNames { get; set; } = new List<string>();


        public IList<UserClaim> UserClaims { get; set; } = new List<UserClaim>();


        //public IList<UserLoginInfo> LoginInfos
        //{
        //    get;
        //    set;
        //} = new List<UserLoginInfo>();


        //public IList<UserToken> UserTokens
        //{
        //    get;
        //    set;
        //} = new List<UserToken>();


        public override string ToString()
        {
            return UserName;
        }
    }
}
