using Microsoft.AspNetCore.Http;
using Transformalize.ConfigurationFacade;

namespace TransformalizeModule.Services {
   public static class SessionExtensions {
      public static void SetCfg(this ISession session, string key, Process cfg) {
         session.SetString("tfl:"+key, cfg.Serialize());
      }
      public static Process GetCfg(this ISession session, string key) {
         var sessionData = session.GetString("tfl:" + key);
         return sessionData == null ? new Process() : new Process(sessionData);
      }
   }
}
