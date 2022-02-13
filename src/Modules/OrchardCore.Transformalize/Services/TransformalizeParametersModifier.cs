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
using TransformalizeModule.Services.Modifiers;
using TransformalizeModule.Services.Modules;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac.Modules;
using Transformalize.Contracts;
using Transformalize.Impl;
using Process = Transformalize.Configuration.Process;
using IContainer = TransformalizeModule.Services.Contracts.IContainer;
using StackExchange.Profiling;
using OrchardCore.DisplayManagement.Notify;
using Microsoft.AspNetCore.Mvc.Localization;

namespace TransformalizeModule.Services {

   /// <summary>
   /// Takes the arrangement and validates and transforms the parameters in a transformalize process
   /// Transfers external parameters to internal parameters and adds Valid and Message fields to each
   /// </summary>
   public class TransformalizeParametersModifier : ITransformalizeParametersModifier {

      private const string _tpInput = "tp-input";
      private const string _tpOutput = "tp-output";
      private readonly ISettingsService _settings;
      private readonly CombinedLogger<TransferParameterModifier> _logger;
      private readonly IContainer _container;
      private readonly INotifier _notifier;
      private readonly IHtmlLocalizer<TransferParameterModifier> H;

      public ISerializer Serializer { get; set; }

      public TransformalizeParametersModifier(
         CombinedLogger<TransferParameterModifier> logger,
         IHtmlLocalizer<TransferParameterModifier> htmlLocalizer,
         ISettingsService settings,
         IContainer container,
         INotifier notifier
      ) {
         _logger = logger;
         _settings = settings;
         _container = container;
         _notifier = notifier;
         H = htmlLocalizer;
      }

      public string Modify(string cfg, int id, IDictionary<string, string> parameters) {
         using (MiniProfiler.Current.Step("Transformalize Parameters")) {
            return ModifyInternal(cfg, id, parameters);
         }
      }

