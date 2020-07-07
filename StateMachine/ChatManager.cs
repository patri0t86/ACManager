using ACManager.Settings;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ACManager.StateMachine
{
    /// <summary>
    /// Class to control all chat parsing while the bot is in operation. This handles tells, general chat, sending tells, and broadcasting advertisements.
    /// </summary>
    internal class ChatManager
    {
        /// <summary>
        /// A reference to the state machine instance.
        /// </summary>
        private Machine Machine { get; set; }

        /// <summary>
        /// Instantiates the ChatCommandManager and sets the internal state machine instance to this.
        /// </summary>
        /// <param name="machine"></param>
        public ChatManager(Machine machine)
        {
            Machine = machine;
        }

        /// <summary>
        /// The parser of all messages. It is subscribed to only while the bot is in operation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Current_ChatBoxMessage(object sender, ChatTextInterceptEventArgs e)
        {
            string text = Regex.Replace(e.Text.ToLower(), @"[^\w:/ ']", string.Empty);

            // Create a new regex match for regular chat
            Match match = new Regex("(?<guid>\\d+):(?<dupleName>.+?)tell says (?<message>.*$)").Match(text);
            if (match.Success)
            {
                HandleChat(match);
            }

            // Create a new regex match for general tells
            match = new Regex("(?<guid>\\d+):(?<dupleName>.+?)tell tells you (?<message>.*$)").Match(text);
            if (match.Success)
            {
                HandleTell(match);
            }
        }

        /// <summary>
        /// Handles all messages sent through general chat.
        /// </summary>
        /// <param name="match"></param>
        private void HandleChat(Match match)
        {
            // The actual message
            string message = match.Groups["message"].Value;

            // The GUID of the player sending the tell
            int guid = Convert.ToInt32(match.Groups["guid"].Value);

            if (!guid.Equals(0)) // guid = 0 is said in general/trade/etc. not in local chat
            {
                Machine.CharacterMakingRequest = match.Groups["dupleName"].Value.Substring(0, match.Groups["dupleName"].Value.Length / 2);

                if ((message.Equals("whereto") || message.Equals("where to")) && Machine.Update())
                {
                    RespondWithPortals();
                }

                if (Machine.RespondToOpenChat && !Machine.DecliningCommands)
                {
                    CheckCommands(message);
                }
            }
        }

        /// <summary>
        /// Handles all direct tells.
        /// </summary>
        /// <param name="match"></param>
        private void HandleTell(Match match)
        {
            // The actual message
            string message = match.Groups["message"].Value;

            // The GUID of the player sending the tell
            int guid = Convert.ToInt32(match.Groups["guid"].Value);

            // The in-game character name sending the tell
            Machine.CharacterMakingRequest = match.Groups["dupleName"].Value.Substring(0, match.Groups["dupleName"].Value.Length / 2);

            if (!string.IsNullOrEmpty(message) && Machine.Update())
            {
                //bool commandExists = false;
                if (message.Equals("whereto") || message.Equals("where to"))
                {
                    RespondWithPortals();
                }
                else if (message.Equals("help"))
                {
                    SendTell(Machine.CharacterMakingRequest, "You can /t me 'whereto' to get a list of portals. Then /t me any keyword for me to summon the portal you request.");
                }
                else if (message.Equals("comps"))
                {
                    for (int i = 0; i < Machine.Core.Filter<FileService>().ComponentTable.Length; i++)
                    {
                        using (WorldObjectCollection collection = Machine.Core.WorldFilter.GetInventory())
                        {
                            collection.SetFilter(new ByNameFilter(Machine.Core.Filter<FileService>().ComponentTable[i].Name));
                            if (collection.Quantity.Equals(0))
                            {
                                continue;
                            }
                            else
                            {
                                if (collection.First.Name.Contains("Scarab"))
                                {
                                    switch (collection.First.Name)
                                    {
                                        case "Lead Scarab":
                                            SendTell(Machine.CharacterMakingRequest, $"I have {collection.Quantity} {collection.First.Name}{(collection.Quantity > 1 ? "s" : "")}. {(collection.Quantity <= Machine.Inventory.LeadScarabThreshold ? "(low)" : "")}");
                                            break;
                                        case "Iron Scarab":
                                            SendTell(Machine.CharacterMakingRequest, $"I have {collection.Quantity} {collection.First.Name}{(collection.Quantity > 1 ? "s" : "")}. {(collection.Quantity <= Machine.Inventory.IronScarabThreshold ? "(low)" : "")}");
                                            break;
                                        case "Copper Scarab":
                                            SendTell(Machine.CharacterMakingRequest, $"I have {collection.Quantity} {collection.First.Name}{(collection.Quantity > 1 ? "s" : "")}. {(collection.Quantity <= Machine.Inventory.CopperScarabThreshold ? "(low)" : "")}");
                                            break;
                                        case "Silver Scarab":
                                            SendTell(Machine.CharacterMakingRequest, $"I have {collection.Quantity} {collection.First.Name}{(collection.Quantity > 1 ? "s" : "")}. {(collection.Quantity <= Machine.Inventory.SilverScarabThreshold ? "(low)" : "")}");
                                            break;
                                        case "Gold Scarab":
                                            SendTell(Machine.CharacterMakingRequest, $"I have {collection.Quantity} {collection.First.Name}{(collection.Quantity > 1 ? "s" : "")}. {(collection.Quantity <= Machine.Inventory.GoldScarabThreshold ? "(low)" : "")}");
                                            break;
                                        case "Pyreal Scarab":
                                            SendTell(Machine.CharacterMakingRequest, $"I have {collection.Quantity} {collection.First.Name}{(collection.Quantity > 1 ? "s" : "")}. {(collection.Quantity <= Machine.Inventory.PyrealScarabThreshold ? "(low)" : "")}");
                                            break;
                                        case "Platinum Scarab":
                                            SendTell(Machine.CharacterMakingRequest, $"I have {collection.Quantity} {collection.First.Name}{(collection.Quantity > 1 ? "s" : "")}. {(collection.Quantity <= Machine.Inventory.PlatinumScarabThreshold ? "(low)" : "")}");
                                            break;
                                        case "Mana Scarab":
                                            SendTell(Machine.CharacterMakingRequest, $"I have {collection.Quantity} {collection.First.Name}{(collection.Quantity > 1 ? "s" : "")}. {(collection.Quantity <= Machine.Inventory.ManaScarabThreshold ? "(low)" : "")}");
                                            break;
                                    }
                                }
                                else
                                {
                                    SendTell(Machine.CharacterMakingRequest, $"I have {collection.Quantity} {collection.First.Name}{(collection.Quantity > 1 ? "s" : "")}. {(collection.Quantity <= Machine.Inventory.ComponentThreshold ? "(low)" : "")}");
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!Machine.DecliningCommands)
                    {
                        CheckCommands(message);
                    }
                    else
                    {
                        SendTell(Machine.CharacterMakingRequest, $"I'm finishing up a request. Once I'm done, please make your request again.");
                    }
                }
            }
        }

        /// <summary>
        /// Responds with all available portals.
        /// </summary>
        private void RespondWithPortals()
        {
            List<string> portalStrings = GetPortals();
            List<string> gemStrings = GetGems();
            if (Machine.Verbosity > 0)
            {
                if (portalStrings.Count > 0)
                {
                    SendTell(Machine.CharacterMakingRequest, "You can /t me any keyword, and I will summon the corresponding portal.");
                    StringBuilder sb = new StringBuilder();
                    int count = 0;
                    for (int i = 0; i < portalStrings.Count; i++)
                    {
                        sb.Append(portalStrings[i]);
                        count++;
                        if (count.Equals(Machine.Verbosity + 1))
                        {
                            SendTell(Machine.CharacterMakingRequest, sb.ToString());
                            sb.Length = 0;
                            sb.Capacity = 0;
                            sb.Capacity = 16;
                            count = 0;
                        }
                        else if (count < Machine.Verbosity + 1 && !i.Equals(portalStrings.Count - 1))
                        {
                            sb.Append(", ");
                        }

                        if (i.Equals(portalStrings.Count - 1) && !string.IsNullOrEmpty(sb.ToString()))
                        {
                            SendTell(Machine.CharacterMakingRequest, sb.ToString());
                            sb.Length = 0;
                            sb.Capacity = 0;
                        }
                    }

                    if (gemStrings.Count > 0)
                    {
                        sb.Length = 0;
                        sb.Capacity = 0;
                        sb.Capacity = 16;
                        count = 0;
                        for (int i = 0; i < gemStrings.Count; i++)
                        {
                            sb.Append(gemStrings[i]);
                            count++;
                            if (count.Equals(Machine.Verbosity + 1))
                            {
                                SendTell(Machine.CharacterMakingRequest, sb.ToString());
                                sb.Length = 0;
                                sb.Capacity = 0;
                                sb.Capacity = 16;
                                count = 0;
                            }
                            else if (count < Machine.Verbosity + 1 && !i.Equals(gemStrings.Count - 1))
                            {
                                sb.Append(", ");
                            }

                            if (i.Equals(gemStrings.Count - 1) && !string.IsNullOrEmpty(sb.ToString()))
                            {
                                SendTell(Machine.CharacterMakingRequest, sb.ToString());
                                sb.Length = 0;
                                sb.Capacity = 0;
                            }
                        }
                    }
                }
                else if (gemStrings.Count > 0)
                {
                    SendTell(Machine.CharacterMakingRequest, "You can /t me any keyword, and I will summon the corresponding portal.");
                    StringBuilder sb = new StringBuilder();
                    int count = 0;
                    for (int i = 0; i < gemStrings.Count; i++)
                    {
                        sb.Append(gemStrings[i]);
                        count++;
                        if (count.Equals(Machine.Verbosity + 1))
                        {
                            SendTell(Machine.CharacterMakingRequest, sb.ToString());
                            sb.Length = 0;
                            sb.Capacity = 0;
                            sb.Capacity = 16;
                            count = 0;
                        }
                        else if (count < Machine.Verbosity + 1 && !i.Equals(gemStrings.Count - 1))
                        {
                            sb.Append(", ");
                        }

                        if (i.Equals(gemStrings.Count - 1) && !string.IsNullOrEmpty(sb.ToString()))
                        {
                            SendTell(Machine.CharacterMakingRequest, sb.ToString());
                            sb.Length = 0;
                            sb.Capacity = 0;
                        }
                    }
                }
                else
                {
                    SendTell(Machine.CharacterMakingRequest, "I don't currently have any portals configured.");
                }
            }
            else
            {
                if (portalStrings.Count > 0)
                {
                    SendTell(Machine.CharacterMakingRequest, "You can /t me any keyword, and I will summon the corresponding portal.");
                    foreach (string portal in portalStrings)
                    {
                        SendTell(Machine.CharacterMakingRequest, portal);
                    }

                    if (gemStrings.Count > 0)
                    {
                        foreach (string gem in gemStrings)
                        {
                            SendTell(Machine.CharacterMakingRequest, gem);
                        }
                    }
                }
                else if (gemStrings.Count > 0)
                {
                    foreach (string gem in gemStrings)
                    {
                        SendTell(Machine.CharacterMakingRequest, gem);
                    }
                }
                else
                {
                    SendTell(Machine.CharacterMakingRequest, "I don't currently have any portals configured.");
                }
            }
        }

        /// <summary>
        /// Builds the list of portals to respond with.
        /// </summary>
        /// <returns>List of formatted strings to be sent to the requestor.</returns>
        private List<string> GetPortals()
        {
            List<string> portalStrings = new List<string>();
            for (int i = 0; i < Machine.Utility.CharacterSettings.Characters.Count; i++)
            {
                foreach (Portal portal in Machine.Utility.CharacterSettings.Characters[i].Portals)
                {
                    if (!string.IsNullOrEmpty(portal.Keyword))
                    {
                        portalStrings.Add($"{portal.Keyword} -> {(string.IsNullOrEmpty(portal.Description) ? "No description" : portal.Description)}{(portal.Level > 0 ? " [" + portal.Level + "+]" : "")}");
                    }
                }
            }
            return portalStrings;
        }

        private List<string> GetGems()
        {
            List<string> gemStrings = new List<string>();
            foreach (GemSetting gemSetting in Machine.Utility.BotSettings.GemSettings)
            {
                if (!string.IsNullOrEmpty(gemSetting.Keyword))
                {
                    gemStrings.Add($"{gemSetting.Keyword} -> {gemSetting.Name}");
                }
            }
            return gemStrings;
        }

        /// <summary>
        /// Determines the character that the portal is on and sets the appropriate variables on the machine to take action.
        /// </summary>
        /// <param name="message"></param>
        private void CheckCommands(string message)
        {
            Machine.Inventory.UpdateInventoryFile();
            for (int i = 0; i < Machine.Utility.CharacterSettings.Characters.Count; i++)
            {
                foreach (Portal portal in Machine.Utility.CharacterSettings.Characters[i].Portals)
                {
                    if (portal.Keyword.Equals(message))
                    {
                        Machine.PortalDescription = portal.Description;
                        Machine.NextHeading = portal.Heading;
                        Machine.NextCharacter = Machine.Utility.CharacterSettings.Characters[i].Name;
                        if (Machine.Core.CharacterFilter.Name.Equals(Machine.Utility.CharacterSettings.Characters[i].Name))
                        {
                            if (portal.Type.Equals(PortalType.Primary))
                            {
                                Machine.SpellsToCast.Add(157);
                            }
                            else if (portal.Type.Equals(PortalType.Secondary))
                            {
                                Machine.SpellsToCast.Add(2648);
                            }
                            break;
                        }
                        else
                        {
                            SendTell(Machine.CharacterMakingRequest, $"The requested portal{(string.IsNullOrEmpty(Machine.PortalDescription) ? " " : $" to {Machine.PortalDescription} ")}is on another character, please standby.");
                            Machine.GetNextCharacter();
                            Broadcast($"Be right back, switching to {Machine.NextCharacter} to summon{(string.IsNullOrEmpty(Machine.PortalDescription) ? "" : $" {Machine.PortalDescription}")}.");

                            if (portal.Type.Equals(PortalType.Primary))
                            {
                                Machine.SpellsToCast.Add(157);
                            }
                            else if (portal.Type.Equals(PortalType.Secondary))
                            {
                                Machine.SpellsToCast.Add(2648);
                            }
                            break;
                        }
                    }
                }
            }

            foreach (GemSetting gemSetting in Machine.Utility.BotSettings.GemSettings)
            {
                if (gemSetting.Keyword.Equals(message))
                {
                    Machine.PortalDescription = gemSetting.Name;
                    Machine.NextHeading = gemSetting.Heading;
                    Machine.ItemToUse = gemSetting.Name;
                    using (WorldObjectCollection inventory = Machine.Core.WorldFilter.GetInventory())
                    {
                        inventory.SetFilter(new ByNameFilter(gemSetting.Name));
                        if (inventory.Quantity > 0) // gem is on this character
                        {
                            Machine.NextCharacter = Machine.Core.CharacterFilter.Name;
                            break;
                        }
                    }

                    for (int i = 0; i < Machine.Utility.Inventory.CharacterInventories.Count; i++)
                    {
                        for (int j = 0; j < Machine.Utility.Inventory.CharacterInventories[i].Gems.Count; j++)
                        {
                            if (Machine.Utility.Inventory.CharacterInventories[i].Gems[j].Name.Equals(gemSetting.Name) && Machine.Utility.Inventory.CharacterInventories[i].Gems[j].Quantity > 0)
                            {
                                Machine.NextCharacter = Machine.Utility.Inventory.CharacterInventories[i].Name;
                                Machine.GetNextCharacter();
                                if (!Machine.NextCharacter.Equals(Machine.Core.CharacterFilter.Name))
                                {
                                    Broadcast($"Be right back, switching to {Machine.NextCharacter} to use {Machine.PortalDescription}.");
                                }
                                return;
                            }
                        }
                    }

                    // no gems found
                    Machine.NextCharacter = Machine.Core.CharacterFilter.Name;
                }
            }
        }

        /// <summary>
        /// Send a tell to someone. 
        /// </summary>
        /// <param name="target">Character name you wish to send a tell to.</param>
        /// <param name="message">Message to send to the character.</param>
        public void SendTell(string target, string message)
        {
            if (!string.IsNullOrEmpty(target) && !string.IsNullOrEmpty(message))
            {
                Machine.Core.Actions.InvokeChatParser($"/t {target}, {message}");
            }
        }

        /// <summary>
        /// Sends a message to local area chat.
        /// </summary>
        /// <param name="message">Message to say out in the open.</param>
        public void Broadcast(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Machine.Core.Actions.InvokeChatParser(message);
            }
        }
    }
}
