using GraphQL.Types;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using OrchardCore.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.OpenApi.GraphQL.Types
{
    public class UserInfoObjectType : ObjectGraphType<User>
    {
        public UserInfoObjectType()
        {

        }
    }
}