      private string ModifyInternal(string cfg, int id, IDictionary<string, string> parameters) {

         // using facade (which is all string properties) so things can be 
         // transformed before types are checked or place-holders are replaced

         var builder = new ContainerBuilder();
         builder.RegisterModule(new ShorthandModule(_logger));

         Transformalize.ConfigurationFacade.Process facade;

         using (var scope = builder.Build().BeginLifetimeScope()) {
            facade = new Transformalize.ConfigurationFacade.Process(
               cfg,
               parameters: parameters,
               dependencies: new List<IDependency> {
                  new TransferParameterModifier(),  // consumes parameters
                  scope.ResolveNamed<IDependency>(TransformModule.ParametersName),
                  scope.ResolveNamed<IDependency>(ValidateModule.ParametersName)
               }.ToArray()
            );
            facade.Id = id.ToString();
         }

         if (!facade.Parameters.Any()) {
            return cfg;
         }

         _settings.ApplyCommonSettings(facade);

         var fields = new List<Field>();

         foreach (var pr in facade.Parameters) {

            var field = new Field {
               Name = pr.Name,
               Alias = pr.Name,
               // Default = pr.Value,  (something has changed, this value didn't cause problems before but it is now in DefaultRowReader)
               Label = pr.Label,
               PostBack = pr.PostBack,
               Type = pr.Type,
               Help = pr.Help,
               InputType = pr.InputType,  // used in ParameterRowReader to identify files
               Transforms = pr.Transforms.Select(o => o.ToOperation()).ToList(),
               Validators = pr.Validators.Select(o => o.ToOperation()).ToList()
            };
            if (!string.IsNullOrEmpty(pr.Length)) {
               field.Length = pr.Length;
            }
            if (!string.IsNullOrEmpty(pr.Precision) && int.TryParse(pr.Precision, out int precision)) {
               field.Precision = precision;
            }
            if (!string.IsNullOrEmpty(pr.Scale) && int.TryParse(pr.Scale, out int scale)) {
               field.Scale = scale;
            }
            fields.Add(field);
         }

         var validatorFields = new List<Field>();

         foreach (var field in fields.Where(f => f.Validators.Any())) {

            field.ValidField = field.Name + "Valid";
            field.MessageField = field.Name + "Message";

            validatorFields.Add(new Field {
               Name = field.ValidField,
               Input = false,
               Default = "true",
               Type = "bool"
            });
            validatorFields.Add(new Field {
               Name = field.MessageField,
               Input = false,
               Default = string.Empty,
               Type = "string",
               Length = "255"
            });
         }

         // create an internal connection for input
         var connections = new List<Transformalize.ConfigurationFacade.Connection> {
            new Transformalize.ConfigurationFacade.Connection() { Name = _tpInput, Provider = "internal" }
         };

         // add existing connections in case maps need to be loaded
         connections.AddRange(facade.Connections);

         //create an internal connection for output
         connections.Add(new Transformalize.ConfigurationFacade.Connection() { Name = _tpOutput, Provider = "internal" });

         // create entity
         var entity = new Entity {
            Name = "Parameters",
            Fields = fields,
            CalculatedFields = validatorFields,
            Input = _tpInput
         };

         // disable checking for invalid characters unless set
         var arrangementParameters = new List<Parameter>();
         foreach (var parameter in facade.Parameters) {
            var add = parameter.ToParameter();
            if (parameter.InvalidCharacters == null) {
               add.InvalidCharacters = string.Empty;
            }
            arrangementParameters.Add(add);
         }

         // create process to transform and validate the parameter values
         var process = new Process {
            Id = id,
            Name = "Transformalize Parameters",
            ReadOnly = true,
            Mode = "form",  // causes auto post-back's to resolve to either true or false
            Output = _tpOutput,
            Parameters = arrangementParameters,
            Maps = facade.Maps.Select(m => m.ToMap()).ToList(),
            Scripts = facade.Scripts.Select(m => m.ToScript()).ToList(),
            Entities = new List<Entity> { entity },
            Connections = connections.Select(c => c.ToConnection()).ToList()
         };

         process.Load(); // very important to check after creating, as it runs validation and even modifies!

         if (!process.Errors().Any()) {

            // modification in Load() do not make it out to local variables so overwrite them
            entity = process.Entities.First();
            fields = entity.Fields;
            validatorFields = entity.CalculatedFields;

            CfgRow output;
            _container.GetReaderAlternate = (input, rowFactory) => new ParameterRowReader(input, new DefaultRowReader(input, rowFactory));
            using (var scope = _container.CreateScope(process, _logger, null)) {
               scope.Resolve<IProcessController>().Execute();
               output = process.Entities[0].Rows.FirstOrDefault();
            }

            for (int i = 0; i < process.Maps.Count; i++) {
               var source = process.Maps[i];
               var target = facade.Maps[i];
               if (source.Items.Any() && !target.Items.Any()) {
                  foreach (var item in source.Items) {
                     target.Items.Add(new Transformalize.ConfigurationFacade.MapItem() {
                        From = item.From.ToString(),
                        To = item.To.ToString(),
                        Parameter = item.Parameter,
                        Value = item.Value
                     });
                  }
                  target.Query = string.Empty;  // remove the query so they are not queried again
               }
            }

            if (output != null) {

               JintVisibility jintVisibility = null;

               foreach (var parameter in facade.Parameters) {
                  var field = fields.First(f => f.Name == parameter.Name);

                  // set the transformed value
                  parameter.Value = output[field.Name].ToString();
                  parameter.PostBack = field.PostBack;  // auto is changed to true|false in transformalize

                  // set the validation results
                  if (parameter.Validators.Any()) {
                     if ((bool)output[field.ValidField]) {
                        parameter.Valid = "true";
                     } else {
                        parameter.Valid = "false";
                        parameter.Message = ((string)output[field.MessageField]).TrimEnd('|');
                     }
                  }

                  // set the visibility
                  if (string.IsNullOrEmpty(parameter.Visible)) {
                     parameter.Visible = "true";
                  } else {
                     if (jintVisibility == null) {
                        jintVisibility = new JintVisibility();
                     }
                     var response = jintVisibility.Visible(new JvRequest(output, parameter.Visible));
                     if (response.Faulted) {
                        _logger.Error(() => $"Parameter {parameter.Name} has a visible script error: {response.Message}");
                        _notifier.Error(H[$"Parameter {parameter.Name} has a visible script error: {response.Message}"]);
                     }
                     parameter.Visible = response.Visible.ToString().ToLower();
                     if (parameter.Visible == "false") {
                        parameter.Valid = "true";  // because they won't be able to fix it
                     }
                  }

                  // remove this stuff because all the transforming and validating is done at this point
                  parameter.T = null;
                  parameter.Transforms.Clear();
                  parameter.V = null;
                  parameter.Validators.Clear();
               }
            }

            return facade.Serialize();
         }

         foreach (var error in process.Errors()) {
            _logger.Error(() => error);
         }

         return cfg;
      }


   }

}
