using System;
using System.IO;
using System.Security.Permissions;
using System.Xml;

using Decal.Adapter;
using Decal.Adapter.Wrappers;

namespace FellowshipManager
{
	public class Utility
	{
		private PluginCore Parent;
		private PluginHost Host;
		private CoreManager Core;
		private string PluginName, SettingsFile = "C:\\Turbine\\test.xml";
		public string SecretPassword { get; set; }
		public string AutoFellow { get; set; }
		public string AutoResponder { get; set; }

		public Utility(PluginCore parent, PluginHost host, CoreManager core, string PluginName)
		{
			Parent = parent;
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
		}

		private void AutoFellowHandler(object sender, ConfigEventArgs e)
		{
			AutoFellow = e.Value;
		}

		private void AutoResponderHandler(object sender, ConfigEventArgs e)
		{
			AutoResponder = e.Value;
		}

		public void SaveSettings()
		{
			if (File.Exists(SettingsFile))
			{
				File.SetAttributes(SettingsFile, FileAttributes.Normal);
				FileIOPermission filePermission =
							  new FileIOPermission(FileIOPermissionAccess.AllAccess, SettingsFile);
				using (FileStream fs = new FileStream(SettingsFile, FileMode.Create))
				{
					using (XmlWriter writer = XmlWriter.Create(fs, SetupXmlWriter()))
					{
						writer.WriteStartDocument();
						writer.WriteStartElement("settings");

						writer.WriteStartElement("secret_password");
						writer.WriteString(SecretPassword);
						writer.WriteEndElement();

						writer.WriteStartElement("auto_fellowship");
						writer.WriteString(AutoFellow);
						writer.WriteEndElement();

						writer.WriteStartElement("auto_responder");
						writer.WriteString(AutoResponder);
						writer.WriteEndElement();

						writer.WriteEndDocument();
						writer.Flush();
						writer.Close();
					}
					fs.Flush();
					fs.Close();
				}
			}
			else
			{
				XmlWriter writer = XmlWriter.Create(SettingsFile, SetupXmlWriter());
				writer.WriteStartDocument();
				writer.WriteStartElement("settings");

				writer.WriteStartElement("secret_password");
				writer.WriteString(SecretPassword);
				writer.WriteEndElement();

				writer.WriteStartElement("auto_fellowship");
				writer.WriteString(AutoFellow);
				writer.WriteEndElement();

				writer.WriteStartElement("auto_responder");
				writer.WriteString(AutoResponder);
				writer.WriteEndElement();

				writer.WriteEndDocument();
				writer.Flush();
				writer.Close();
			}
		}

		public void LoadSettingsFromFile()
		{
			XmlReader XmlReader = XmlReader.Create(SettingsFile, SetupXmlReader());
			while (XmlReader.Read())
			{
				if (XmlReader.IsStartElement())
				{
					if(XmlReader.Name.Equals("secret_password"))
					{
						XmlReader.Read();
						SecretPassword = XmlReader.ReadString();
					}
					if (XmlReader.Name.Equals("auto_fellowship"))
					{
						XmlReader.Read();
						AutoFellow = XmlReader.ReadString();
					}
					if (XmlReader.Name.Equals("auto_responder"))
					{
						XmlReader.Read();
						AutoResponder = XmlReader.ReadString();
					}
				}
			}
			XmlReader.Close();
		}

		private XmlWriterSettings SetupXmlWriter()
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			return settings;
		}

		private XmlReaderSettings SetupXmlReader()
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			return settings;
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
