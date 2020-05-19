using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using Microsoft.Win32;
using System.IO;
using System.Xml;

namespace ACManager
{
    public static class Utility
    {
        // Installation location from registry entry
        private static readonly string PluginFolder = Registry.GetValue(
            @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Decal\Plugins\{A56AFA67-44C9-4DB9-871E-4A450FA5FBAC}",
            "Path",
            Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Asheron's Call").ToString();
        private static readonly string SettingsFile = PluginFolder + @"\settings.xml";
        private static readonly string ErrorFile = PluginFolder + @"\errors.txt";
        private static readonly string CrashLog = PluginFolder + @"\crashlog.txt";

        public static PluginHost Host { get; set; }
        public static CoreManager Core { get; set; }
        public static string PluginName { get; set; }
        public static string CharacterName { get; set; }
        internal static string AccountName { get; set; }
        public static void SaveSetting(string module, string characterName, string setting, string value)
        {
            if (characterName.Contains(" "))
            {
                characterName = characterName.Replace(" ", "_");
            }
            try
            {
                if (File.Exists(SettingsFile))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(SettingsFile);

                    XmlNode node = doc.SelectSingleNode(String.Format(@"/Settings/{0}", AccountName));
                    if (node != null)
                    {
                        // account exists
                        node = doc.SelectSingleNode(String.Format(@"/Settings/{0}/{1}", AccountName, module));
                        if (node != null)
                        {
                            // module exists
                            node = doc.SelectSingleNode(String.Format(@"/Settings/{0}/{1}/Characters/{2}", AccountName, module, characterName));
                            if (node != null)
                            {
                                // character exists
                                node = doc.SelectSingleNode(String.Format(@"/Settings/{0}/{1}/Characters/{2}/{3}", AccountName, module, characterName, setting));
                                if (node != null)
                                {
                                    // setting exists
                                    node.InnerText = value;
                                }
                                else
                                {
                                    // setting does not exist
                                    node = doc.SelectSingleNode(String.Format(@"/Settings/{0}/{1}/Characters/{2}", AccountName, module, characterName));
                                    XmlNode newSetting = doc.CreateNode(XmlNodeType.Element, setting, string.Empty);
                                    newSetting.InnerText = value;
                                    node.AppendChild(newSetting);
                                }
                            }
                            else
                            {
                                // character does not exist
                                node = doc.SelectSingleNode(String.Format(@"/Settings/{0}/{1}/Characters", AccountName, module));
                                XmlNode newCharacterNode = doc.CreateNode(XmlNodeType.Element, characterName, string.Empty);
                                XmlNode newSetting = doc.CreateNode(XmlNodeType.Element, setting, string.Empty);
                                newSetting.InnerText = value;
                                newCharacterNode.AppendChild(newSetting);
                                node.AppendChild(newCharacterNode);
                            }
                        }
                        else
                        {
                            // module does not exist
                            node = doc.SelectSingleNode(String.Format(@"/Settings/{0}", AccountName));
                            XmlNode newModule = doc.CreateNode(XmlNodeType.Element, module, string.Empty);
                            XmlNode newCharacters = doc.CreateNode(XmlNodeType.Element, "Characters", string.Empty);
                            XmlNode newCharacterNode = doc.CreateNode(XmlNodeType.Element, characterName, string.Empty);
                            XmlNode newSetting = doc.CreateNode(XmlNodeType.Element, setting, string.Empty);
                            newSetting.InnerText = value;
                            newCharacterNode.AppendChild(newSetting);
                            newCharacters.AppendChild(newCharacterNode);
                            newModule.AppendChild(newCharacters);
                            node.AppendChild(newModule);
                        }
                    }
                    else
                    {
                        // account doesn't exist
                        node = doc.SelectSingleNode(@"/Settings");
                        XmlNode newAccount = doc.CreateNode(XmlNodeType.Element, AccountName, string.Empty);
                        XmlNode newModule = doc.CreateNode(XmlNodeType.Element, module, string.Empty);
                        XmlNode newCharacters = doc.CreateNode(XmlNodeType.Element, "Characters", string.Empty);
                        XmlNode newCharacterNode = doc.CreateNode(XmlNodeType.Element, characterName, string.Empty);
                        XmlNode newSetting = doc.CreateNode(XmlNodeType.Element, setting, string.Empty);
                        newSetting.InnerText = value;
                        newCharacterNode.AppendChild(newSetting);
                        newCharacters.AppendChild(newCharacterNode);
                        newModule.AppendChild(newCharacters);
                        newAccount.AppendChild(newModule);
                        node.AppendChild(newAccount);
                    }

                    doc.Save(SettingsFile);
                }
                else
                {
                    // file does not exist
                    using(XmlWriter writer = XmlWriter.Create(SettingsFile, SetupXmlWriter()))
                    {

                        writer.WriteStartDocument();
                        writer.WriteStartElement("Settings");

                        writer.WriteStartElement(AccountName);
                        writer.WriteStartElement(module);
                        writer.WriteStartElement("Characters");
                        writer.WriteStartElement(characterName);
                        writer.WriteStartElement(setting);
                        writer.WriteString(value);

                        writer.WriteEndDocument();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToChat(ex.Message);
            }
        }

        public static XmlNode LoadCharacterSettings(string module, bool portal=false, string characterName="")
        {
            if (characterName.Contains(" "))
            {
                characterName = characterName.Replace(" ", "_");
            }

            XmlNode node = null;
            if (File.Exists(SettingsFile))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(SettingsFile);
                if (portal == false)
                {
                    node = doc.SelectSingleNode(String.Format(@"/Settings/{0}/{1}/Characters/{2}", AccountName,  module, characterName));
                } else
                {
                    node = doc.SelectSingleNode(String.Format(@"/Settings/{0}/{1}/Characters", AccountName, module));
                }
            }
            return node;
        }

        private static XmlWriterSettings SetupXmlWriter()
        {
            return new XmlWriterSettings
            {
                Indent = true
            };
        }

        public static void WriteToChat(string message)
        {
            try
            {
                Host.Actions.AddChatText(" <{ " + PluginName + " }>: " + message, 5);
            }
            catch (Exception ex) { LogError(ex); }
        }

        public static void LogError(Exception ex)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(ErrorFile, true))
                {
                    writer.WriteLine("============================================================================");
                    writer.WriteLine(DateTime.UtcNow.ToString());
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

        public static void LogCrash(string characterName, string duration, string xp, string reasonIfKnown="Crash")
        {
            try
            {
                if (!characterName.Equals(""))
                {
                    using (StreamWriter writer = new StreamWriter(CrashLog, true))
                    {
                        writer.WriteLine(DateTime.UtcNow.ToString() + " -" +
                            " Character=" + characterName + 
                            " Duration=" + duration + 
                            " XP=" + xp + 
                            " Reason=" + reasonIfKnown);
                    }
                }
            }
            catch (Exception ex) { LogError(ex); }
        }
    }
}
