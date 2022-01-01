using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;

namespace EasyOC.OrchardCoreIndexModels
{

    public partial class Audit_AuditTrailEventIndex
    {

        public int Id { get; set; }

        public int? DocumentId { get; set; }

        public string EventId { get; set; }

        public string Category { get; set; }

        public string Name { get; set; }

        public string CorrelationId { get; set; }

        public string UserId { get; set; }

        public string NormalizedUserName { get; set; }

        public DateTime? CreatedUtc { get; set; }

    }

}



