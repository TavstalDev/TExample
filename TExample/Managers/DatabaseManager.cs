using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Tavstal.TExample.Models;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Compatibility.Database;
using Tavstal.TLibrary.Compatibility.Interfaces;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Managers;

namespace Tavstal.TExample.Managers
{
    public class DatabaseManager : DatabaseManagerBase
    {
        private static ExampleConfig _pluginConfig => ExampleMain.Instance.Config;

        public DatabaseManager(IPlugin plugin, IConfigurationBase config) : base(plugin, config)
        {

        }

        /// <summary>
        /// Checks the schema of the database, creates or modifies the tables if needed
        /// <br/>PS. If you change the Primary Key then you must delete the table.
        /// </summary>
        protected override void CheckSchema()
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    if (!connection.OpenSafe())
                        ExampleMain.IsConnectionAuthFailed = true;
                    if (connection.State != System.Data.ConnectionState.Open)
                        throw new Exception("# Failed to connect to the database. Please check the plugin's config file.");

                    // Player Table
                    if (connection.DoesTableExist<PlayerData>(_pluginConfig.Database.DatabaseTable_Players))
                        connection.CheckTable<PlayerData>(_pluginConfig.Database.DatabaseTable_Players);
                    else
                        connection.CreateTable<PlayerData>(_pluginConfig.Database.DatabaseTable_Players);

                    if (connection.State != System.Data.ConnectionState.Closed)
                        connection.Close();
                }
            }
            catch (Exception ex)
            {
                ExampleMain.Logger.LogException("Error in checkSchema:");
                ExampleMain.Logger.LogError(ex);
            }
        }

        #region Player Table
        public bool AddPlayer(ulong steamId, string steamName, string characterName)
        {
            MySqlConnection MySQLConnection = CreateConnection();
            return MySQLConnection.AddTableRow(tableName: _pluginConfig.Database.DatabaseTable_Players, value: new PlayerData(steamId, steamName, characterName, DateTime.Now));
        }

        public bool RemovePlayer(ulong steamId)
        {
            MySqlConnection MySQLConnection = CreateConnection();
            return MySQLConnection.RemoveTableRow<PlayerData>(tableName: _pluginConfig.Database.DatabaseTable_Players, whereClause: $"SteamId='{steamId}'", parameters: null);
        }

        public bool UpdatePlayer(ulong steamId, string characterName)
        {
            MySqlConnection MySQLConnection = CreateConnection();
            return MySQLConnection.UpdateTableRow<PlayerData>(tableName: _pluginConfig.Database.DatabaseTable_Players, $"SteamId='{steamId}'", new List<SqlParameter>
            {
                SqlParameter.Get<PlayerData>(x => x.LastCharacterName, characterName),
                SqlParameter.Get<PlayerData>(x => x.LastLogin, DateTime.Now)
            });
        }

        public List<PlayerData> GetPlayers()
        {
            MySqlConnection MySQLConnection = CreateConnection();
            return MySQLConnection.GetTableRows<PlayerData>(tableName: _pluginConfig.Database.DatabaseTable_Players, whereClause: string.Empty, null);
        }

        public PlayerData FindPlayer(ulong steamId)
        {
            MySqlConnection MySQLConnection = CreateConnection();
            return MySQLConnection.GetTableRow<PlayerData>(tableName: _pluginConfig.Database.DatabaseTable_Players, whereClause: $"SteamId='{steamId}'", null);
        }
        #endregion
    }
}
