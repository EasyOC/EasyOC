using FreeSql.DataAnnotations;
using System.Collections.Generic;
using YesSql;
using YesSql.Indexes;

namespace EasyOC.Core.Indexs
{
    public interface IFreeSqlMapDocumentIndex 
    {
        int DocumentId { get; set; }
    }
}
