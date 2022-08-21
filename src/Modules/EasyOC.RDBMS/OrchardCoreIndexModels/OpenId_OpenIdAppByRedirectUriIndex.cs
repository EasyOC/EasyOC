using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace EasyOC.OrchardCoreIndexModels
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class OpenId_OpenIdAppByRedirectUriIndex
    {

        [JsonProperty, Column(IsPrimary = true)]
        public int Id { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string RedirectUri { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int? Count { get; set; }

    }

}



