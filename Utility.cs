using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.IO;
using System.Xml;

namespace ACManager
{
    public static class Utility
    {
        private static readonly string SettingsFile = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Asheron's Call\" + "FMsettings.xml";
        public static PluginHost Host { get; set; }
        public static CoreManager Core { get; set; }
        public static string PluginName { get; set; }
        public static string CharacterName { get; set; }

        public static void SaveSetting(string module, string setting, string value)
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(SettingsFile);

                    XmlNode node = doc.SelectSingleNode(String.Format(@"/Settings/{0}", module));

                    if (node != null)
                    {
                        // module exists
                        node = doc.SelectSingleNode(String.Format(@"/Settings/{0}/Characters/{1}", module, CharacterName));
                        if (node != null)
                        {
                            // character exists
                            node = doc.SelectSingleNode(String.Format(@"/Settings/{0}/Characters/{1}/{2}", module, CharacterName, setting));
                            if (node != null)
                            {
                                // setting exists
                                node.InnerText = value;
                            }
                            else
                            {
                                // setting does not exist
                                node = doc.SelectSingleNode(String.Format(@"/Settings/{0}/Characters/{1}", module, CharacterName));
                                XmlNode newSetting = doc.CreateNode(XmlNodeType.Element, setting, string.Empty);
                                newSetting.InnerText = value;
                                node.AppendChild(newSetting);
                            }
                        }
                        else
                        {
                            // character does not exist
                            node = doc.SelectSingleNode(String.Format(@"/Settings/{0}/Characters", module));
                            XmlNode newCharacterNode = doc.CreateNode(XmlNodeType.Element, CharacterName, string.Empty);
                            XmlNode newSetting = doc.CreateNode(XmlNodeType.Element, setting, string.Empty);
                            newSetting.InnerText = value;
                            newCharacterNode.AppendChild(newSetting);
                            node.AppendChild(newCharacterNode);
                        }
                    }
                    else
                    {
                        // module does not exist
                        node = doc.SelectSingleNode("/Settings");
                        XmlNode newModule = doc.CreateNode(XmlNodeType.Element, module, string.Empty);
                        XmlNode newCharacters = doc.CreateNode(XmlNodeType.Element, "Characters", string.Empty);
                        XmlNode newCharacterNode = doc.CreateNode(XmlNodeType.Element, CharacterName, string.Empty);
                        XmlNode newSetting = doc.CreateNode(XmlNodeType.Element, setting, string.Empty);
                        newSetting.InnerText = value;
                        newCharacterNode.AppendChild(newSetting);
                        newCharacters.AppendChild(newCharacterNode);
                        newModule.AppendChild(newCharacters);
                        node.AppendChild(newModule);
                    }

                    doc.Save(SettingsFile);
                }
                else
                {
                    // file does not exist
                    XmlWriter writer = XmlWriter.Create(SettingsFile, SetupXmlWriter());
                    writer.WriteStartDocument();
                    writer.WriteStartElement("Settings");

                    writer.WriteStartElement(module);
                    writer.WriteStartElement("Characters");

                    writer.WriteStartElement(CharacterName);

                    writer.WriteStartElement(setting);
                    writer.WriteString(value);

                    writer.WriteEndDocument();
                    writer.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        public static XmlNode LoadCharacterSettings(string module)
        {
            XmlNode node = null;
            if (File.Exists(SettingsFile))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(SettingsFile);
                switch (module)
                {
                    case "General":
                        node = doc.SelectSingleNode(String.Format(@"/Settings/{0}/Characters/{1}", module, CharacterName));
                        break;
                    case "FellowshipManager":
                        node = doc.SelectSingleNode(String.Format(@"/Settings/{0}/Characters/{1}", module, CharacterName));
                        break;
                    case "InventoryTracker":
                        node = doc.SelectSingleNode(String.Format(@"/Settings/{0}/Characters/{1}", module, CharacterName));
                        break;
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
                Host.Actions.AddChatText("<{" + PluginName + "}>: " + message, 5);
            }
            catch (Exception ex) { LogError(ex); }
        }

        public static void LogError(Exception ex)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Asheron's Call\" + PluginName + " errors.txt", true))
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
                    writer.Close();
                }
            }
            catch
            {
            }
        }
    }
}
