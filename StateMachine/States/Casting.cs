using ACManager.Settings;
using Decal.Adapter;
using Decal.Adapter.Wrappers;
using Decal.Filters;
using System;
using System.Collections.Generic;

namespace ACManager.StateMachine.States
{
    /// <summary>
    /// This state controls the casting of all spells.
    /// </summary>
    public class Casting : StateBase<Casting>, IState
    {
        private DateTime Started { get; set; }
        private bool StartedBuffCycle { get; set; }
        private Spell LastSpell { get; set; }
        private bool LostTarget { get; set; }
        private DateTime StartedTracking { get; set; }
        private readonly List<Spell> HoldSpells = new List<Spell>();

        public void Enter(Machine machine)
        {
            machine.Fizzled = false;
            machine.CastCompleted = false;
            machine.CastStarted = false;
            LostTarget = false;

            if (machine.CurrentRequest.RequestType.Equals(RequestType.Buff) && !StartedBuffCycle)
            {
                Started = DateTime.Now;
                StartedBuffCycle = true;

                ChatManager.Broadcast(Inventory.ReportOnLowComponents());
                TimeSpan buffTime = TimeSpan.FromSeconds(machine.CurrentRequest.SpellsToCast.Count * 4);
                ChatManager.SendTell(machine.CurrentRequest.RequesterName, $"Casting {machine.CurrentRequest.SpellsToCast.Count} buffs on you. This should take about {(buffTime.Minutes > 0 ? $"{buffTime.Minutes} minutes and " : string.Empty)}{buffTime.Seconds} seconds.");
                if (!HaveSelfBuffs(machine))
                {
                    SetupSelfBuffing(machine);
                }
            }
        }

        public void Exit(Machine machine)
        {
            
        }

