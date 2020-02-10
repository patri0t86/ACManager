using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.IO;
using System.Xml;

namespace FellowshipManager
{
    public class Utility
    {
        private PluginHost Host;
        private CoreManager Core;
        private string PluginName;
        private readonly string SettingsFile = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Asheron's Call\" + "FMsettings.xml";

        public string SecretPassword { get; set; }
        public string AutoFellow { get; set; }
        public string AutoResponder { get; set; }
        public string CharacterName { get; set; }

        public Utility(PluginCore parent, PluginHost host, CoreManager core, string PluginName)
        {
            Host = host;
            Core = core;
            this.PluginName = PluginName;

            #region Settings Subscriptions
            parent.RaiseSecretPasswordEvent += SecretPasswordHandler;
            parent.RaiseAutoFellowEvent += AutoFellowHandler;
            parent.RaiseAutoResponderEvent += AutoResponderHandler;
            #endregion
        }

        private void SecretPasswordHandler(object sender, ConfigEventArgs e)
        {
            SecretPassword = e.Value;
            SaveSetting(e.Module, e.Setting, e.Value);
        }

        private void AutoFellowHandler(object sender, ConfigEventArgs e)
        {
            AutoFellow = e.Value;
            SaveSetting(e.Module, e.Setting, e.Value);
        }

        private void AutoResponderHandler(object sender, ConfigEventArgs e)
        {
            AutoResponder = e.Value;
            SaveSetting(e.Module, e.Setting, e.Value);
        }

        public void SaveSetting(string module, string setting, string value)
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

        public void LoadCharacterSettings()
        {
            if (File.Exists(SettingsFile))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(SettingsFile);

                #region Fellowship Manager Settings
                XmlNode characterNode = doc.SelectSingleNode(String.Format(@"/Settings/FellowshipManager/Characters/{0}", CharacterName));
                if (characterNode != null)
                {
                    // character exists
                    XmlNodeList settingNodes = characterNode.ChildNodes;
                    if (settingNodes.Count > 0)
                    {
                        foreach (XmlNode node in settingNodes)
                        {
                            switch (node.Name)
                            {
                                case "SecretPassword":
                                    SecretPassword = node.InnerText;
                                    break;
                                case "AutoFellow":
                                    AutoFellow = node.InnerText;
                                    break;
                                case "AutoRespond":
                                    AutoResponder = node.InnerText;
                                    break;
                            }
                        }
                    }
                }
                #endregion
            }
        }

        private XmlWriterSettings SetupXmlWriter()
        {
            return new XmlWriterSettings
            {
                Indent = true
            };
        }

        private XmlReaderSettings SetupXmlReader()
        {
            return new XmlReaderSettings();
        }

        public void WriteToChat(string message)
        {
            try
            {
                Host.Actions.AddChatText("<{" + PluginName + "}>: " + message, 5);
            }
            catch (Exception ex) { LogError(ex); }
        }

        public void LogError(Exception ex)
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
