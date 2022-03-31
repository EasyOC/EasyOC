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
        /// <summary>
        /// 对应数据库 DocumentId ，不能与 MapIndex 中的属性重名所以简写为 DocId
        /// </summary>
        //[Column(Name = "DocumentId")]
        public virtual int DocumentId { get; set; }

    }
}
