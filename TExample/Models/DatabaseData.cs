using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Compatibility;

namespace Tavstal.TExample.Models
{
    public class DatabaseData : DatabaseSettingsBase
    {
        // Note: It starts from 7 because there are 6 defined property in the base class
        [JsonProperty(Order = 7)]
        public string DatabaseTable_Players { get; set; }
        public DatabaseData(string tableName) 
        {
            DatabaseTable_Players = tableName;
        }
    }
}
