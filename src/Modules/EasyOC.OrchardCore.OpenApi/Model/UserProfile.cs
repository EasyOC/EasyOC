using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;

namespace EasyOC.OrchardCore.OpenApi.Model
{
    public class UserProfilePart : ContentPart
    {
        public TextField NickName { get; set; }
        public TextField FirstName { get; set; }
        public TextField LastName { get; set; }
        public TextField Gender { get; set; }
    }
}
