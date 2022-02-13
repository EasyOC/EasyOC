using System.Collections.Generic;

namespace TransformalizeModule.Services.Contracts {
   public interface IParameterService {
      public IDictionary<string, string> GetParameters();
   }
}
