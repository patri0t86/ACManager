using ACManager.Settings;
using Decal.Adapter;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ACManager
{
    internal class Utility
    {
        private PluginCore Plugin { get; set; }
        private string AllSettingsPath { get; set; }
        private string OldSettingsFile { get; set; } = "settings.xml";
        private string OldSettingsPath { get; set; }
        private string CharacterSettingsFile { get; set; } = "acm_settings.xml";
        private string CharacterSettingsPath { get; set; }
        private string BotSettingsFile { get; set; } = "acm_bot_settings.xml";
        private string BotSettingsPath { get; set; }
        private string ErrorFile { get; set; } = "errors.txt";
        private string ErrorPath { get; set; }
        internal string Version { get; set; }
        internal AllSettings AllSettings { get; set; } = new AllSettings();

        public Utility(PluginCore parent)
        {
            Plugin = parent;
            CreateSettingsPath();
            OldSettingsPath = Path.Combine(Plugin.Path, OldSettingsFile); // remove eventually
            CharacterSettingsPath = Path.Combine(AllSettingsPath, CharacterSettingsFile);
            ErrorPath = Path.Combine(AllSettingsPath, ErrorFile);
            GetVersion();
        }

        private void CreateSettingsPath()
        {
            try
            {
                string intermediatePath = Path.Combine(Plugin.Path, CoreManager.Current.CharacterFilter.AccountName);
                AllSettingsPath = Path.Combine(intermediatePath, CoreManager.Current.CharacterFilter.Server);
            }
            catch (Exception ex) { LogError(ex); }
        }

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
                WriteToChat(ex.Message);
                return false;
            }
        }

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

                        if (AllSettings.Characters.Contains(Plugin.CurrentCharacter))
                        {
                            for (int i = 0; i < AllSettings.Characters.Count; i++)
                            {
                                if (AllSettings.Characters[i].Equals(Plugin.CurrentCharacter))
                                {
                                    AllSettings.Characters[i] = Plugin.CurrentCharacter;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            AllSettings.Characters.Add(Plugin.CurrentCharacter);
                        }

                        // remove this chunk of code in a future release (2.0?)
                        AllSettings temp = new AllSettings();

                        foreach (Character ch in AllSettings.Characters)
                        {
                            if (ch.Account.Equals(CoreManager.Current.CharacterFilter.AccountName) && ch.Server.Equals(CoreManager.Current.CharacterFilter.Server))
                            {
                                temp.Characters.Add(ch);
                            }
                        }
                        // end remove code

                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(AllSettings));
                        xmlSerializer.Serialize(writer, temp); // change this back to AllSettings from temp, when removing above code
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToChat(ex.Message);
            }
        }

        public void LoadSettings()
        {
            try
            {
                if (File.Exists(CharacterSettingsPath)) // read the new settings file
                {
                    using (XmlTextReader reader = new XmlTextReader(CharacterSettingsPath))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(AllSettings));
                        AllSettings = (AllSettings)xmlSerializer.Deserialize(reader);
                    }
                }
                else if (File.Exists(OldSettingsPath)) // read the old settings.xml file 
                {
                    using (XmlTextReader reader = new XmlTextReader(OldSettingsPath))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(AllSettings));
                        AllSettings = (AllSettings)xmlSerializer.Deserialize(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToChat(ex.Message);
            }
        }

        internal void WriteToChat(string message)
        {
            try
            {
                CoreManager.Current.Actions.AddChatText(" <{ " + PluginCore.PluginName + " }>: " + message, 5);
            }
            catch (Exception ex) { LogError(ex); }
        }

        internal void LogError(Exception ex)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(ErrorPath, true))
                {
                    writer.WriteLine("============================================================================");
                    writer.WriteLine(DateTime.Now.ToString());
                    writer.WriteLine("Error: " + ex.Message);
                    writer.WriteLine("Source: " + ex.Source);
                    writer.WriteLine("Stack: " + ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        writer.WriteLine("Inner: " + ex.InnerException.Message);
                        writer.WriteLine("Inner Stack: " + ex.InnerException.StackTrace);
                    }
                    writer.WriteLine("============================================================================");
                    writer.WriteLine("");
                }
            }
            catch { }
        }

        internal void GetVersion()
        {
            Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
        }
    }
}
