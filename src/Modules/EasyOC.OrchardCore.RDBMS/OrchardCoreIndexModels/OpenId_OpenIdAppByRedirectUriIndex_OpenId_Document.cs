using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace EasyOC.OrchardCoreIndexModels
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class OpenId_OpenIdAppByRedirectUriIndex_OpenId_Document
    {

        [JsonProperty, Column(DbType = "INT")]
        public int OpenIdAppByRedirectUriIndexId { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int DocumentId { get; set; }

    }

}



