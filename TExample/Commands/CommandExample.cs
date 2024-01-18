using System.Collections.Generic;
using SDG.Unturned;
using Rocket.Unturned.Player;
using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Compatibility.Interfaces;
using static UnityEngine.GraphicsBuffer;
using Tavstal.TLibrary.Helpers.Unturned;
using Pathfinding.Ionic.Zlib;

namespace Tavstal.TExample
{
    public class CommandExample : CommandBase
    {
        public override IPlugin Plugin => ExampleMain.Instance; 
        public override AllowedCaller AllowedCaller => AllowedCaller.Both;
        public override string Name => "example";
        public override string Help => "This is an example description what the command does.";
        public override string Syntax => "";
        public override List<string> Aliases => new List<string>() { "" };
        public override List<string> Permissions => new List<string> { "" };
        public override List<SubCommand> SubCommands => new List<SubCommand>()
        {
            new SubCommand("test", "Example subcommand for the command", "", new List<string>(), new List<string>(), 
                (IRocketPlayer caller, string[] args) =>
                {
                    
                })
        };
        public override bool ExecutionRequested(IRocketPlayer caller, string[] args)
        {
            return true;
        }
    }
}
