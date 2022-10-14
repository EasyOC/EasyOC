using EasyOC.Core.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.AppCenter.Indexing
{
    [EOCTable(Name = "DIndex_ClientAppConfiguration")]
    public class ClientAppConfigurationIndex : DIndexBase
    {
        public string HostNames { get; set; }
    }
}
