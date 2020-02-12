using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Decal.Adapter;
using Decal.Adapter.Wrappers;

namespace ACManager
{
    public static class ChatParser
    {
        public static PluginHost Host { get; set; }
        public static CoreManager Core { get; set; }
        public static FellowshipControl FellowshipControl { get; set; }
        public static bool AutoRespond { get; set; }
        public static bool AutoFellow { get; set; }

        public static void Initialize(PluginHost host, CoreManager core, FellowshipControl control)
        {
            Host = host;
            Core = core;
            FellowshipControl = control;
        }

        public static void Parse(object sender, ChatTextInterceptEventArgs e)
        {
            string sanitizedInput = Regex.Replace(e.Text, @"[^\w:/ ']", string.Empty);
            if (AutoRespond)
            {
                // refactor to go to InventoryTracker
                Match match;
                string singleCompResponse = "/t {0}, I currently have {1} {2}.";
                string pluralCompResponse = "/t {0}, I currently have {1} {2}s.";
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

                // checking spell components
                string componentsPattern = string.Format(@"(?<guid>\d+):(?<dupleName>.+?)Tell\s(?<msg>tells)\syou\s(?<secret>.*)");

                match = new Regex(componentsPattern).Match(sanitizedInput);
                if (match.Success)
                {
                    string name = match.Groups["dupleName"].Value.Substring(0, match.Groups["dupleName"].Value.Length / 2);
                    if (match.Groups["secret"].Value.Equals("comps"))
                    {
                        string[] comps = {
                "Lead Scarab",
                "Iron Scarab",
                "Copper Scarab",
                "Silver Scarab",
                "Gold Scarab",
                "Pyreal Scarab",
                "Platinum Scarab",
                "Mana Scarab",
                "Prismatic Taper"
                };
                        foreach (string comp in comps)
                        {
                            WorldObjectCollection collection = Core.WorldFilter.GetInventory();
                            collection.SetFilter(new ByNameFilter(comp));
                            if (collection.Quantity == 0) continue;
                            Host.Actions.InvokeChatParser(
                                collection.Quantity == 1 ?
                                string.Format(singleCompResponse, name, collection.Quantity.ToString(), collection.First.Name) :
                                string.Format(pluralCompResponse, name, collection.Quantity.ToString(), collection.First.Name)
                            );
                        }
                    }
                    else
                    {
                        WorldObjectCollection collection = Core.WorldFilter.GetInventory();
                        collection.SetFilter(new ByNameFilter(textInfo.ToTitleCase(match.Groups["secret"].Value)));
                        if (collection.Count > 0)
                        {
                            Host.Actions.InvokeChatParser(collection.Quantity == 1 ?
                                string.Format(singleCompResponse, name, collection.Quantity.ToString(), collection.First.Name) :
                                string.Format(pluralCompResponse, name, collection.Quantity.ToString(), collection.First.Name)
                            );
                        }
                    }
                }
            }
            
            if (AutoFellow)
            {
                Dictionary<string, string> AutoFellowStrings = new Dictionary<string, string>
                {
                    ["CreatedFellow"] = @"^You have created the Fellowship",
                    ["NewLeader"] = string.Format(@"{0} now leads the fellowship", Utility.CharacterName),
                    ["NotAccepting"] = @"(?<name>.+?) is not accepting fellowing requests",
                    ["JoinedFellow"] = @"(?<name>.+?) joined the fellowship",
                    ["ElseLeftFellow"] = @"(?<name>.+?) left the fellowship",
                    ["RequestFellow"] = string.Format(@"(?<guid>\d+):(?<dupleName>.+?)Tell\s(?<msg>tells)\syou\s(?<secret>{0}$)", FellowshipControl.Password),
                    ["DeclinedFellow"] = @"(?<name>.+?) declines your invite",
                    ["AlreadyFellow"] = @"(?<name>.+?) is already in a fellowship",
                    ["FullFellow"] = @"^Fellowship is already full"
                };

                Match match;

                foreach (var item in AutoFellowStrings)
                {
                    match = new Regex(item.Value).Match(sanitizedInput);
                    if (match.Success)
                    {
                        switch (item.Key)
                        {
                            case "CreatedFellow": // You have created the Fellowship
                                Host.Actions.FellowshipSetOpen(true);
                                break;
                            case "NewLeader": // You inherited leader
                                Host.Actions.FellowshipSetOpen(true);
                                break;
                            case "NotAccepting": // Target is not accepting fellowship requests
                                Host.Actions.InvokeChatParser(string.Format("/t {0}, You are not accepting fellowship requests!", match.Groups["name"].Value));
                                FellowshipControl.StopTimer();
                                break;
                            case "JoinedFellow": // Target joins the fellowship
                                    FellowshipControl.StopTimer();
                                break;
                            case "ElseLeftFellow": // Someone leaves the fellowship
                                break;
                            case "FullFellow": // Fellowship is currently full
                                Host.Actions.InvokeChatParser(String.Format("/t {0}, The fellowship is already full.", match.Groups["name"].Value));
                                FellowshipControl.StopTimer();
                                break;
                            case "RequestFellow": // Someone /tells you the fellowship password
                                if (match.Groups["msg"].Value.Equals("tells") && match.Groups["secret"].Value.Equals(FellowshipControl.Password))
                                {
                                    string targetRecruit = match.Groups["dupleName"].Value.Substring(0, match.Groups["dupleName"].Value.Length / 2);
                                    int targetGuid = int.Parse(match.Groups["guid"].Value);
                                    FellowshipControl.InviteRequested(targetGuid, targetRecruit);
                                }
                                break;
                            case "DeclinedFellow": // Declines fellowship invite
                                    FellowshipControl.StopTimer();
                                break;
                            case "AlreadyFellow": // Target is already in a fellowship
                                Host.Actions.InvokeChatParser(String.Format("/t {0}, You're already in a fellowship.", match.Groups["name"].Value));
                                FellowshipControl.StopTimer();
                                break;
                        }
                        break;
                    }
                }
            }
        }
    }
}


