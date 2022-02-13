using System;
using System.Collections.Generic;
using System.Linq;

namespace TransformalizeModule.Models {
   public class TransformalizeSettings {

      private List<int> _pageSizes;
      private List<int> _pageSizesExtended;
      private string _defaultPageSizes;
      private string _defaultPageSizesExtended;
      private string _commonArrangement;
      private string _mapBoxToken;
      private string _bulkActionCreateTask;
      private string _bulkActionWriteTask;
      private string _bulkActionSummaryTask;
      private string _bulkActionRunTask;
      private string _bulkActionSuccessTask;
      private string _bulkActionFailTask;

      public string CommonArrangement {
         get => string.IsNullOrWhiteSpace(_commonArrangement) ? string.Empty : _commonArrangement;
         set => _commonArrangement = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
      }
      public string DefaultPageSizes {
         get => string.IsNullOrEmpty(_defaultPageSizes) ? "20,50,100" : _defaultPageSizes;
         set => _defaultPageSizes = string.IsNullOrEmpty(value) ? "20,50,100" : value;
      }

      public string DefaultPageSizesExtended {
         get => string.IsNullOrEmpty(_defaultPageSizesExtended) ? "1000,5000,10000" : _defaultPageSizesExtended;
         set => _defaultPageSizesExtended = string.IsNullOrEmpty(value) ? "1000,5000,10000" : value;
      }

      public IEnumerable<int> DefaultPageSizesAsEnumerable() {
         if (_pageSizes != null) {
            return _pageSizes;
         }
         _pageSizes = new List<int>();
         foreach (var size in DefaultPageSizes.Split(',', StringSplitOptions.RemoveEmptyEntries)) {
            if (int.TryParse(size, out int result)) {
               _pageSizes.Add(result);
            }
         }
         return _pageSizes;
      }

      public IEnumerable<int> DefaultPageSizesExtendedAsEnumerable() {
         if (_pageSizesExtended != null) {
            return _pageSizesExtended;
         }
         _pageSizesExtended = new List<int>();
         foreach (var size in DefaultPageSizesExtended.Split(',', StringSplitOptions.RemoveEmptyEntries)) {
            if (int.TryParse(size, out int result)) {
               _pageSizesExtended.Add(result);
            }
         }
         return _pageSizesExtended;
      }


      public string MapBoxToken {
         get => string.IsNullOrWhiteSpace(_mapBoxToken) ? string.Empty : _mapBoxToken;
         set => _mapBoxToken = string.IsNullOrWhiteSpace(value) ? string.Empty : value;
      }

      public string BulkActionCreateTask { get => _bulkActionCreateTask ?? "batch-create"; set => _bulkActionCreateTask = value; }
      public string BulkActionWriteTask { get => _bulkActionWriteTask ?? "batch-write"; set => _bulkActionWriteTask = value; }
      public string BulkActionSummaryTask { get => _bulkActionSummaryTask ?? "batch-summary"; set => _bulkActionSummaryTask = value; }
      public string BulkActionRunTask { get => _bulkActionRunTask ?? "batch-run"; set => _bulkActionRunTask = value; }
      public string BulkActionSuccessTask { get => _bulkActionSuccessTask ?? "batch-success"; set => _bulkActionSuccessTask = value; }
      public string BulkActionFailTask { get => _bulkActionFailTask ?? "batch-fail"; set => _bulkActionFailTask = value; }

   }
}
