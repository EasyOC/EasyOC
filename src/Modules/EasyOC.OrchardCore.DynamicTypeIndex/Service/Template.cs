
using EasyOC.Core.Indexs;
using FreeSql.DataAnnotations;
namespace EasyOC.DynamicTypeIndex.IndexModels
{
    [EOCIndex("IDX_{tablename}_DocumentId", "DocumentId,ContentItemId")]
    [Table(Name = "Customer_DIndex")]
    public class CustomerDIndex : FreeSqlDocumentIndex
    {
        [Column(StringLength = 26)]
        public string ContentItemId { get; set; }

        [Column(Name = "Name", IsNullable = true)]
        public System.String Name { get; set; }


        [Column(Name = "CustNum", IsNullable = true)]
        public System.String CustNum { get; set; }


        [Column(Name = "MarketSegment", IsNullable = true, StringLength = 26)]
        public System.String MarketSegment { get; set; }


        [Column(Name = "Source", IsNullable = true, StringLength = 26)]
        public System.String Source { get; set; }


        [Column(Name = "Industry", IsNullable = true, StringLength = 26)]
        public System.String Industry { get; set; }


        [Column(Name = "CustClass", IsNullable = true, StringLength = 26)]
        public System.String CustClass { get; set; }


        [Column(Name = "SalesOwner", IsNullable = true, StringLength = 26)]
        public System.String SalesOwner { get; set; }


        [Column(Name = "AddressPart_CountryName", IsNullable = true)]
        public System.String AddressPart_CountryName { get; set; }


        [Column(Name = "AddressPart_Province", IsNullable = true)]
        public System.String AddressPart_Province { get; set; }


        [Column(Name = "AddressPart_City", IsNullable = true)]
        public System.String AddressPart_City { get; set; }


        [Column(Name = "AddressPart_PostalCode", IsNullable = true)]
        public System.String AddressPart_PostalCode { get; set; }


        [Column(Name = "AddressPart_Details", IsNullable = true)]
        public System.String AddressPart_Details { get; set; }


        [Column(Name = "AddressPart_Name", IsNullable = true)]
        public System.String AddressPart_Name { get; set; }

    }
}
