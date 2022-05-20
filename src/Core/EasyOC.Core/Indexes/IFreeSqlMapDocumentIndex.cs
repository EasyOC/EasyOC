using FreeSql.DataAnnotations;
using System.Collections.Generic;
using YesSql;
using YesSql.Indexes;

namespace EasyOC.Core.Indexes
{
    public interface IFreeSqlMapDocumentIndex
    {
        int DocumentId { get; set; }
    }
}
