using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.OpenApi.Dto
{
    public class PagedResultDto<T>
    {
        public PagedResultDto(int total, IEnumerable<T> items)
        {
            Total = total;
            Items = items;
        }
        public int Total { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
    //
    // 摘要:
    //     Implements Abp.Application.Services.Dto.IListResult`1.
    //
    // 类型参数:
    //   T:
    //     Type of the items in the Abp.Application.Services.Dto.ListResultDto`1.Items list
    public class ListResultDto<T> : IListResult<T>
    {
        //
        // 摘要:
        //     Creates a new Abp.Application.Services.Dto.ListResultDto`1 object.
        public ListResultDto() {
           
        }
        //
        // 摘要:
        //     Creates a new Abp.Application.Services.Dto.ListResultDto`1 object.
        //
        // 参数:
        //   items:
        //     List of items
        public ListResultDto(IReadOnlyList<T> items)
        {
            Items = items;
        }

        //
        // 摘要:
        //     List of items.
        public IReadOnlyList<T> Items { get; set; }
    }

    //
    // 摘要:
    //     This interface is defined to standardize to return a list of items to clients.
    //
    // 类型参数:
    //   T:
    //     Type of the items in the Abp.Application.Services.Dto.IListResult`1.Items list
    public interface IListResult<T>
    {
        //
        // 摘要:
        //     List of items.
        IReadOnlyList<T> Items { get; set; }
    }
}
