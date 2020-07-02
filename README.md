# Overview

This project was designed from the ground up to be the new defacto multicharacter bot for Asheron's Call. The idea for the project was it was to be completed within Decal only, requiring no other plugins (except for the Virindi View System) or programs. If you know how to add a plugin to Decal, you are already halfway to running your bot!

## What It Does
This provides multicharacter portal bot functionality to an account on Asheron's Call. You set a portal keyword (only requirement), and a description, level, and a heading (those 3 are optional). If the bot is turned on, the bot will do the rest!

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
3. To start/stop the bot, simply toggle the *Start/Stop* checkbox in the *Config* tab of the *Bot Manager*.

## Assumptions

The bot does handle mana management, but then it also assumes:

- Your characters know life magic.
- Your characters know Stamina to Mana [1-7].
- Your characters know Revitalize Self [1-7].

## FAQs

- What other plugins do I need to have enabled for this to work?
  - You do not require any other plugins to be enabled/turned on at time of running the bot. This plugin is completely self-contained. The only pre-requisite is Decal and the Virindi View System.
- Do I need to escape characters?
  - No need to worry about character escaping. You type anything you want into descriptions, keywords, or advertisements as you want them to be displayed.
- Do I need to fill all character slots?
  - No, you don't need a full account for this to work. You don't even need to have portals on every character for this to work. As long as you don't map portals in the GUI to the characters that don't have portals, the bot will handle it automatically for you.
- What if my bot doesn't know life magic?
  - The bot will just wait until it is >50% mana and then summon.
