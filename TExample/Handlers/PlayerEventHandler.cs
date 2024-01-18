using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TExample.Models;

namespace Tavstal.TExample.Handlers
{
    public static class PlayerEventHandler
    {
        private static bool _isAttached = false;

        public static void AttachEvents()
        {
            if (_isAttached)
                return;

            _isAttached = true;

            U.Events.OnPlayerConnected += OnPlayerConnected;
        }

        public static void DetachEvents()
        {
            if (!_isAttached)
                return;

            _isAttached = false;

            U.Events.OnPlayerConnected -= OnPlayerConnected;
        }

        private static void OnPlayerConnected(UnturnedPlayer player)
        {
            PlayerData data = ExampleMain.DatabaseManager.FindPlayer(player.CSteamID.m_SteamID);
            if (data == null)
            {
                ExampleMain.DatabaseManager.AddPlayer(player.CSteamID.m_SteamID, player.SteamName, player.CharacterName);
            }
            else
            {
                if (data.LastCharacterName != player.CharacterName)
                    ExampleMain.DatabaseManager.UpdatePlayer(player.CSteamID.m_SteamID, player.CharacterName);
            }
        }
    }
}
