using System.ComponentModel;

namespace EasyOC
{
    public class OCPermissions
    {
        [Description("View content types")]
        public const string ViewContentTypes = "ViewContentTypes";
        [Description("Edit content types")]
        public const string EditContentTypes = "EditContentTypes";
        #region Users
        /// <summary>
        /// 因为源属性包含空格，无法使用枚举类型替代
        /// <see cref="OrchardCore.Users.CommonPermissions.ViewUsers"/>
        /// </summary>
        //[Description("View Users")]
        public const string View_Users = "View Users";
        /// <summary>
        /// <see cref="OrchardCore.Users.CommonPermissions.ManageUsers"/>
        /// </summary>
        [Description("ManageUsers")]
        public const string ManageUsers = "ManageUsers";
        #endregion
    }
}