        public void Process(Machine machine)
        {
            if (!machine.Enabled)
            {
                StartedBuffCycle = false;
                machine.NextState = Equipping.GetInstance;
                return;
            }

            if (machine.CurrentRequest.SpellsToCast.Count.Equals(0))
            {
                if (!HoldSpells.Count.Equals(0))
                {
                    machine.CurrentRequest.SpellsToCast.AddRange(HoldSpells);
                    HoldSpells.Clear();
                    return;
                }

                if (machine.CurrentRequest.RequestType.Equals(RequestType.Buff))
                {
                    StartedBuffCycle = false;
                    TimeSpan duration = DateTime.Now - Started;
                    ChatManager.SendTell(machine.CurrentRequest.RequesterName, $"Your request is complete, it took {(duration.Minutes > 0 ? $"{duration.Minutes} minutes and ": string.Empty)}{duration.Seconds} seconds.");
                }

                if (machine.Requests.Count.Equals(0) || !machine.Requests.Peek().RequestType.Equals(RequestType.Buff) || !CoreManager.Current.CharacterFilter.Name.Equals(Utility.BotSettings.BuffingCharacter)) { 
                    machine.NextState = Equipping.GetInstance;
                    return;
                }

                machine.CurrentRequest = machine.Requests.Dequeue();
                Started = DateTime.Now;
                StartedBuffCycle = true;

                ChatManager.Broadcast(Inventory.ReportOnLowComponents());
                TimeSpan buffTime = TimeSpan.FromSeconds(machine.CurrentRequest.SpellsToCast.Count * 4);
                ChatManager.SendTell(machine.CurrentRequest.RequesterName, $"Casting {machine.CurrentRequest.SpellsToCast.Count} buffs on you. This should take about {(buffTime.Minutes > 0 ? $"{buffTime.Minutes} minutes and " : string.Empty)}{buffTime.Seconds} seconds.");

                if (!HaveSelfBuffs(machine))
                {
                    SetupSelfBuffing(machine);
                }
            }

            if (!CoreManager.Current.Actions.CombatMode.Equals(CombatState.Magic))
            {
                CoreManager.Current.Actions.SetCombatMode(CombatState.Magic);
                return;
            }
            
            if (machine.CastStarted && machine.CastCompleted && !machine.Fizzled)
            {
                if (machine.CurrentRequest.SpellsToCast[0].Id.Equals(157) || machine.CurrentRequest.SpellsToCast[0].Id.Equals(2648))
                {
                    machine.PortalsSummonedThisSession += 1;
                    ChatManager.Broadcast($"/s Portal now open to {(string.IsNullOrEmpty(machine.CurrentRequest.Destination) ? "Portal now open." : machine.CurrentRequest.Destination)}. Safe journey, friend.");
                }
                machine.CurrentRequest.SpellsToCast.RemoveAt(0);
                machine.CastCompleted = false;
                machine.CastStarted = false;
                return;
            }


            if (machine.CastStarted && machine.CastCompleted && machine.Fizzled)
            {
                machine.Fizzled = false;
                machine.CastCompleted = false;
                machine.CastStarted = false;
                LostTarget = false; // Put here in case of more than 2 fizzles on a buff cycle
                return;
            }

            if (machine.CastStarted)
            {
                return;
            }

            if (machine.CurrentRequest.IsCancelled)
            {
                machine.CurrentRequest.SpellsToCast.Clear();
                return;
            }

            if (CoreManager.Current.CharacterFilter.Mana < Utility.BotSettings.ManaThreshold * CoreManager.Current.CharacterFilter.EffectiveVital[CharFilterVitalType.Mana]
                && !CoreManager.Current.Actions.SkillTrainLevel[Decal.Adapter.Wrappers.SkillType.BaseLifeMagic].Equals(1))
            {
                machine.NextState = VitalManagement.GetInstance;
                return;
            }

            if (!(CoreManager.Current.CharacterFilter.IsSpellKnown(machine.CurrentRequest.SpellsToCast[0].Id) && machine.SpellSkillCheck(machine.CurrentRequest.SpellsToCast[0])))
            {
                Spell fallbackSpell = machine.GetFallbackSpell(machine.CurrentRequest.SpellsToCast[0]);
                machine.CurrentRequest.SpellsToCast.RemoveAt(0);
                if (fallbackSpell != null)
                {
                    machine.CurrentRequest.SpellsToCast.Insert(0, fallbackSpell);
                }
                return;
            }

            if (!Inventory.HaveComponents(machine.CurrentRequest.SpellsToCast[0]))
            {
                ChatManager.Broadcast($"I have run out of spell components.");
                machine.CurrentRequest.SpellsToCast.Clear();
                return;
            }

            if (LastSpell != null
                && LastSpell.Equals(machine.CurrentRequest.SpellsToCast[0])
                && !(machine.CurrentRequest.SpellsToCast[0].Id.Equals(157)
                || machine.CurrentRequest.SpellsToCast[0].Id.Equals(2648)))
            {
                if (!LostTarget)
                {
                    StartedTracking = DateTime.Now;
                    LostTarget = true;
                }
                else if ((DateTime.Now - StartedTracking).TotalSeconds > 5)
                {
                    machine.CurrentRequest.SpellsToCast.Clear();
                    return;
                }
            } 
            else
            {
                LastSpell = machine.CurrentRequest.SpellsToCast[0];
                LostTarget = false;
            }

            if (IsBane(machine.CurrentRequest.SpellsToCast[0]) && !IsBaneable(machine.CurrentRequest.RequesterGuid))
            {
                ChatManager.SendTell(machine.CurrentRequest.RequesterName, "You're not wearing a shield or are wearing a covenant shield, so I am skipping banes.");
                while(IsBane(machine.CurrentRequest.SpellsToCast[0]))
                {
                    machine.CurrentRequest.SpellsToCast.RemoveAt(0);
                }
            }

            CoreManager.Current.Actions.CastSpell(machine.CurrentRequest.SpellsToCast[0].Id, machine.CurrentRequest.RequesterGuid);
        }

        public override string ToString()
        {
            return nameof(Casting);
        }

