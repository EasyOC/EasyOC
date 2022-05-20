using EasyOC.Core.Indexes;
using EasyOC.Core.Indexes;
using FreeSql.DataAnnotations;
using EasyOC.OrchardCore.DynamicTypeIndex.Index;

// 此代码由程序生成，复制到代码文件后请更新命名空间，
// 或者在命名空间处点击 Alt+Enter 自动更新命名空间
namespace EasyOC.OrchardCore.OpenApi.Handlers
{
    [EOCIndex("IDX_{tablename}_DocumentId","ContentItemId,DocumentId")]
    [EOCTable(Name = "DIndex_VbenMenu")]
    public class DIndexVbenMenu : DIndexBase
    {

        [Column(Name = "MenuName",IsNullable = true,StringLength = -1)]
        public string MenuName { get; set; }

        [Column(Name = "ParentMenu",IsNullable = true,StringLength = 26)]
        public string ParentMenu { get; set; }

        [Column(Name = "OrderNo",IsNullable = true)]
        public decimal OrderNo { get; set; }

        [Column(Name = "Icon",IsNullable = true,StringLength = -1)]
        public string Icon { get; set; }

        [Column(Name = "RoutePath",IsNullable = true,StringLength = -1)]
        public string RoutePath { get; set; }

        [Column(Name = "Component",IsNullable = true,StringLength = -1)]
        public string Component { get; set; }

        [Column(Name = "Permission",IsNullable = true,StringLength = -1)]
        public string Permission { get; set; }

        [Column(Name = "Status",IsNullable = true,StringLength = -1)]
        public string Status { get; set; }

        [Column(Name = "IsExt",IsNullable = true,StringLength = -1)]
        public string IsExt { get; set; }

        [Column(Name = "Show",IsNullable = true,StringLength = -1)]
        public string Show { get; set; }

        [Column(Name = "MenuType",IsNullable = true,StringLength = -1)]
        public string MenuType { get; set; }

        [Column(Name = "Meta",IsNullable = true,StringLength = -1)]
        public string Meta { get; set; }

        [Column(Name = "ComponentType",IsNullable = true)]
        public decimal ComponentType { get; set; }

        [Column(Name = "SchemaId",IsNullable = true,StringLength = -1)]
        public string SchemaId { get; set; }

        [Column(Name = "redirect",IsNullable = true,StringLength = -1)]
        public string redirect { get; set; }

        [Column(Name = "KeepAlive",IsNullable = true,StringLength = -1)]
        public string KeepAlive { get; set; }

    }
}
