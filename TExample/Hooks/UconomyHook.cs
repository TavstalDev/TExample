using Newtonsoft.Json.Linq;
using Rocket.Core;
using Rocket.Unturned.Player;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TLibrary.Compatibility.Economy;
using Tavstal.TLibrary.Compatibility;
using Tavstal.TLibrary.Extensions;
using Rocket.API;
using Rocket.Core.Plugins;

namespace Tavstal.TExample.Hooks
{
    public class UconomyHook : Hook, IEconomyProvider
    {
        private MethodInfo _getBalanceMethod;
        private MethodInfo _increaseBalanceMethod;
        private MethodInfo _getTranslation;
        private object _databaseInstance;
        private object _pluginInstance;
        private object uconomyConfig;

        public UconomyHook() : base("uconomy_exampleplugin", true) { }

        public override void OnLoad()
        {
            try
            {
                ExampleMain.Logger.Log("Loading Uconomy hook...");

                // Find the IRocketPlugin, in our case it is Uconomy
                IRocketPlugin uconomyPlugin = R.Plugins.GetPlugins().FirstOrDefault(c => c.Name.EqualsIgnoreCase("uconomy"));
                // Get the assembly type of the IRocketPlugin Main
                // Note: fr34kyn01535.Uconomy is the namespace of Uconomy, and Uconomy is the main class name
                Type uconomyType = uconomyPlugin.GetType().Assembly.GetType("fr34kyn01535.Uconomy.Uconomy");
                // Gets the Instance Field of the plugin
                // Note: Instance is a static field declared in the Uconomy class
                // It's the equavalent of doing: Uconomy.Instance
                _pluginInstance =
                    uconomyType.GetField("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(uconomyPlugin);

                // Gets the Configuration Instance from a property declared in the RocketPlugin class
                // Note: It's the equavalent of doing: Uconomy.Instance.Configuration
                var uconomyConfigInst = uconomyType.GetProperty("Configuration").GetValue(uconomyPlugin);
                // Gets the Configuration object from the Instance property
                // Note: It's the equavalent of doing: Uconomy.Instance.Configuration.Instance
                uconomyConfig = uconomyConfigInst.GetType().GetProperty("Instance").GetValue(uconomyConfigInst);

                // Gets the Database Instance from a property declared in the Uconomy class
                // Note: It's the equavalent of doing: Uconomy.Instance.Database or Uconomy.Database if the Database property is static
                _databaseInstance = _pluginInstance.GetType().GetField("Database").GetValue(_pluginInstance);

                // Gets the GetBalance method from the Database Instance
                _getBalanceMethod = _databaseInstance.GetType().GetMethod(
                    "GetBalance", new[] { typeof(string) });

                // Gets the IncreaseBalance method from the Database Instance
                _increaseBalanceMethod = _databaseInstance.GetType().GetMethod(
                    "IncreaseBalance", new[] { typeof(string), typeof(decimal) });

                // Gets the Translate method from the RocketPlugin Instance
                // This helps in using the Uconomy translations
                _getTranslation = _pluginInstance.GetType().GetMethod("Translate", new[] { typeof(string), typeof(object[]) });

                ExampleMain.Logger.LogException("Currency Name >> " + GetCurrencyName());
                ExampleMain.Logger.LogException("Initial Balance >> " + GetConfigValue<decimal>("InitialBalance").ToString());
                ExampleMain.Logger.Log("Uconomy hook loaded.");
            }
            catch (Exception e)
            {
                ExampleMain.Logger.LogError("Failed to load Uconomy hook");
                ExampleMain.Logger.LogError(e.ToString());
            }
        }

        public override void OnUnload() { }

        public override bool CanBeLoaded()
        {
            return R.Plugins.GetPlugins().Any(c => c.Name.EqualsIgnoreCase("uconomy"));
        }

        public string GetCurrencyName()
        {
            string value = "Credits";
            try
            {
                value = GetConfigValue<string>("MoneyName").ToString();
            }
            catch { }
            return value;
        }

        public T GetConfigValue<T>(string VariableName)
        {
            try
            {
                return (T)Convert.ChangeType(uconomyConfig.GetType().GetField(VariableName).GetValue(uconomyConfig), typeof(T));
            }
            catch
            {
                try
                {
                    return (T)Convert.ChangeType(uconomyConfig.GetType().GetProperty(VariableName).GetValue(uconomyConfig), typeof(T));
                }
                catch
                {
                    ExampleMain.Logger.LogError($"Failed to get '{VariableName}' variable!");
                    return default;
                }
            }
        }

        public JObject GetConfig()
        {
            try
            {
                return JObject.FromObject(uconomyConfig.GetType());
            }
            catch
            {
                ExampleMain.Logger.LogError($"Failed to get config jobj.");
                return null;
            }
        }

        public bool HasBuiltInTransactionSystem() { return false; }
        public bool HasBuiltInBankCardSystem() { return false; }
        public decimal Withdraw(UnturnedPlayer player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT)
        {
            return (decimal)_increaseBalanceMethod.Invoke(_databaseInstance, new object[] {
                player.CSteamID.m_SteamID.ToString(), -amount
            });
        }

        public decimal Deposit(UnturnedPlayer player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT)
        {
            return (decimal)_increaseBalanceMethod.Invoke(_databaseInstance, new object[] {
                player.CSteamID.m_SteamID.ToString(), amount
            });
        }

        public decimal GetBalance(UnturnedPlayer player, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT)
        {
            return (decimal)_getBalanceMethod.Invoke(_databaseInstance, new object[] {
                player.CSteamID.m_SteamID.ToString()
            });
        }

        public bool Has(UnturnedPlayer player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT)
        {
            return (GetBalance(player) - amount) >= 0;
        }

        public decimal Withdraw(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT)
        {
            return (decimal)_increaseBalanceMethod.Invoke(_databaseInstance, new object[] {
                player.m_SteamID.ToString(), -amount
            });
        }

        public decimal Deposit(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT)
        {
            return (decimal)_increaseBalanceMethod.Invoke(_databaseInstance, new object[] {
                player.m_SteamID.ToString(), amount
            });
        }

        public decimal GetBalance(CSteamID player, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT)
        {
            return (decimal)_getBalanceMethod.Invoke(_databaseInstance, new object[] {
                player.m_SteamID.ToString()
            });
        }

        public bool Has(CSteamID player, decimal amount, EPaymentMethod method = EPaymentMethod.BANK_ACCOUNT)
        {
            return (GetBalance(player) - amount) >= 0;
        }

        #region Functions Not Used In This Example But Required By Interface
        // Note: you can write custom functions to add Transaction and Bank Card system 
        // It's not recommended to use the native Uconomy events to add them because you can't really handle much stuff with them
        // so it's more recommended to create a modification of Uconomy and add your own events to handle them

        public void AddTransaction(UnturnedPlayer player, Transaction transaction)
        {
            // Not using because the native Uconomy does not have any method to store transactions
            throw new NotSupportedException("This method can not be used because native Uconomy does not have function to use with.");
        }

        public void AddTransaction(CSteamID player, Transaction transaction)
        {
            // Not using because the native Uconomy does not have any method to store transactions
            AddTransaction(UnturnedPlayer.FromCSteamID(player), transaction);
        }

        public List<Transaction> GetTransactions(UnturnedPlayer player)
        {
            // Not using because the native Uconomy does not have any method to store transactions
            throw new NotSupportedException("This method can not be used because native Uconomy does not have function to use with.");
        }

        public void AddPlayerCard(CSteamID steamID, BankCard newCard)
        {
            // Not using because the native Uconomy does not have bank card system
            throw new NotSupportedException("This method can not be used because native Uconomy does not have function to use with.");
        }

        public void UpdatePlayerCard(CSteamID steamID, string id, BankCardDetails newData)
        {
            // Not using because the native Uconomy does not have bank card system
            throw new NotSupportedException("This method can not be used because native Uconomy does not have function to use with.");
        }

        public void RemovePlayerCard(CSteamID steamID, int index, bool isReversed = false)
        {
            // Not using because the native Uconomy does not have bank card system
            throw new NotSupportedException("This method can not be used because native Uconomy does not have function to use with.");
        }

        public List<BankCard> GetPlayerCards(CSteamID steamID)
        {
            // Not using because the native Uconomy does not have bank card system
            throw new NotSupportedException("This method can not be used because native Uconomy does not have function to use with.");
        }

        public BankCard GetPlayerCard(CSteamID steamID, int index)
        {
            // Not using because the native Uconomy does not have bank card system
            throw new NotSupportedException("This method can not be used because native Uconomy does not have function to use with.");
        }

        public BankCard GetPlayerCard(CSteamID steamID, string id)
        {
            // Not using because the native Uconomy does not have bank card system
            throw new NotSupportedException("This method can not be used because native Uconomy does not have function to use with.");
        }

        public BankCard GetCard(string id)
        {
            // Not using because the native Uconomy does not have bank card system
            throw new NotSupportedException("This method can not be used because native Uconomy does not have function to use with.");
        }
        #endregion

        public string Localize(string translationKey, params object[] placeholder)
        {
            return Localize(false, translationKey, placeholder);
        }

        public string Localize(bool addPrefix, string translationKey, params object[] placeholder)
        {
            return ((string)_getTranslation.Invoke(_pluginInstance, new object[] { translationKey, placeholder }));
        }
    }
}
