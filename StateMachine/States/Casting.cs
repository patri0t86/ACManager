using ACManager.StateMachine.Queues;
using Decal.Adapter.Wrappers;
using System;
using System.Collections.Generic;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// This state controls the casting of all spells.
    /// </summary>
    internal class Casting : StateBase<Casting>, IState
    {
        private DateTime Started { get; set; }
        private bool SentInitialInfo { get; set; }
        private bool CastBanes { get; set; }
        private int LastSpell { get; set; }
        private bool StartedTracking { get; set; }
        private DateTime StartedTrackingTime { get; set; }
        private int SpellCastCount { get; set; }

        public void Enter(Machine machine)
        {
            machine.Fizzled = false;
            machine.CastCompleted = false;
            machine.CastStarted = false;

            if (machine.Inventory.LowComponents.Count > 0)
            {
                machine.ChatManager.Broadcast(machine.Inventory.LowCompsReport());
            }

            if (machine.CurrentRequest.RequestType.Equals(RequestType.Buff))
            {
                if (!SentInitialInfo)
                {
                    Started = DateTime.Now;
                    SentInitialInfo = !SentInitialInfo;
                    SpellCastCount = 0;
                    TimeSpan buffTime = TimeSpan.FromSeconds(machine.SpellsToCast.Count * 4);
                    if (machine.SpellsToCast.Contains(5989)
                        || machine.SpellsToCast.Contains(5997)
                        || machine.SpellsToCast.Contains(6006)
                        || machine.SpellsToCast.Contains(6014)
                        || machine.SpellsToCast.Contains(6023)
                        || machine.SpellsToCast.Contains(6031))
                    {
                        machine.ChatManager.SendTell(machine.CurrentRequest.RequesterName, $"Casting {machine.SpellsToCast.Count} buffs on you. This should take about {buffTime.Minutes} minutes and {buffTime.Seconds} seconds. Make sure you are standing near me as Item Enchantments are short range spells.");
                    }
                    else
                    {
                        machine.ChatManager.SendTell(machine.CurrentRequest.RequesterName, $"Casting {machine.SpellsToCast.Count} buffs on you. This should take about {buffTime.Minutes} minutes and {buffTime.Seconds} seconds.");
                    }

                    if (machine.CurrentRequest.RequesterGuid != 0 && !CastBanes)
                    {
                        if (ContainsBanes(machine.SpellsToCast))
                        {
                            using (WorldObjectCollection items = machine.Core.WorldFilter.GetByContainer(machine.CurrentRequest.RequesterGuid))
                            {
                                items.SetFilter(new ByObjectClassFilter(ObjectClass.Armor));
                                if (items.Count > 0)
                                {
                                    if (!items.First.Name.Contains("Covenant"))
                                    {
                                        CastBanes = true;
                                    }
                                    else
                                    {
                                        machine.ChatManager.SendTell(machine.CurrentRequest.RequesterName, "You are wearing a covenant shield, so I will be skipping banes.");
                                    }
                                }
                                else
                                {
                                    machine.ChatManager.SendTell(machine.CurrentRequest.RequesterName, "You are not wearing a shield, so I will be skipping banes.");
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Exit(Machine machine)
        {
            if (machine.CurrentRequest.RequestType.Equals(RequestType.Buff) && machine.SpellsToCast.Count.Equals(0))
            {
                SentInitialInfo = !SentInitialInfo;
                TimeSpan duration = DateTime.Now - Started;
                machine.ChatManager.SendTell(machine.CurrentRequest.RequesterName, $"Buffing is complete with {SpellCastCount} buffs, it took {duration.Minutes} minutes and {duration.Seconds} seconds.");
                CastBanes = false;
            }
        }

        public void Process(Machine machine)
        {
            if (machine.Enabled)
            {
                if (machine.SpellsToCast.Count > 0)
                {
                    if (machine.CastStarted && machine.CastCompleted && !machine.Fizzled)
                    {
                        if (machine.SpellsToCast[0].Equals(157) || machine.SpellsToCast[0].Equals(2648))
                        {
                            machine.PortalsSummonedThisSession += 1;
                            if (!string.IsNullOrEmpty(machine.PortalDescription))
                            {
                                machine.ChatManager.Broadcast($"/s Portal now open to {machine.PortalDescription}. Safe journey, friend.");
                            }
                            else
                            {
                                machine.ChatManager.Broadcast($"/s Portal now open. Safe journey, friend.");
                            }
                        }
                        SpellCastCount += 1;
                        machine.SpellsToCast.RemoveAt(0);
                        machine.CastCompleted = false;
                        machine.CastStarted = false;
                    }
                    else if (machine.CastStarted && machine.CastCompleted && machine.Fizzled)
                    {
                        machine.Fizzled = false;
                        machine.CastCompleted = false;
                        machine.CastStarted = false;
                    }
                    else if (!machine.CastStarted)
                    {
                        if (machine.Core.CharacterFilter.Mana < machine.ManaThreshold * machine.Core.CharacterFilter.EffectiveVital[CharFilterVitalType.Mana] && machine.Core.Actions.SkillTrainLevel[SkillType.BaseLifeMagic] != 1)
                        {
                            machine.NextState = VitalManagement.GetInstance;
                        }
                        else
                        {
                            if (machine.Core.Actions.CombatMode != CombatState.Magic)
                            {
                                machine.Core.Actions.SetCombatMode(CombatState.Magic);
                            }
                            else if (machine.Core.CharacterFilter.IsSpellKnown(machine.SpellsToCast[0]))
                            {
                                if (machine.ComponentChecker.HaveComponents(machine.SpellsToCast[0]))
                                {
                                    if (LastSpell.Equals(machine.SpellsToCast[0]) && !machine.CastStarted)
                                    {
                                        if (!StartedTracking)
                                        {
                                            StartedTrackingTime = DateTime.Now;
                                            StartedTracking = !StartedTracking;
                                        }
                                        else if ((DateTime.Now - StartedTrackingTime).TotalSeconds > 10)
                                        {
                                            StartedTracking = !StartedTracking;
                                            machine.SpellsToCast.Clear();
                                        }
                                    }
                                    else if (StartedTracking)
                                    {
                                        StartedTracking = !StartedTracking;
                                    }

                                    LastSpell = machine.SpellsToCast[0];

                                    if (SpellIsBane(machine.SpellsToCast[0]))
                                    {
                                        if (CastBanes)
                                        {
                                            machine.Core.Actions.CastSpell(machine.SpellsToCast[0], machine.CurrentRequest.RequesterGuid);
                                        }
                                        else
                                        {
                                            machine.SpellsToCast.RemoveAt(0);
                                        }
                                    }
                                    else
                                    {
                                        machine.Core.Actions.CastSpell(machine.SpellsToCast[0], machine.CurrentRequest.RequesterGuid);
                                    }
                                }
                                else
                                {
                                    int fallbackSpell = FallbackSpellCheck(machine.SpellsToCast[0]);
                                    machine.SpellsToCast.RemoveAt(0);
                                    if (fallbackSpell != 0)
                                    {
                                        machine.SpellsToCast.Insert(0, fallbackSpell);
                                    }
                                    else
                                    {
                                        machine.ChatManager.Broadcast($"I have run out of spell components.");
                                        machine.SpellsToCast.Clear();
                                        machine.Requests.Clear();
                                    }
                                }
                            }
                            else
                            {
                                int fallbackSpell = FallbackSpellCheck(machine.SpellsToCast[0]);
                                machine.SpellsToCast.RemoveAt(0);
                                if (fallbackSpell != 0)
                                {
                                    machine.SpellsToCast.Insert(0, fallbackSpell);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (machine.Core.Actions.CombatMode != CombatState.Peace)
                    {
                        machine.Core.Actions.SetCombatMode(CombatState.Peace);
                    }
                    else if (machine.Core.Actions.CombatMode == CombatState.Peace)
                    {
                        machine.NextState = Equipping.GetInstance;
                    }
                }
            }
            else
            {
                SentInitialInfo = !SentInitialInfo;
                machine.NextState = Equipping.GetInstance;
            }
        }

        public override string ToString()
        {
            return nameof(Casting);
        }

        private bool SpellIsBane(int spellId)
        {
            return spellId.Equals(4391)
                || spellId.Equals(4393)
                || spellId.Equals(4397)
                || spellId.Equals(4401)
                || spellId.Equals(4403)
                || spellId.Equals(4407)
                || spellId.Equals(4409)
                || spellId.Equals(4412);
        }

        private bool ContainsBanes(List<int> spellList)
        {
            List<int> banes = new List<int>()
            {
                4407,
                4412,
                4393,
                4397,
                4401,
                4403,
                4409,
                4391
            };

            foreach (int bane in banes)
            {
                if (spellList.Contains(bane))
                {
                    return true;
                }
            }
            return false;
        }

        private int FallbackSpellCheck(int spellId)
        {
            List<int> spellList = new List<int>()
            {
                4407,
                4412,
                4393,
                4397,
                4401,
                4403,
                4409,
                4391,
                5989,
                5998,
                6006,
                6014,
                6023,
                6031
            };

            if (spellList.Contains(spellId))
            {
                if (spellId.Equals(4407))
                {
                    return 2108;
                }
                else if (spellId.Equals(4412))
                {
                    return 2113;
                }
                else if (spellId.Equals(4393))
                {
                    return 2094;
                }
                else if (spellId.Equals(4397))
                {
                    return 2098;
                }
                else if (spellId.Equals(4401))
                {
                    return 2102;
                }
                else if (spellId.Equals(4403))
                {
                    return 2014;
                }
                else if (spellId.Equals(4409))
                {
                    return 2210;
                }
                else if (spellId.Equals(4391))
                {
                    return 2092;
                }
                else if (spellId.Equals(5989))
                {
                    return 5988;
                }
                else if (spellId.Equals(5998))
                {
                    return 5996;
                }
                else if (spellId.Equals(6006))
                {
                    return 6005;
                }
                else if (spellId.Equals(6014))
                {
                    return 6013;
                }
                else if (spellId.Equals(6023))
                {
                    return 6021;
                }
                else if (spellId.Equals(6031))
                {
                    return 6030;
                }
            }
            return 0;
        }
    }
}
