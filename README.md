# Overview

This project was designed from the ground up to be the new defacto multicharacter portal bot for Asheron's Call. The idea for the project was it was to be completed within Decal only, requiring no other plugins or programs. This was designed to be very user friendly from setup to actual end-user use. If you know how to add a Decal plugin, you are already halfway to running your portal bot!

This bot's processing speed is tied to your client's frame rate. The faster the frame rate, the faster the processing. This was tested on a very performant gaming computer and, with little surprise, performed flawlessly. But the bot was also tested on low spec server hardware (no GPU), in a VM, and performed just fine, though a few seconds slower. So, unless someone finds otherwise, this will work on virtually any system spec that can play AC. 

NOTE: If you completely tax your system resources (max CPU/RAM/etc.) your system may run garbage collection to save itself and kill parts of the bot. This was experienced on my low end server when I had 4 instances of AC running at the same time on 3GB of memory. Once I lowered the resource usage, it ran without fail.

## Installation

1. ACManager plugin is the frontend of the bot only. The backend is controlled via the Decal filter. To run the bot, you must have the filter.
   1. **(Recommended)** This can be automatically handled using the installer found [here](https://github.com/patri0t86/ACManager/releases). The uninstaller will then remove all files created throughout the lifecycle of the plugin/filter, giving you a clean uninstall.
   2. You can manually install everything from the same location above, just using the .dll like you would any other plugin.

## Usage

1. Use the ACManager plugin to easily edit the portal mapping to your characters.
   1. This can be accessed by checking the "Portal Bot" check box on the "Controls" tab. Pressing this check box does **not** start the bot.
2. Once your ties are set up for each character, type `/acm start` and the bot will start.
3. To stop your bot, type `/acm stop` or completely close your client.

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

- ThwargFilter installed in Decal.
- Have your portal bot account set to login automatically (Auto Relaunch).
- Set your On-Login command for the default character to `acm start`.

## FAQs

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

### For What It's Worth

The original plugin, that the bot piggybacked on, was intended for general gameplay/personal use. The plan is to refactor it completely into cleaner, more maintainable code. It's ugly in its current form!

My plan is for this project to grow and evolve to help the entire AC community. If you have any issues, or feature requests, please record them here so I can properly track and respond. As the project name implies, my intention was for this bot to be your best friend, and manager of AC :)