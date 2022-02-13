#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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

using Autofac;
using Cfg.Net.Contracts;
using TransformalizeModule.Services.Contracts;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Impl;
using Process = Transformalize.Configuration.Process;
using IContainer = TransformalizeModule.Services.Contracts.IContainer;
using StackExchange.Profiling;

namespace TransformalizeModule.Services {

   /// <summary>
   /// Loads parameter values from form table
   /// </summary>
   public class LoadFormModifier : ILoadFormModifier {

      private readonly ISettingsService _settings;
      private readonly CombinedLogger<LoadFormModifier> _logger;
      private readonly IContainer _container;

      public ISerializer Serializer { get; set; }

      public LoadFormModifier(
         CombinedLogger<LoadFormModifier> logger,
         ISettingsService settings,
         IContainer container
      ) {
         _logger = logger;
         _settings = settings;
         _container = container;
      }

      public string Modify(string cfg, int id, IDictionary<string, string> parameters) {
         using (MiniProfiler.Current.Step("Load Form")) {
            return ModifyInternal(cfg, id, parameters);
         }
      }

      private string ModifyInternal(string cfg, int id, IDictionary<string,string> parameters) {

         var process = new Process(cfg) { Id = id };

         // if there aren't any parameters, just leave
         if (!process.Parameters.Any()) {
            return cfg;
         }

         // if there isn't a primary key, just leave
         var key = process.Parameters.FirstOrDefault(p => p.PrimaryKey);
         if(key == null || parameters == null || !parameters.ContainsKey(key.Name)) {
            return cfg;
         }

         // if the value being passed in for the primary key is the type's default value, just leave because it's an insert
         if(parameters[key.Name] == Transformalize.Constants.TypeDefaults()[key.Type].ToString()) {
            return cfg;
         }

         _settings.ApplyCommonSettings(process);

         var fields = new List<Field>();

         // get all the fields that should be in the form's table and are not specific to insert or update scope
         foreach (var pr in process.Parameters.Where(p=>p.Output && p.Scope == "[default]")) {

            var field = new Field {
               Name = pr.Name,
               Alias = pr.Name,
               Type = pr.Type,
               Default = pr.Value,
               Length = pr.Length,
               Format = pr.Format,
               InputType = pr.InputType,
               Precision = pr.Precision,
               Scale = pr.Scale
            };

            fields.Add(field);
         }

         var connection = process.Connections.First(c => c.Table != "[default]");

         // create entity
         var entity = new Entity {
            Name = connection.Table,
            Fields = fields,
            Input = connection.Name,
            Filter = new List<Filter>() {
               new Filter { Field = key.Name, Value= parameters[key.Name]}
            }
         };

         // create process to load the form submission
         var modified = new Process {
            Id = id,
            Name = "Load Form",
            ReadOnly = true,
            Entities = new List<Entity> { entity },
            Connections = new List<Connection> { connection }
         };

         modified.Load();

         if (!modified.Errors().Any()) {

            // run the process which should get a single row (the form submission) into output
            CfgRow output;
            using (var scope = _container.CreateScope(modified, _logger, null)) {
               scope.Resolve<IProcessController>().Execute();
               output = modified.Entities[0].Rows.FirstOrDefault();
            }

            if (output != null) {
               foreach (var parameter in process.Parameters) {
                  var field = modified.Entities[0].Fields.FirstOrDefault(f => f.Name == parameter.Name);
                  // put the form submission value in the parameters
                  if(field != null) {
                     parameters[parameter.Name] = output[field.Name].ToString();
                  }
               }
            }
         }

         foreach (var error in process.Errors()) {
            _logger.Error(() => error);
         }

         return cfg;
      }


   }

}
