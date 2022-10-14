using Newtonsoft.Json.Linq;
using OrchardCore.DisplayManagement.ModelBinding;

namespace EasyOC.GraphQL.Abstractions.Mutations
{
    public interface IApiUpdateModel : IUpdateModel
    {
        IApiUpdateModel WithModel(JObject jObject);
    }
}
