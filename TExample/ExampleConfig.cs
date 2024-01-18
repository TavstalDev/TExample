using Newtonsoft.Json;
using Rocket.API;
using Tavstal.TLibrary.Compatibility;
using UnityEngine;
using Tavstal.TExample.Models;

namespace Tavstal.TExample
{
    public class ExampleConfig : ConfigurationBase
    {
        [JsonProperty(Order = 3)]
        public DatabaseData Database { get; set; }

        public override void LoadDefaults()
        {
            Database = new DatabaseData("example_players");
        }
    }
}
