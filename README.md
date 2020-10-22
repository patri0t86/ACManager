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

1. **(Recommended)** This can be automatically handled using the installer found [here](https://github.com/patri0t86/ACManager/releases). The uninstaller will then remove all files created throughout the lifecycle of the plugin, giving you a clean uninstall.
2. You can manually install from the same location above, just using the .dll like you would any other plugin.

## Usage

1. Use the GUI to easily edit the portal mapping to your characters.
   1. This can be accessed by entering the *Portals* tab in the *Bot Manager* view.
2. Configure any other settings you wish in the *Config* tab of the *Bot Manager*.
3. To setup a buff bot, simply select the character for "Buffing Character" in the dropdown list found in the *Config* tab.
4. To start/stop the bot, simply toggle the *Start/Stop* checkbox in the *Config* tab of the *Bot Manager*.
5. If you wish to integrate with metas for recomping itself or any other functionality, you can use `/acm start` and `/acm stop` command line arguments from within a meta to control the bot.

### Buff Bot Info

1. The bot comes with a full complement of buff profiles out of the box. If you wish to add, or amend a profile, follow the format under the `BuffProfiles` folder where you installed ACManager. If there is a big enough ask, I may add in the ability to edit buff profiles in the GUI, or a standalone application since Decal views are very limited and clunky.
2. For level 8 spells that can be cast on others (per end of retail, this is only item spells; i.e. banes and weapon enchants), there is level 7 spell fallback built in if the spell is not known.

*Shoutout to Darlene of Reefcull for saving me the time and building the baseline buff profiles!*
