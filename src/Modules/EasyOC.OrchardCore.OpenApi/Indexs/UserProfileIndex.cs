using AutoMapper;
using EasyOC.Core.Indexs;
using EasyOC.OrchardCore.OpenApi.Model;
using OrchardCore.ContentManagement;
using OrchardCore.Entities;
using OrchardCore.Users.Models;
using YesSql.Indexes;
namespace EasyOC.OrchardCore.OpenApi.Indexs
{
    [AutoMap(typeof(UserProfilePart), ReverseMap = true)]
    [AutoMap(typeof(ContentItem), ReverseMap = true)]
    [AutoMap(typeof(User), ReverseMap = true)]
    [EOCIndex("IDX_UserProfileIndex_DocumentId", "DocumentId,NickName,UserId,Username,FirstName,LastName,Gender,DepartmentId")]
    [EOCTable]
    public class UserProfileIndex : FreeSqlDocumentIndex
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public string EmployeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string NickName { get; set; }
        public string Department { get; set; } 
        public string Manager { get; set; }
    }
    public class UserProfileIndexProvider : IndexProvider<User>
    {
        public readonly IMapper _mapper;

        public UserProfileIndexProvider(IMapper mapper)
        {
            _mapper = mapper;
        }

        public override void Describe(DescribeContext<User> context)
        {
            context.For<UserProfileIndex>().Map(user =>
            {
                var profileIndex = _mapper.Map<UserProfileIndex>(user);
                var profiles = user.As<ContentItem>("UserProfile");

                if (profiles != null)
                {
                    profileIndex = _mapper.Map(profiles, profileIndex);

                    var userProfile = profiles.As<UserProfilePart>();
                    profileIndex = _mapper.Map(userProfile, profileIndex);

                    return profileIndex;
                }
                return null;
            });
        }
    }

}