        private bool IsBane(Spell spell)
        {
            return spell.Family.Equals(160)
                || spell.Family.Equals(162)
                || spell.Family.Equals(164)
                || spell.Family.Equals(166)
                || spell.Family.Equals(168)
                || spell.Family.Equals(170)
                || spell.Family.Equals(172)
                || spell.Family.Equals(174);
        }

        private bool ContainsBanes(List<Spell> spells)
        {
            foreach (Spell spell in spells)
            {
                if (IsBane(spell))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsBaneable(int targetGuid)
        {
            using (WorldObjectCollection items = CoreManager.Current.WorldFilter.GetByContainer(targetGuid))
            {
                items.SetFilter(new ByObjectClassFilter(ObjectClass.Armor));
                if (items.Count > 0)
                {
                    if (!items.First.Name.Contains("Covenant"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool HaveSelfBuffs(Machine machine)
        {
            try
            {
                BuffProfile profile = Utility.GetProfile("botbuffs");

                List<Spell> requiredBuffs = new List<Spell>();
                foreach (Buff buff in profile.Buffs)
                {
                    Spell spell = CoreManager.Current.Filter<FileService>().SpellTable.GetById(buff.Id);

                    if (Utility.BotSettings.Level7Self && spell.Difficulty > 300)
                    {
                        requiredBuffs.Add(machine.GetFallbackSpell(spell, true));
                    }
                    else
                    {
                        requiredBuffs.Add(spell);
                    }
                }

                Dictionary<int, int> enchantments = new Dictionary<int, int>();
                foreach (EnchantmentWrapper enchantment in CoreManager.Current.CharacterFilter.Enchantments)
                {
                    if (requiredBuffs.Contains(CoreManager.Current.Filter<FileService>().SpellTable.GetById(enchantment.SpellId)) && !enchantments.ContainsKey(enchantment.SpellId))
                    {
                        enchantments.Add(enchantment.SpellId, enchantment.TimeRemaining);
                    }
                }

                foreach (Spell requiredBuff in requiredBuffs)
                {
                    if (!enchantments.ContainsKey(requiredBuff.Id))
                    {
                        return false;
                    }

                    else if (enchantments[requiredBuff.Id] < 300 && !enchantments[requiredBuff.Id].Equals(-1))
                    { 
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return false;
            }
        }

        public void SetupSelfBuffing(Machine machine)
        {
            if (!string.IsNullOrEmpty(machine.CurrentRequest.RequesterName))
            {
                ChatManager.SendTell(machine.CurrentRequest.RequesterName, "I need to buff myself first.");
            }

            HoldSpells.AddRange(machine.CurrentRequest.SpellsToCast);
            machine.CurrentRequest.SpellsToCast.Clear();
            machine.CurrentRequest.SpellsToCast.AddRange(GetSelfBuffs(machine));
        }

        public static List<Spell> GetSelfBuffs(Machine machine)
        {
            List<Spell> spells = new List<Spell>();
            foreach (Buff buff in Utility.GetProfile("botbuffs").Buffs)
            {
                if (Utility.BotSettings.Level7Self && CoreManager.Current.Filter<FileService>().SpellTable.GetById(buff.Id).Difficulty > 300)
                {
                    spells.Add(machine.GetFallbackSpell(CoreManager.Current.Filter<FileService>().SpellTable.GetById(buff.Id), true));
                }
                else
                {
                    spells.Add(CoreManager.Current.Filter<FileService>().SpellTable.GetById(buff.Id));
                }
            }

            if (CoreManager.Current.CharacterFilter.EffectiveSkill[CharFilterSkillType.CreatureEnchantment] < 400 && !Utility.BotSettings.Level7Self)
            {
                spells.Insert(0, CoreManager.Current.Filter<FileService>().SpellTable.GetById(2215)); // creature 7
                spells.Insert(0, CoreManager.Current.Filter<FileService>().SpellTable.GetById(2067)); // focus 7
                spells.Insert(0, CoreManager.Current.Filter<FileService>().SpellTable.GetById(2091)); // self 7
            }
            return spells;
        }
    }
}
