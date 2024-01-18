using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Compatibility.Interfaces;
using Tavstal.TLibrary.Managers;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Services;
using Tavstal.TExample.Models;

namespace Tavstal.TExample.Managers
{
    public class DatabaseManager : DatabaseManagerBase
    {
        private static ExampleConfig _pluginConfig => ExampleMain.Instance.Config;

        public DatabaseManager(IPlugin plugin, IConfigurationBase config) : base(plugin, config)
        {

        }

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

        #endregion
    }
}
