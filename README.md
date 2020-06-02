# Overview

This project was designed from the ground up to be the new defacto multicharacter portal bot for Asheron's Call. The idea for the project was it was to be completed within Decal only, requiring no other plugins or programs to be running (except for the Virindi View System). This was designed to be very user friendly from setup to actual end-user use. If you know how to add a Decal plugin, you are already halfway to running your portal bot!

## Prerequisites
1. Asheron's Call/ThwargLauncher/Decal Installed and working properly.
2. Virindi Bundle is installed - the plugin **requires** VVS and does **not** provide Decal fallback.

## Installation

ACManager plugin is the frontend of the bot only. The backend is controlled via the Decal filter. To run the bot, you must have the filter. Once the bot is configured, you don't actually need the plugin to function, just the filter.

1. **(Recommended)** This can be automatically handled using the installer found [here](https://github.com/patri0t86/ACManager/releases). The uninstaller will then remove all files created throughout the lifecycle of the plugin/filter, giving you a clean uninstall.
2. You can manually install everything from the same location above, just using the .dlls like you would any other plugin.
    1. **Both** dlls are required for bot functionality!
    2. Make sure both dlls are contained in the same folder when running the bot.

## Usage

1. Use the ACManager plugin to easily edit the portal mapping to your characters.
   1. This can be accessed by checking the "Portal Bot" check box on the "Controls" tab. Pressing this check box does **not** start the bot.
2. Once your ties are set up for each character, type `/acm bot enable` and the bot will start.
3. To stop your bot, type `/acm bot disable` or completely close your client.

## Assumptions 

These may be handled in the future if asked for - dev time isn't infinite!

- Your characters know Summon Primary Portal 1.
- Your characters know Summon Secondary Portal 1.
- Your characters have wands equipped.
- Your characters have components.

The bot does handle mana management, but then it also assumes:

- Your characters know life magic.
- Your characters know Stamina to Mana [1-7].
- Your characters know Revitalize Self [1-7].

## Recommendations

These recommendations will allow the bot to restart automatically, even on server crashes and reboots.

- ThwargFilter added in Decal.
- Have your portal bot account set to login automatically (Auto Relaunch).
- Set your On-Login command for the default character to `/acm bot enable`.
- Set any other commands you want to customize your bot with, i.e. `/acm ads interval 6.67` if you want ads every 6.67 minutes.

## FAQs
- What other plugins do I need to have enabled for this to work?
  - You do not require any other plugins to be enabled. This plugin/filter combo is completely self-contained. The only pre-requisite is Decal and the Virindi Bundle being installed.
- Do I need to escape characters?
  - No need to worry about character escaping. You type anything you want into descriptions, keywords, or advertisements as you want them to be displayed.
- Do I need to fill all character slots?
  - No, you don't need a full account for this to work. You don't even need to have portals on every character for this to work. As long as you don't map portals in the GUI to the characters that don't have portals, the bot will handle it automatically for you.
- What if my bot doesn't know life magic?
  - The bot will just wait until it is >50% mana and then summon.

## Extras

1. You can have as many advertisements as you want, customized any way you want. It is highly recommended to control the advertisement creation and deletion through the GUI since it will handle it for you. However, if you want to edit the XML directly, you can. Any manual edits will not show up in the GUI until the next login, but the bot will read all XML edits live.
2. The frontend plugin initially started as a completely different plugin, with the portal bot added later. Any additional funtionality is hopefully fairly self-explanatory, but as a quick rundown:
   1. *Recruit Command* is a custom, case-sensitive password you can set for fellowship management. If you are in an open fellowship, or lead a fellowship, if someone whispers you that command, they will automatically receive a fellowship invite. This command can be anything you want to set.
   2. *Auto Respond* has some minor functionality built-in. Whisper "comps" to get a status on comps. Or whisper any in-game name and the bot will respond with how many of those items it currently possesses, i.e. "Prismatic Taper" or "Brood Matron Carapace".
   3. EXP Tracker should be totally self-explanatory.
   4. *Low Comp. Logoff* gives you the ability to log off a character automatically if it runs low on a component. You have the option to broadcast your disconnect to your fellowship.
