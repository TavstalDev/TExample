using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using UnityEngine;
using System.Collections.Generic;
using System;
using Logger = Rocket.Core.Logging.Logger;
using Steamworks;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Tavstal.TLibrary;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.TExample
{
    public class ExampleMain : PluginBase<ExampleConfig>
    {
        public new static ExampleMain Instance { get; private set; }
        public new static TLogger Logger = new TLogger("TExample", false);

        public override void OnLoad()
        {
            Instance = this;
            
            Logger.Log("#########################################");
            Logger.Log("# Thanks for using my plugin");
            Logger.Log("# Plugin Created By YourName");
            Logger.Log("# Discord: @YourDiscordName");
            Logger.Log("# Website: https://your.website.example");
            Logger.Log("# Discord: https://discord.gg/your_invite");
            Logger.Log("#########################################");
            Logger.Log($"# Build Version: {Version}");
            Logger.Log($"# Build Date: {BuildDate}");
            Logger.Log("#########################################");
            Logger.Log($"# {Name} has been loaded.");
        }

        public override void OnUnLoad()
        {
            
            Logger.Log($"# {Name} has been successfully unloaded.");

        }


        public override Dictionary<string, string> DefaultLocalization =>
           new Dictionary<string, string>
           {
             { "prefix", "&e[TExample]" }
           };

        private void Update()
        {
            
        }
    }
}