﻿using Newtonsoft.Json.Linq;
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

namespace Tavstal.TExample.Hooks
{
    public class UconomyHook : Hook, IEconomyProvider
    {
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

                var uconomyPlugin = R.Plugins.GetPlugins().FirstOrDefault(c => c.Name.EqualsIgnoreCase("uconomy"));
                var uconomyType = uconomyPlugin.GetType().Assembly.GetType("fr34kyn01535.Uconomy.Uconomy");
                _pluginInstance =
                    uconomyType.GetField("Instance", BindingFlags.Static | BindingFlags.Public).GetValue(uconomyPlugin);

                var uconomyConfigInst = uconomyType.GetProperty("Configuration").GetValue(uconomyPlugin);
                uconomyConfig = uconomyConfigInst.GetType().GetProperty("Instance").GetValue(uconomyConfigInst);

                _databaseInstance = _pluginInstance.GetType().GetField("Database").GetValue(_pluginInstance);

                _getBalanceMethod = _databaseInstance.GetType().GetMethod(
                    "GetBalance", new[] { typeof(string) });

                _increaseBalanceMethod = _databaseInstance.GetType().GetMethod(
                    "IncreaseBalance", new[] { typeof(string), typeof(decimal) });

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

        public void AddTransaction(UnturnedPlayer player, Transaction transaction)
        {

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

        public void AddTransaction(CSteamID player, Transaction transaction)
        {
            AddTransaction(UnturnedPlayer.FromCSteamID(player), transaction);
        }

        public List<Transaction> GetTransactions(UnturnedPlayer player)
        {
            return null;
        }

        public void AddPlayerCard(CSteamID steamID, BankCard newCard)
        {

        }

        public void UpdatePlayerCard(CSteamID steamID, string id, BankCardDetails newData)
        {

        }

        public void RemovePlayerCard(CSteamID steamID, int index, bool isReversed = false)
        {

        }

        public List<BankCard> GetPlayerCards(CSteamID steamID)
        {
            return null;
        }

        public BankCard GetPlayerCard(CSteamID steamID, int index)
        {
            return null;
        }

        public BankCard GetPlayerCard(CSteamID steamID, string id)
        {
            return null;
        }

        public BankCard GetCard(string id)
        {
            return null;
        }

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
