using EasyOC.Core.Application;
using EasyOC.OrchardCore.ContentExtentions.AppServices;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace EasyOC.OrchardCore.RDBMS.Services
{

    public class ContentMappingAppService : AppServcieBase
    {
        private readonly IContentManagementAppService _contentManagementAppService;

        public ContentMappingAppService(IContentManagementAppService contentManagementAppService)
        {
            _contentManagementAppService = contentManagementAppService;
        }

        ///// <summary>
        ///// OC Content to RDBMS
        ///// </summary>
        ///// <param name="contentTypeName"></param>
        ///// <returns></returns>
        //public JObject GetContentTypeMappingRDBMSResult(string contentTypeName)
        //{

        //    var configResult = _contentManagementAppService.(contentTypeName, false);
        //    var result = new Dictionary<string, string>();

        //    foreach (var part in configResult.Parts)
        //    {
        //        foreach (var filed in part.Fields)
        //        {

        //        }
        //    }


        //    return JObject.FromObject(null);

        //}

        /// <summary>
        /// RDBMS TO OC Content
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="contentTypeName"></param>
        /// <returns></returns>
        public JObject GetRDBMSMappingContentTypeResult(string tableName, string contentTypeName)
        {

            var configResult = _contentManagementAppService.GetTypeDefinition(contentTypeName, false);
            var result = new Dictionary<string, string>();

            foreach (var part in configResult.Parts)
            {
                foreach (var filed in part.Fields)
                {

                }
            }
            return JObject.FromObject(null);

        }
    }
}



