using ACManager.Settings;
using ACManager.StateMachine;
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
    /// Static class to handle miscellaneous tasks, outside of the client.
    /// </summary>
    internal class Utility
    {
        private Machine Machine { get; set; }
        private string AllSettingsPath { get; set; }
        private string CharacterSettingsFile { get { return "acm_settings.xml"; } }
        private string CharacterSettingsPath { get; set; }
        private string BotSettingsFile { get { return "acm_bot_settings.xml"; } }
        private string BotSettingsPath { get; set; }
        private string GUISettingsFile { get { return "acm_gui_settings.xml"; } }
        private string GUISettingsPath { get; set; }
        internal CharacterSettings CharacterSettings { get; set; } = new CharacterSettings();
        internal BotSettings BotSettings { get; set; } = new BotSettings();
        internal GUISettings GUISettings { get; set; } = new GUISettings();
        internal string Version { get; set; }
        internal bool BackCompat { get; set; } = false;

        public Utility(Machine machine, string path, string account, string server)
        {
            Machine = machine;
            CreatePaths(path, account, server);
            Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            CharacterSettings = LoadCharacterSettings();
            BotSettings = LoadBotSettings();
            GUISettings = LoadGUISettings();
        }

        /// <summary>
        /// Creates the paths necessary to save/load settings.
        /// </summary>
        /// <param name="root">Root path of the plugin.</param>
        /// <param name="account">Account name of the currently logged in account.</param>
        /// <param name="server">Server name of teh currently logged in account.</param>
        private void CreatePaths(string root, string account, string server)
        {
            try
            {
                string intermediatePath = Path.Combine(root, account);
                AllSettingsPath = Path.Combine(intermediatePath, server);
                CharacterSettingsPath = Path.Combine(AllSettingsPath, CharacterSettingsFile);
                BotSettingsPath = Path.Combine(AllSettingsPath, BotSettingsFile);
                GUISettingsPath = Path.Combine(AllSettingsPath, GUISettingsFile);
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }

        /// <summary>
        /// Creates the directory path if it does not exist.
        /// </summary>
        /// <returns>Boolean whether or not the directory path exists.</returns>
        private bool SettingsPathExists()
        {
            try
            {
                DirectoryInfo di = Directory.CreateDirectory(AllSettingsPath);
                if (di.Exists)
                {
                    return true;
                }
                else
                {
                    return false;
                }
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
        public void SaveCharacterSettings()
        {
            try
            {
                if (SettingsPathExists())
                {
                    using (XmlTextWriter writer = new XmlTextWriter(CharacterSettingsPath, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented;
                        writer.WriteStartDocument();
                        if (CharacterSettings.Characters.Contains(Machine.CurrentCharacter))
                        {
                            for (int i = 0; i < CharacterSettings.Characters.Count; i++)
                            {
                                if (CharacterSettings.Characters[i].Equals(Machine.CurrentCharacter))
                                {
                                    CharacterSettings.Characters[i] = Machine.CurrentCharacter;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            CharacterSettings.Characters.Add(Machine.CurrentCharacter);
                        }
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
        /// <returns>AllSettings</returns>
        internal CharacterSettings LoadCharacterSettings()
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
                return new CharacterSettings();
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
        public void SaveBotSettings()
        {
            try
            {
                if (SettingsPathExists())
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
        /// <returns>AllSettings</returns>
        internal BotSettings LoadBotSettings()
        {
            try
            {
                if (File.Exists(BotSettingsPath))
                {
                    using (XmlTextReader reader = new XmlTextReader(BotSettingsPath))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(BotSettings));

                        BotSettings settings = (BotSettings)xmlSerializer.Deserialize(reader);
                        if (CharacterSettings.Advertisements.Count > 0 && settings.Advertisements.Count == 0)
                        {
                            settings.Advertisements = new List<Advertisement>(CharacterSettings.Advertisements);
                            CharacterSettings.Advertisements.Clear();
                            SaveCharacterSettings();
                            BackCompat = true;
                        }
                        return settings;
                    }
                }
                else
                {
                    BotSettings botSettings = new BotSettings();
                    if (CharacterSettings.Advertisements.Count > 0 && botSettings.Advertisements.Count == 0)
                    {
                        botSettings.Advertisements = new List<Advertisement>(CharacterSettings.Advertisements);
                        CharacterSettings.Advertisements.Clear();
                        SaveCharacterSettings();
                        BackCompat = true;
                    }
                    return botSettings;
                }
            }
            catch (Exception ex)
            {
                Debug.ToChat(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Saves the GUI settings for views to be visible or not, etc.
        /// </summary>
        internal void SaveGUISettings()
        {
            try
            {
                if (SettingsPathExists())
                {
                    using (XmlTextWriter writer = new XmlTextWriter(GUISettingsPath, Encoding.UTF8))
                    {
                        writer.Formatting = Formatting.Indented;
                        writer.WriteStartDocument();

                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(GUISettings));
                        xmlSerializer.Serialize(writer, GUISettings);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.ToChat(ex.Message);
            }
        }

        /// <summary>
        /// Loads the GUI settings for views to be visible by default or not, etc.
        /// </summary>
        /// <returns></returns>
        internal GUISettings LoadGUISettings()
        {
            try
            {
                if (File.Exists(GUISettingsPath))
                {
                    using (XmlTextReader reader = new XmlTextReader(GUISettingsPath))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(GUISettings));
                        return (GUISettings)xmlSerializer.Deserialize(reader);
                    }
                }
                return new GUISettings();
            }
            catch (Exception ex)
            {
                Debug.ToChat(ex.Message);
                return null;
            }
        }
    }
}
