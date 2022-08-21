using OrchardCore.Users.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace EasyOC.OpenApi.Dto
{
    public class UsersBulkActionInput
    {
        public UsersBulkAction BulkAction { get; set; }
        public IEnumerable<string> ItemIds { get; set; } = Enumerable.Empty<string>();
    }
}
