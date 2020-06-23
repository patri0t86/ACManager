using Decal.Adapter;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ACManager
{
    internal class FellowshipControl
    {
        private FilterCore Filter { get; set; }
        private CoreManager Core { get; set; }
        private List<Recruit> Recruits { get; set; } = new List<Recruit>();
        internal FellowshipEventType FellowStatus { get; set; } = FellowshipEventType.Quit;
        internal DateTime LastAttempt { get; set; } = DateTime.MinValue;

        public FellowshipControl(FilterCore parent, CoreManager core)
        {
            Filter = parent;
            Core = core;
            Filter.MainView.AutoFellow.Checked = Filter.Machine.CurrentCharacter.AutoFellow;
            if (string.IsNullOrEmpty(Filter.Machine.CurrentCharacter.Password))
            {
                Filter.MainView.Password.Text = "xp";
            }
            else
            {
                Filter.MainView.Password.Text = Filter.Machine.CurrentCharacter.Password;
            }
        }

        public void ChatActions(object sender, ChatTextInterceptEventArgs e)
        {
            string message = Regex.Replace(e.Text, @"[^\w:/ ']", string.Empty);

            Dictionary<string, string> AutoFellowStrings = new Dictionary<string, string>
            {
                ["CreatedFellow"] = "^You have created the Fellowship",
                ["NewLeader"] = $"{Core.CharacterFilter.Name} now leads the fellowship",
                ["NotAccepting"] = "(?<name>.+?) is not accepting fellowing requests",
                ["JoinedFellow"] = "(?<name>.+?) joined the fellowship",
                ["ElseLeftFellow"] = "(?<name>.+?) left the fellowship",
                ["RequestFellow"] = $"(?<guid>\\d+):(?<dupleName>.+?)Tell\\stells\\syou\\s(?<secret>{Filter.MainView.Password.Text}$)",
                ["DeclinedFellow"] = "(?<name>.+?) declines your invite",
                ["AlreadyFellow"] = "(?<name>.+?) is already in a fellowship",
                ["TargetJoined"] = "(?<name>.+?) is now a member of your Fellowship",
                ["YouDisband"] = "^You have disbanded your Fellowship",
                ["YouQuit"] = "^You are no longer a member of",
                ["YouWereDismissed"] = "has dismissed you from the Fellowship",
                ["FellowWasDisbanded"] = "has disbanded your Fellowship",
                ["Recruited"] = "You have been recruited"
            };

            Match match;

            foreach (KeyValuePair<string, string> item in AutoFellowStrings)
            {
                match = new Regex(item.Value).Match(message);
                if (match.Success)
                {
                    switch (item.Key)
                    {
                        case "Recruited":
                            FellowStatus = FellowshipEventType.Create;
                            break;
                        case "YouDisband":
                            FellowStatus = FellowshipEventType.Disband;
                            break;
                        case "YouQuit":
                            FellowStatus = FellowshipEventType.Quit;
                            break;
                        case "YouWereDismissed":
                            FellowStatus = FellowshipEventType.Dismiss;
                            break;
                        case "FellowWasDisbanded":
                            FellowStatus = FellowshipEventType.Disband;
                            break;
                        case "CreatedFellow": // You have created the Fellowship
                            FellowStatus = FellowshipEventType.Create;
                            Core.Actions.FellowshipSetOpen(true);
                            break;
                        case "NewLeader": // You inherited leader
                            Core.Actions.FellowshipSetOpen(true);
                            break;
                        case "NotAccepting": // Target is not accepting fellowship requests
                            Core.Actions.InvokeChatParser(string.Format("/t {0}, You are not accepting fellowship requests!", match.Groups["name"].Value));
                            Remove(match.Groups["name"].Value);
                            break;
                        case "JoinedFellow": // Target joins the fellowship
                            Remove(match.Groups["name"].Value);
                            break;
                        case "ElseLeftFellow": // Someone leaves the fellowship
                            break;
                        case "TargetJoined":
                            Remove(match.Groups["name"].Value);
                            break;
                        case "RequestFellow": // Someone /tells you the fellowship password
                            if (Filter.MainView.AutoFellow.Checked)
                            {
                                string name = match.Groups["dupleName"].Value.Substring(0, match.Groups["dupleName"].Value.Length / 2);
                                if (!FellowStatus.Equals(FellowshipEventType.Create))
                                {
                                    Core.Actions.InvokeChatParser(string.Format("/t {0}, I'm not currently in a fellowship.", name));
                                }
                                else if (match.Groups["secret"].Value.Equals(Filter.MainView.Password.Text))
                                {
                                    bool exists = false;
                                    for (int i = 0; i < Recruits.Count; i++)
                                    {
                                        if (Recruits[i].Name.Equals(name))
                                        {
                                            exists = true;
                                            Core.Actions.InvokeChatParser(string.Format("/t {0}, You have already requested a fellowship invitation.", name));
                                        }
                                    }
                                    if (!exists)
                                    {
                                        Core.Actions.InvokeChatParser(string.Format("/t {0}, Please stand near me, I'm going to try and recruit you into the fellowship.", name));
                                        int targetGuid = int.Parse(match.Groups["guid"].Value);
                                        Recruit recruit = new Recruit(name, targetGuid);
                                        Recruits.Add(recruit);
                                    }
                                }
                            }
                            break;
                        case "DeclinedFellow": // Declines fellowship invite
                            Remove(match.Groups["name"].Value);
                            break;
                        case "AlreadyFellow": // Target is already in a fellowship
                            Core.Actions.InvokeChatParser(String.Format("/t {0}, You're already in a fellowship.", match.Groups["name"].Value));
                            Remove(match.Groups["name"].Value);
                            break;
                    }
                    break;
                }
            }
        }

        public void CheckRecruit()
        {
            if (Recruits.Count > 0 && DateTime.Now - LastAttempt > TimeSpan.FromMilliseconds(2000))
            {
                LastAttempt = DateTime.Now;
                for (int i = 0; i < Recruits.Count; i++)
                {
                    Recruits[i].Attempts += 1;
                    if (Recruits[i].Attempts >= 10)
                    {
                        Core.Actions.InvokeChatParser(string.Format("/t {0}, I wasn't able to recruit you into the fellowship, or you did not accept the invite.", Recruits[i].Name));
                        Remove(Recruits[i].Name);
                    }
                    else
                    {
                        Core.Actions.FellowshipRecruit(Recruits[i].Guid);
                    }
                }
            }
        }

        private void Remove(string name)
        {
            for (int i = 0; i < Recruits.Count; i++)
            {
                if (Recruits[i].Name.Equals(name))
                {
                    Recruits.RemoveAt(i);
                    break;
                }
            }
        }

        private class Recruit
        {
            public string Name { get; set; }
            public int Guid { get; set; }
            public int Attempts { get; set; }
            public Recruit(string name, int guid)
            {
                Name = name;
                Guid = guid;
                Attempts = 0;
            }
        }
    }
}
