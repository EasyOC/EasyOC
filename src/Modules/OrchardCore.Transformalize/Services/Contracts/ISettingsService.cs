using TransformalizeModule.Models;
using System.Collections.Generic;
using Transformalize.Configuration;

namespace TransformalizeModule.Services.Contracts {
   public interface ISettingsService {
      public Dictionary<string, Connection> Connections { get; }
      public Dictionary<string, Map> Maps { get; }
      public TransformalizeSettings Settings { get; }
      public Process Process { get; set; }
      public IEnumerable<int> GetPageSizes(TransformalizeReportPart part);
      public IEnumerable<int> GetPageSizesExtended(TransformalizeReportPart part);
      public BulkActionTaskNames GetBulkActionTaskNames(TransformalizeReportPart part);
      public void ApplyCommonSettings(Process process);
      public void ApplyCommonSettings(Transformalize.ConfigurationFacade.Process process);
   }
}
