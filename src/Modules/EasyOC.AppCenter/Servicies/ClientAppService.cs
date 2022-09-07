using EasyOC.AppCenter.Indexing;
using EasyOC.Core.Application;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement.Notify;
using System;
using System.Linq;
using System.Threading.Tasks;
using YesSql;

namespace EasyOC.AppCenter.Servicies
{
    [EOCAuthorization(Ignore = true)]
    public class ClientAppService : AppServiceBase
    {
        public async Task<JObject> GetConfiguration()
        {
            var requestFrom = HttpContextAccessor?.HttpContext?.Request.Host.Value.ToLower();
            var indexModel = await Fsql.Select<ClientAppConfigurationIndex>()
                .Where(x => x.Published && x.Latest && x.HostNames.Contains(requestFrom))
                .FirstAsync();
            if (indexModel is null)
            {
               await Notifier.ErrorAsync(H["Invalid Client:{0}", requestFrom]);
                return null;
            }
            var hostNamesToken = JArray.Parse(indexModel.HostNames);
            if (hostNamesToken is null)
            {
                await Notifier.ErrorAsync(H["Service Error : Invalid  Client Configuration.", requestFrom]);
                return null;
            }
            var configHostNames = hostNamesToken.Values<string>();

            if (configHostNames.Any(x => x.ToLower() == requestFrom))
            {
                var configData = await ContentManager.GetAsync(indexModel.ContentItemId);
                return configData.Content;
            }
            return null;

        }
    }
}
