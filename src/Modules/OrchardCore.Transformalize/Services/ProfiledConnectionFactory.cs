#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2021 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using Transformalize.Providers.Ado;
using System.Data;
using System.Data.Common;

namespace TransformalizeModule.Services {
   public class ProfiledConnectionFactory : IConnectionFactory {
      private readonly IConnectionFactory _original;
      public ProfiledConnectionFactory(IConnectionFactory original) {
         _original = original;
      }

      public AdoProvider AdoProvider => _original.AdoProvider;

      public string Terminator => _original.Terminator;

      public bool SupportsLimit => _original.SupportsLimit;

      public string Enclose(string name) {
         return _original.Enclose(name);
      }

      public IDbConnection GetConnection(string appName = null) {
         return new StackExchange.Profiling.Data.ProfiledDbConnection(_original.GetConnection(appName) as DbConnection, StackExchange.Profiling.MiniProfiler.Current);
      }

      public string GetConnectionString(string appName = null) {
         return _original.GetConnectionString(appName);
      }

      public string SqlDataType(Transformalize.Configuration.Field field) {
         return _original.SqlDataType(field);
      }
   }

}