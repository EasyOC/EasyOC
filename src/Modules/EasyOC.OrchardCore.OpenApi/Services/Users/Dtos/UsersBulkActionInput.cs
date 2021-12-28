using OrchardCore.Users.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.OpenApi.Dto
{
    public class UsersBulkActionInput
    {
        public UsersBulkAction BulkAction { get; set; }
        public IEnumerable<string> ItemIds { get; set; } = Enumerable.Empty<string>();
    }
}
