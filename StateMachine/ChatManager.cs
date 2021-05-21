using ACManager.Settings;
using Decal.Adapter;
using Decal.Filters;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ACManager.StateMachine
{
    /// <summary>
    /// Class to control all chat parsing while the bot is in operation. This handles tells, general chat, sending tells, and broadcasting advertisements.
    /// </summary>
    public class ChatManager
    {
        private Machine Machine { get; set; }
        private readonly Regex RegexLocal = new Regex("^<Tell:IIDString:(?<guid>\\d+):(?<name>.+?)>.*says, \"(?<message>.*)\"");
        private readonly Regex RegexTell = new Regex("^<Tell:IIDString:(?<guid>\\d+):(?<name>.+?)>.*tells you, \"(?<message>.*)\"");
        private readonly Regex RegexAllegiance = new Regex("^\\[Allegiance] <Tell:IIDString:(?<guid>0):(?<name>.+?)>.*says, \"(?<message>.*)\"");
        private readonly Regex RegexGift = new Regex("(?<name>.+?) gives you (?<gift>.*?)\\.");
        private readonly Spell PrimaryPortal = CoreManager.Current.Filter<FileService>().SpellTable.GetById(157);
        private readonly Spell SecondaryPortal = CoreManager.Current.Filter<FileService>().SpellTable.GetById(2648);

        /// <summary>
        /// Instantiates the ChatManager and sets the public state machine instance to this.
        /// </summary>
        public ChatManager(Machine machine)
        {
            Machine = machine;
        }

        /// <summary>
        /// The parser of all messages. It is subscribed to only while the bot is in operation.
        /// </summary>
        public void Current_ChatBoxMessage(object sender, ChatTextInterceptEventArgs e)
        {
            Match match = RegexLocal.Match(e.Text);
            if (match.Success)
            {
                if (Machine.RespondToOpenChat) { Handle(match, false); }
                return;
            }

            match = RegexAllegiance.Match(e.Text);
            if (match.Success)
            {
                if (Machine.RespondToAllegiance) { Handle(match, false); }
                return;
            }

            match = RegexTell.Match(e.Text);
            if (match.Success)
            {
                Handle(match, true);
                return;
            }

            match = RegexGift.Match(e.Text);
            if (match.Success)
            {
                SendTell(match.Groups["name"].Value, $"Thank you for the {match.Groups["gift"].Value}!");
                Utility.SaveGiftToLog(e.Text);
                return;
            }
        }

        /// <summary>
        /// Handles all messages sent through general chat.
        /// </summary>
        private void Handle(Match match, bool isTell)
        {
            int guid = int.Parse(match.Groups["guid"].Value);
            string name = match.Groups["name"].Value;
            string message = match.Groups["message"].Value.ToLower();

            switch (message)
            {
                case "whereto":
                    List<string> responeStrings = new List<string>();
                    responeStrings.AddRange(Utility.GetPortals());
                    responeStrings.AddRange(Utility.GetGems());
                    if (responeStrings.Count > 0)
                    {
                        SendTell(name, "You can tell me any keyword, and I will summon the corresponding portal.");
                        if (Machine.Verbosity > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            int count = 0;
                            for (int i = 0; i < responeStrings.Count; i++)
                            {
                                sb.Append(responeStrings[i]);
                                count++;
                                if (count.Equals(Machine.Verbosity + 1))
                                {
                                    SendTell(name, sb.ToString());
                                    sb.Length = 0;
                                    sb.Capacity = 0;
                                    sb.Capacity = 16;
                                    count = 0;
                                }
                                else if (count < Machine.Verbosity + 1 && !i.Equals(responeStrings.Count - 1))
                                {
                                    sb.Append(", ");
                                }

                                if (i.Equals(responeStrings.Count - 1) && !string.IsNullOrEmpty(sb.ToString()))
                                {
                                    SendTell(name, sb.ToString());
                                    sb.Length = 0;
                                    sb.Capacity = 0;
                                }
                            }
                        }
                        else
                        {
                            foreach (string response in responeStrings)
                            {
                                SendTell(name, response);
                            }
                        }
                    }
                    else
                    {
                        SendTell(name, "I don't currently have any portals configured.");
                    }
                    break;
                case "profiles":
                    if (isTell && !string.IsNullOrEmpty(Machine.BuffingCharacter))
                    {
                        List<string> profiles = new List<string>();
                        foreach (BuffProfile profile in Utility.BuffProfiles)
                        {
                            foreach (string command in profile.Commands)
                            {
                                profiles.Add(command);
                            }
                        }

                        StringBuilder sb = new StringBuilder();
                        sb.Append("My profile commands are: ");
                        for (int i = 0; i < profiles.Count; i++)
                        {
                            if (!profiles[i].Equals("botbuffs"))
                            {
                                if (!i.Equals(profiles.Count - 1))
                                {
                                    sb.Append($"{profiles[i]}");
                                    sb.Append(", ");
                                }
                                else
                                {
                                    if (profiles.Count.Equals(1))
                                    {
                                        sb.Append($"{profiles[i]}.");
                                    }
                                    else
                                    {
                                        sb.Append($"and {profiles[i]}.");
                                    }
                                }
                            }
                        }

                        SendTell(name, sb.ToString());
                    }                    
                    break;
                case "cancel":
                    if (isTell)
                    {
                        Machine.CancelRequest(name);
                    }
                    break;
                case "comps":
                    Dictionary<string, int> components = Inventory.GetComponentLevels();
                    if (components.Count > 0)
                    {
                        string[] keys = new string[components.Count];
                        int[] values = new int[components.Count];
                        components.Keys.CopyTo(keys, 0);
                        components.Values.CopyTo(values, 0);
                        StringBuilder sb = new StringBuilder();
                        sb.Append("I currently have ");
                        for (int i = 0; i < components.Count; i++)
                        {
                            if (i.Equals(components.Count - 1) && components.Count > 1)
                            {
                                sb.Append($"and ");
                            }
                            sb.Append($"{values[i]} {keys[i]}{(values[i] > 1 ? "s" : "")}");
                            if (i < components.Count - 1)
                            {
                                sb.Append($", ");
                            }
                            if (i.Equals(components.Count - 1))
                            {
                                sb.Append(".");
                            }
                        }
                        SendTell(name, sb.ToString());
                    }
                    else
                    {
                        SendTell(name, "I don't have any components.");
                    }
                    break;
                case "help":
                    if (!string.IsNullOrEmpty(Machine.BuffingCharacter))
                    {
                        SendTell(name, "My list of commands are: 'profiles', 'whereto', and 'comps'.");
                    }
                    else
                    {
                        SendTell(name, "My list of commands are: 'whereto', and 'comps'.");
                    }
                    break;
                default:
                    for (int i = 0; i < Utility.CharacterSettings.Characters.Count; i++)
                    {
                        foreach (Portal portal in Utility.CharacterSettings.Characters[i].Portals)
                        {
                            if (portal.Keyword.Equals(message))
                            {
                                Request newRequest = new Request
                                {
                                    RequestType = RequestType.Portal,
                                    RequesterName = name,
                                    Destination = portal.Description,
                                    Heading = portal.Heading,
                                    Character = Utility.CharacterSettings.Characters[i].Name                                    
                                };

                                if (portal.Type.Equals(PortalType.Primary))
                                {
                                    newRequest.SpellsToCast.Add(PrimaryPortal);
                                }
                                else if (portal.Type.Equals(PortalType.Secondary))
                                {
                                    newRequest.SpellsToCast.Add(SecondaryPortal);
                                }
                                Machine.AddToQueue(newRequest);
                                return;
                            }
                        }
                    }

                    foreach (GemSetting gemSetting in Utility.BotSettings.GemSettings)
                    {
                        if (gemSetting.Keyword.Equals(message))
                        {
                            Request newRequest = new Request
                            {
                                RequestType = RequestType.Gem,
                                RequesterName = name,
                                Destination = gemSetting.Name,
                                Heading = gemSetting.Heading,
                                ItemToUse = gemSetting.Name
                            };

                            if (Inventory.GetInventoryCount(gemSetting.Name) > 0)
                            {
                                newRequest.Character = CoreManager.Current.CharacterFilter.Name;
                            }
                            else
                            {
                                for (int i = 0; i < Utility.Inventory.CharacterInventories.Count; i++)
                                {
                                    for (int j = 0; j < Utility.Inventory.CharacterInventories[i].Gems.Count; j++)
                                    {
                                        if (Utility.Inventory.CharacterInventories[i].Gems[j].Name.Equals(gemSetting.Name))
                                        {
                                            newRequest.Character = Utility.Inventory.CharacterInventories[i].Name;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (string.IsNullOrEmpty(newRequest.Character))
                            {
                                SendTell(name, $"It appears I've run out of {newRequest.Destination}.");
                            } else
                            {
                                Machine.AddToQueue(newRequest);
                            }
                            return;
                        }
                    }

                    if (!string.IsNullOrEmpty(Machine.BuffingCharacter) && isTell)
                    {
                        if (!message.Equals("botbuffs"))
                        {
                            foreach (BuffProfile profile in Utility.BuffProfiles)
                            {
                                if (profile.Commands.Contains(message))
                                {
                                    Request newRequest = new Request
                                    {
                                        RequestType = RequestType.Buff,
                                        Character = Machine.BuffingCharacter,
                                        RequesterName = name,
                                        RequesterGuid = guid
                                    };

                                    foreach (Buff buff in profile.Buffs)
                                    {
                                        newRequest.SpellsToCast.Add(CoreManager.Current.Filter<FileService>().SpellTable.GetById(buff.Id));
                                    }
                                    Machine.AddToQueue(newRequest);
                                    return;
                                }
                            }
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Send a tell to someone. 
        /// </summary>
        /// <param name="target">Character name you wish to send a tell to.</param>
        /// <param name="message">Message to send to the character.</param>
        public static void SendTell(string target, string message)
        {
            if (!string.IsNullOrEmpty(target) && !string.IsNullOrEmpty(message))
            {
                CoreManager.Current.Actions.InvokeChatParser($"/t {target}, {message}");
            }
        }

        /// <summary>
        /// Sends a message to local area chat.
        /// </summary>
        /// <param name="message">Message to say out in the open.</param>
        public static void Broadcast(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                CoreManager.Current.Actions.InvokeChatParser(message);
            }
        }
    }
}
