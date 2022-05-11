
using EasyOC.Core.Indexs;
using FreeSql.DataAnnotations;
using EasyOC.OrchardCore.DynamicTypeIndex.Index;

// 此代码由程序生成，复制到代码文件后请更新命名空间，
// 或者在命名空间处点击 Alt+Enter 自动更新命名空间
namespace EasyOC.OrchardCore.OpenApi.Indexs
{
    [EOCIndex("IDX_{tablename}_DocumentId", "ContentItemId,DocumentId")]
    [EOCTable(Name = "UserProfile_DIndex")]
    public class UserProfileDIndex : DIndexBase
    {

        [Column(Name = "UserProfilePart_NickName", IsNullable = true, StringLength = -1)]
        public string UserProfilePartNickName { get; set; }

        [Column(Name = "UserProfilePart_FirstName", IsNullable = true, StringLength = -1)]
        public string UserProfilePartFirstName { get; set; }

        [Column(Name = "UserProfilePart_LastName", IsNullable = true, StringLength = -1)]
        public string UserProfilePartLastName { get; set; }

        [Column(Name = "UserProfilePart_Gender", IsNullable = true, StringLength = -1)]
        public string UserProfilePartGender { get; set; }

        [Column(Name = "UserProfilePart_JobTitle", IsNullable = true, StringLength = -1)]
        public string UserProfilePartJobTitle { get; set; }

        [Column(Name = "UserProfilePart_Department", IsNullable = true, StringLength = 26)]
        public string UserProfilePartDepartment { get; set; }

        [Column(Name = "UserProfilePart_Manager", IsNullable = true, StringLength = 26)]
        public string UserProfilePartManager { get; set; }

        [Column(Name = "UserProfilePart_EmployeCode", IsNullable = true, StringLength = -1)]
        public string UserProfilePartEmployeCode { get; set; }

        [Column(Name = "UserProfilePart_RealName", IsNullable = true, StringLength = -1)]
        public string UserProfilePartRealName { get; set; }

        [Column(Name = "OwnerUser", IsNullable = true, StringLength = 26)]
        public string OwnerUser { get; set; }

        [Column(Name = "UserName", IsNullable = true, StringLength = -1)]
        public string UserName { get; set; }

        [Column(Name = "Email", IsNullable = true, StringLength = -1)]
        public string Email { get; set; }

        [Column(Name = "UserId", IsNullable = true, StringLength = -1)]
        public string UserId { get; set; }

    }
}
