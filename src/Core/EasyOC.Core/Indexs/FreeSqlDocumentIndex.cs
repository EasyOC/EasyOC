using FreeSql.DataAnnotations;
using System.Collections.Generic;
using YesSql;
using YesSql.Indexes;

namespace EasyOC.Core.Indexs
{
    public abstract class FreeSqlDocumentIndex : MapIndex, IFreeSqlMapDocumentIndex
    {
        [Column(IsPrimary = true, IsIdentity = true)]
        new public virtual int Id { get => base.Id; set { base.Id = value; } }
        public virtual int DocumentId { get; set; }

    }
}
