using ACManager.Settings;
using ACManager.Settings.BuffDefaults;
using Decal.Adapter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ACManager
{
    /// <summary>
    /// Static class to handle miscellaneous tasks, like file I/O, outside of the client.
    /// </summary>
    public static class Utility
    {
        public static string Version { get; set; }
        private static string AllSettingsPath { get; set; }
        private static string CharacterSettingsFile { get { return "acm_settings.xml"; } }
        private static string CharacterSettingsPath { get; set; }
        private static string BotSettingsFile { get { return "acm_bot_settings.xml"; } }
        private static string BotSettingsPath { get; set; }
        private static string InventoryFile { get { return "acm_inventory.xml"; } }
        private static string InventoryPath { get; set; }
        private static string EquipmentSettingsFile { get { return "acm_equipment.xml"; } }
        private static string EquipmentSettingsPath { get; set; }
        private static string GiftsFile { get { return "acm_gifts.log"; } }
        private static string GiftsPath { get; set; }
        private static string BuffProfilesPath { get; set; }
        public static CharacterSettings CharacterSettings { get; set; } = new CharacterSettings();
        public static BotSettings BotSettings { get; set; } = new BotSettings();
        public static InventorySettings Inventory { get; set; } = new InventorySettings();
        public static List<BuffProfile> BuffProfiles { get; set; } = new List<BuffProfile>();
        public static EquipmentSettings EquipmentSettings { get; set; } = new EquipmentSettings();

        public static void Init(string path)
        {
            CreatePaths(path);
            Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            CharacterSettings = LoadCharacterSettings();
            BotSettings = LoadBotSettings();
            Inventory = LoadInventories();
            EquipmentSettings = LoadEquipmentSettings();
            LoadBuffProfiles();
        }

        /// <summary>
        /// Creates the paths necessary to save/load settings.
        /// </summary>
        /// <param name="root">Root path of the plugin.</param>
        private static void CreatePaths(string root)
        {
            try
            {
                string intermediatePath = Path.Combine(root, CoreManager.Current.CharacterFilter.AccountName);
                AllSettingsPath = Path.Combine(intermediatePath, CoreManager.Current.CharacterFilter.Server);
                CharacterSettingsPath = Path.Combine(AllSettingsPath, CharacterSettingsFile);
                BotSettingsPath = Path.Combine(AllSettingsPath, BotSettingsFile);
                InventoryPath = Path.Combine(AllSettingsPath, InventoryFile);
                BuffProfilesPath = Path.Combine(root, "BuffProfiles");
                EquipmentSettingsPath = Path.Combine(AllSettingsPath, EquipmentSettingsFile);
                GiftsPath = Path.Combine(AllSettingsPath, GiftsFile);
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        /// <summary>
        /// Creates the directory path if it does not exist.
        /// </summary>
        /// <returns>Boolean whether or not the directory path exists.</returns>
        private static bool SettingsPathCreateOrExists()
        {
            try
            {
                DirectoryInfo di = Directory.CreateDirectory(AllSettingsPath);
                return di.Exists;
            }
            catch (Exception ex)
            {
                Debug.ToChat(ex.Message);
                return false;
            }
        }

        private static bool BuffsPathCreateOrExists()
        {
            try
            {
                DirectoryInfo di = Directory.CreateDirectory(BuffProfilesPath);
                return di.Exists;
            }
            catch (Exception ex)
            {
                Debug.ToChat(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Saves the per character settings to file.
        /// </summary>
        public static void SaveCharacterSettings()
        {
            try
            {
                if (SettingsPathCreateOrExists())
                {
                    using (XmlTextWriter writer = new XmlTextWriter(CharacterSettingsPath, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented;
                        writer.WriteStartDocument();

                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(CharacterSettings));
                        xmlSerializer.Serialize(writer, CharacterSettings);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.ToChat(ex.Message);
            }
        }

        /// <summary>
        /// Loads the character settings file if it exists.
        /// </summary>
        /// <returns></returns>
        public static CharacterSettings LoadCharacterSettings()
        {
            try
            {
                if (File.Exists(CharacterSettingsPath))
                {
                    using (XmlTextReader reader = new XmlTextReader(CharacterSettingsPath))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(CharacterSettings));
                        return (CharacterSettings)xmlSerializer.Deserialize(reader);
                    }
                }
                else
                {
                    return new CharacterSettings();
                }
            }
            catch (Exception ex)
            {
                Debug.ToChat(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Saves the bot settings to file.
        /// </summary>
        public static void SaveBotSettings()
        {
            try
            {
                if (SettingsPathCreateOrExists())
                {
                    using (XmlTextWriter writer = new XmlTextWriter(BotSettingsPath, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented;
                        writer.WriteStartDocument();

                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(BotSettings));
                        xmlSerializer.Serialize(writer, BotSettings);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.ToChat(ex.Message);
            }
        }

        /// <summary>
        /// Loads the bot settings file if it exists.
        /// </summary>
        /// <returns></returns>
        public static BotSettings LoadBotSettings()
        {
            try
            {
                if (File.Exists(BotSettingsPath))
                {
                    using (XmlTextReader reader = new XmlTextReader(BotSettingsPath))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(BotSettings));
                        return (BotSettings)xmlSerializer.Deserialize(reader);
                    }
                }
                else
                {
                    return new BotSettings();
                }
            }
            catch (Exception ex)
            {
                Debug.ToChat(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Save the updated inventory to disk.
        /// </summary>
        public static void SaveInventory(CharacterInventory inventory)
        {
            try
            {
                if (Inventory.CharacterInventories.Contains(inventory))
                {
                    for (int i = 0; i < Inventory.CharacterInventories.Count; i++)
                    {
                        if (Inventory.CharacterInventories[i].Equals(inventory))
                        {
                            Inventory.CharacterInventories[i] = inventory;
                            break;
                        }
                    }
                }
                else
                {
                    Inventory.CharacterInventories.Add(inventory);
                }

                if (SettingsPathCreateOrExists())
                {
                    using (XmlTextWriter writer = new XmlTextWriter(InventoryPath, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented;
                        writer.WriteStartDocument();

                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(InventorySettings));
                        xmlSerializer.Serialize(writer, Inventory);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.ToChat(ex.Message);
            }
        }

        /// <summary>
        /// Load the known inventories from disk.
        /// </summary>
        /// <returns></returns>
        public static InventorySettings LoadInventories()
        {
            try
            {
                if (File.Exists(InventoryPath))
                {
                    using (XmlTextReader reader = new XmlTextReader(InventoryPath))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(InventorySettings));
                        return (InventorySettings)xmlSerializer.Deserialize(reader);
                    }
                }
                else
                {
                    return new InventorySettings();
                }
            }
            catch (Exception ex)
            {
                Debug.ToChat(ex.Message);
                return null;
            }
        }

        public static void SaveBuffProfile(BuffProfile profile)
        {
            try
            {
                if (BuffsPathCreateOrExists())
                {
                    string ProfilePath = Path.Combine(BuffProfilesPath, profile.Commands[0] + ".xml");
                    using (XmlTextWriter writer = new XmlTextWriter(ProfilePath, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented;
                        writer.WriteStartDocument();

                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(BuffProfile));
                        xmlSerializer.Serialize(writer, profile);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.ToChat(ex.Message);
            }
        }

        public static bool LoadBuffProfiles()
        {
            try
            {
                if (BuffsPathCreateOrExists())
                {
                    DirectoryInfo dir = new DirectoryInfo(BuffProfilesPath);
                    FileInfo[] files = dir.GetFiles("*.xml");
                    BuffProfiles.Clear();
                    if (files.Length > 0)
                    {
                        foreach (FileInfo file in files)
                        {
                            using (XmlTextReader reader = new XmlTextReader(file.FullName))
                            {
                                XmlSerializer xmlSerializer = new XmlSerializer(typeof(BuffProfile));
                                BuffProfiles.Add((BuffProfile)xmlSerializer.Deserialize(reader));
                            }
                        }
                    }
                    else
                    {
                        GenerateDefaultProfiles();
                        LoadBuffProfiles();
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.ToChat(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Saves Equipment profiles to disk.
        /// </summary>
        public static void SaveEquipmentSettings()
        {
            try
            {
                if (SettingsPathCreateOrExists())
                {
                    using (XmlTextWriter writer = new XmlTextWriter(EquipmentSettingsPath, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented;
                        writer.WriteStartDocument();

                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(EquipmentSettings));
                        xmlSerializer.Serialize(writer, EquipmentSettings);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.ToChat(ex.Message);
            }
        }

        /// <summary>
        /// Loads Equipment profiles if they exist.
        /// </summary>
        /// <returns></returns>
        public static EquipmentSettings LoadEquipmentSettings()
        {
            try
            {
                if (File.Exists(EquipmentSettingsPath))
                {
                    using (XmlTextReader reader = new XmlTextReader(EquipmentSettingsPath))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(EquipmentSettings));
                        return (EquipmentSettings)xmlSerializer.Deserialize(reader);
                    }
                }
                else
                {
                    return new EquipmentSettings();
                }
            }
            catch (Exception ex)
            {
                Debug.ToChat(ex.Message);
                return null;
            }
        }

        public static void SaveGiftToLog(string message)
        {
            try
            {
                string text = $"{DateTime.Now} --- {message}";
                File.AppendAllText(GiftsPath, text);
            }
            catch (Exception ex)
            {
                Debug.ToChat(ex.Message);
            }
        }

        public static void GenerateDefaultProfiles()
        {
            BuffProfile newProfile = new BuffProfile();

            BotBuffs botBuffs = new BotBuffs();
            newProfile.Commands = botBuffs.Commands;
            newProfile.Buffs = botBuffs.Buffs;
            SaveBuffProfile(newProfile);

            Banes banes = new Banes();
            newProfile.Commands = banes.Commands;
            newProfile.Buffs = banes.Buffs;
            SaveBuffProfile(newProfile);

            Finesse finesse = new Finesse();
            newProfile.Commands = finesse.Commands;
            newProfile.Buffs = finesse.Buffs;
            SaveBuffProfile(newProfile);

            Heavy heavy = new Heavy();
            newProfile.Commands = heavy.Commands;
            newProfile.Buffs = heavy.Buffs;
            SaveBuffProfile(newProfile);

            Light light = new Light();
            newProfile.Commands = light.Commands;
            newProfile.Buffs = light.Buffs;
            SaveBuffProfile(newProfile);

            Mage mage = new Mage();
            newProfile.Commands = mage.Commands;
            newProfile.Buffs = mage.Buffs;
            SaveBuffProfile(newProfile);

            Missile missile = new Missile();
            newProfile.Commands = missile.Commands;
            newProfile.Buffs = missile.Buffs;
            SaveBuffProfile(newProfile);

            Trades trades = new Trades();
            newProfile.Commands = trades.Commands;
            newProfile.Buffs = trades.Buffs;
            SaveBuffProfile(newProfile);

            TwoHand twoHand = new TwoHand();
            newProfile.Commands = twoHand.Commands;
            newProfile.Buffs = twoHand.Buffs;
            SaveBuffProfile(newProfile);

            VoidBuffs voidBuffs = new VoidBuffs();
            newProfile.Commands = voidBuffs.Commands;
            newProfile.Buffs = voidBuffs.Buffs;
            SaveBuffProfile(newProfile);

            XpChain xpChain = new XpChain();
            newProfile.Commands = xpChain.Commands;
            newProfile.Buffs = xpChain.Buffs;
            SaveBuffProfile(newProfile);
        }

        public static BuffProfile GetProfile(string command)
        {
            for (int i = 0; i < BuffProfiles.Count; i++)
            {
                if (BuffProfiles[i].Commands.Contains(command))
                {
                    return BuffProfiles[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Builds the list of portals to respond with.
        /// </summary>
        /// <returns>List of formatted strings to be sent to the requestor.</returns>
        public static List<string> GetPortals()
        {
            List<string> portals = new List<string>();
            for (int i = 0; i < CharacterSettings.Characters.Count; i++)
            {
                foreach (Portal portal in CharacterSettings.Characters[i].Portals)
                {
                    if (!string.IsNullOrEmpty(portal.Keyword))
                    {
                        portals.Add($"{portal.Keyword} -> {(string.IsNullOrEmpty(portal.Description) ? "No description" : portal.Description)}{(portal.Level > 0 ? " [" + portal.Level + "+]" : "")}");
                    }
                }
            }
            return portals;
        }

        public static List<string> GetGems()
        {
            List<string> gems = new List<string>();
            foreach (GemSetting gemSetting in BotSettings.GemSettings)
            {
                if (!string.IsNullOrEmpty(gemSetting.Keyword))
                {
                    gems.Add($"{gemSetting.Keyword} -> {gemSetting.Name}");
                }
            }
            return gems;
        }
    }
}
