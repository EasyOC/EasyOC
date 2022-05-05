using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;

namespace EasyOC.OrchardCore.OpenApi.Model
{
    public class UserProfile : ContentPart
    {
        public UserPickerField OwnerUser { get; set; } = new UserPickerField();
        public TextField UserName { get; set; } = new TextField();
        public TextField Email { get; set; } = new TextField();
        public TextField UserId { get; set; } = new TextField();
    }
    //public class UserProfilePart : ContentPart
    //{ 
    //    public TextField NickName { get; set; }
    //    public TextField FirstName { get; set; }
    //    public TextField LastName { get; set; }
    //    public TextField Gender { get; set; }
    //    public UserPickerField Manager { get; set; }
    //    public ContentPickerField Department { get; set; }
    //    public TextField RealName { get; set; }
    //    public TextField EmployeCode { get; set; }
    //}
}
