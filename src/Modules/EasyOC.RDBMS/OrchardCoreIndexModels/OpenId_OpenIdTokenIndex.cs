using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;

namespace EasyOC.OrchardCoreIndexModels
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class OpenId_OpenIdTokenIndex
    {

        [JsonProperty, Column(IsPrimary = true)]
        public int Id { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int? DocumentId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string TokenId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ApplicationId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string AuthorizationId { get; set; }

        [JsonProperty]
        public DateTime? ExpirationDate { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ReferenceId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string Status { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string Subject { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string Type { get; set; }

        [JsonProperty]
        public DateTime? CreationDate { get; set; }

    }

}



