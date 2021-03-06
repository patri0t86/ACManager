# Overview

This project was designed from the ground up to be the new defacto multicharacter bot for Asheron's Call. The idea for the project was it was to be completed within Decal only, requiring no other plugins (except for the Virindi View System) or programs. If you know how to add a plugin to Decal, you are already halfway to running your bot!

## What It Does

This provides multicharacter bot functionality to an account on Asheron's Call. You set a portal keyword (only requirement), and a description, level, and a heading (those 3 are optional). If the bot is turned on, the bot will do the rest!

Now it also supports being a buff bot as well. Just set a character as your buff bot character from the dropdown list and the bot will handle the rest automagically :) When someone whispers you a buffing command, the bot will switch to the buffing character and do its thing.

## Prerequisites

1. Asheron's Call/ThwargLauncher/Decal Installed and working properly.
2. Virindi Bundle is installed - the plugin **requires** the Virindi View System (VVS) and does **not** provide Decal fallback.

## Installation

You have two choices: you can install automatically using the installer or manually just adding the DLLs to Decal.

1. **(Recommended)** This can be automatically handled using the installer found [here](https://github.com/patri0t86/ACManager/releases).
2. You can manually install from the same location above, just using the .dll like you would any other plugin.

## Usage

### General

1. To start/stop the bot, simply toggle the *Start/Stop* checkbox in the *Config* tab of the *Bot Manager*.
2. If you wish to integrate with metas for recomping itself or any other functionality, you can use `/acm start` and `/acm stop` command line arguments from within a meta to control the bot.

### Portal Bot

1. Use the GUI to easily edit the portal mapping to your characters.
   1. This can be accessed by entering the *Portals* tab in the *Bot Manager* view.
2. Configure any other settings you wish in the *Config* tab of the *Bot Manager*.

### Buff Bot

1. To setup a buff bot, simply select the character for "Buffing Character" in the dropdown list found in the *Config* tab.
2. Adjust stamina/mana percentages and decide if you want to self-buff with only lvl 7s (even if level 8s are known), as well as keep buffs alive or never let them expire.
3. A reasonable default of 80% success chance on cast was chosen to determine what level buffs you cast. This can be overridden by adding a positive integer into the Skill Override box.

### Buff Bot Info

1. The bot comes with a full complement of buff profiles out of the box. If you wish to add, or amend a profile, follow the format under the `BuffProfiles` folder where you installed ACManager. If there is a big enough ask, I may add in the ability to edit buff profiles in the GUI, or a standalone application since Decal views are very limited and clunky.
2. Full spell fallback is implemented from level 8 spells down to level 1. If a spell is unknown, or you don't have components, it goes down until a spell can be cast, or is unknown and then continues on with the buff cycle.
3. **WARNING** Due to the server protocol differences between ACE and GDLE, on release, only ACE servers are supported for the buff bot. This affects the buff bot functionality *only* and nothing else.
