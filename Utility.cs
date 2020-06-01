using Decal.Adapter;
using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Reflection;
using ACManager.Settings;
using System.Xml.Serialization;

namespace ACManager
{
    internal class Utility
    {
        private PluginCore Plugin { get; set; }
        private string SettingsFile { get; set; } = "settings.xml";
        private string SettingsPath { get; set; }
        private string ErrorFile { get; set; } = "errors.txt";
        private string ErrorPath { get; set; }
        private string CrashFile { get; set; } = "crashlog.txt";
        private string CrashPath { get; set; }
        internal string Version { get; set; }
        internal AllSettings AllSettings { get; set; } = new AllSettings();

        public Utility(PluginCore parent)
        {
            Plugin = parent;
            SettingsPath = Path.Combine(Plugin.Path, SettingsFile);
            ErrorPath = Path.Combine(Plugin.Path, ErrorFile);
            CrashPath = Path.Combine(Plugin.Path, CrashFile);
            GetVersion();
        }

        public void SaveSettings()
        {
            try
            {
                using (XmlTextWriter writer = new XmlTextWriter(SettingsPath, Encoding.UTF8))
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

                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(AllSettings));
                    xmlSerializer.Serialize(writer, AllSettings);
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
                if (File.Exists(SettingsPath))
                {
                    using (XmlTextReader reader = new XmlTextReader(SettingsPath))
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
            } catch {}
        }

        internal void LogCrash(string characterName, string duration, string xp, string reasonIfKnown="Crash")
        {
            try
            {
                if (!characterName.Equals(""))
                {
                    using (StreamWriter writer = new StreamWriter(CrashPath, true))
                    {
                        writer.WriteLine(DateTime.Now.ToString() + " -" +
                            " Character=" + characterName + 
                            " Duration=" + duration + 
                            " XP=" + xp + 
                            " Reason=" + reasonIfKnown);
                    }
                }
            }
            catch (Exception ex) { LogError(ex); }
        }

        internal void GetVersion()
        {
            Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
        }
    }
}
