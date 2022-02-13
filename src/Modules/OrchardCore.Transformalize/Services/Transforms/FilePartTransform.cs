using OrchardCore.Modules;
using System;
using System.Collections.Generic;
using Transformalize;
using Transformalize.Contracts;
using Transformalize.Transforms;
using TransformalizeModule.Services.Contracts;

namespace TransformalizeModule.Services.Transforms {
   public class FilePartTransform : StringTransform {

      private readonly IFileService _fileService;
      private readonly Transformalize.Configuration.Field _input;
      private readonly Func<string, IFileService, string> _transform;

      public FilePartTransform(
         IContext context = null,
         IFileService fileService = null
      ) : base(context, "string") {

         if (IsMissingContext()) {
            return;
         }

         if (IsNotReceiving("string")) {
            return;
         }

         if (IsMissing(Context.Operation.Property)) {
            return;
         }

         if(fileService == null) {
            Run = false;
            Context.Error("FilePartTransform needs file service passed in.");
            return;
         }

         _fileService = fileService;         

         _input = SingleInput();

         switch (Context.Operation.Property.ToLower()) {
            case "filepath":
            case "path":
            case "fullpath":
               _transform = (val, svc) => {
                  if (string.IsNullOrEmpty(val)) {
                     return string.Empty;
                  }
                  if (val.Length != Common.IdLength) {
                     return string.Empty;
                  }
                  var file = svc.GetFilePart(val).Result;
                  return file.FullPath.Text;
               };
               break;
            case "originalname":
            case "name":
            case "filename":
               _transform = (val, svc) => {
                  if (string.IsNullOrEmpty(val)) {
                     return string.Empty;
                  }
                  if (val.Length != Common.IdLength) {
                     return string.Empty;
                  }
                  var file = svc.GetFilePart(val).Result;
                  return file.OriginalName.Text;
               };
               break;
            default:
               Run = false;
               Context.Error($"FilePartTransform can only retrieve FullPath, FilePath, OriginalName, or FileName. It can not retrieve {Context.Operation.Property}.");
               break;
         }
      }

      public override IRow Operate(IRow row) {
         var val = GetString(row, _input);
         row[Context.Field] = _transform(val, _fileService);
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         return new[] { 
            new OperationSignature("filepart") {
               Parameters = new List<OperationParameter>() { 
                  new OperationParameter("property", "") 
               }
            } 
         };
      }
   }
}
