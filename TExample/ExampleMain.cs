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
using Tavstal.TExample.Managers;
using Tavstal.TLibrary.Compatibility.Economy;
using Tavstal.TLibrary.Managers;
using Tavstal.TExample.Hooks;
using Tavstal.TExample.Handlers;

namespace Tavstal.TExample
{
    public class ExampleMain : PluginBase<ExampleConfig>
    {
        public new static ExampleMain Instance { get; private set; }
        public new static TLogger Logger = new TLogger("TExample", false);
        public new static DatabaseManager DatabaseManager { get; private set; }
        public static bool IsConnectionAuthFailed { get; set; }
        public static IEconomyProvider EconomyProvider { get; private set; }

        public override void OnLoad()
        {
            Instance = this;
            Level.onPostLevelLoaded += Event_OnPluginsLoaded;
            PlayerEventHandler.AttachEvents();

            Logger.Log("#########################################");
            Logger.Log("# Thanks for using my plugin");
            Logger.Log($"# Plugin Created By {VersionInfo.CompanyName}");
            Logger.Log("# Discord: @YourDiscordName");
            Logger.Log("# Website: https://your.website.example");
            Logger.Log("# Discord: https://discord.gg/your_invite");
            Logger.Log("#########################################");
            Logger.Log($"# Build Version: {Version}");
            Logger.Log($"# Build Date: {BuildDate}");
            Logger.Log("#########################################");

            DatabaseManager = new DatabaseManager(this, Config);
            if (IsConnectionAuthFailed)
                return;

            Logger.Log($"# {Name} has been loaded.");
        }

        public override void OnUnLoad()
        {
            Level.onPostLevelLoaded -= Event_OnPluginsLoaded;
            PlayerEventHandler.DetachEvents();
            Logger.Log($"# {Name} has been successfully unloaded.");
        }

        private void Event_OnPluginsLoaded(int i)
        {
            if (IsConnectionAuthFailed)
            {
                Logger.LogWarning($"# Unloading {GetPluginName()} due to database authentication error.");
                this?.UnloadPlugin();
                return;
            }

            Logger.LogLateInit();
            Logger.LogWarning("# Searching for economy plugin...");
            HookManager = new HookManager(this);
            HookManager.LoadAll(Assembly);

            if (!HookManager.IsHookLoadable<UconomyHook>())
            {
                Logger.LogError($"# Failed to load economy hook. Unloading {GetPluginName()}...");
                this?.UnloadPlugin();
                return;
            }
            EconomyProvider = HookManager.GetHook<UconomyHook>();
        }


        public override Dictionary<string, string> DefaultLocalization =>
           new Dictionary<string, string>
           {
             { "prefix", $"&e[{GetPluginName()}]" }
           };

        private void Update()
        {
            
        }
    }
}