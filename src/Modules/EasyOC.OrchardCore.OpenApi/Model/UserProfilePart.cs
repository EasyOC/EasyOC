using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;

namespace EasyOC.OrchardCore.OpenApi.Model
{
    public class UserProfile : ContentItem
    {


    }
    public class UserProfilePart : ContentPart
    {
        public TextField UserName { get; set; }
        public TextField UserId { get; set; }
        public TextField NickName { get; set; }
        public TextField FirstName { get; set; }
        public TextField LastName { get; set; }
        public TextField Gender { get; set; }
        public UserPickerField Manager { get; set; }
        public ContentPickerField Department { get; set; }
        public TextField RealName { get; set; }
        public TextField EmployeCode { get; set; }
    }
}
