# Steam Controller 2 Smooth Brain-ifier

You bought a $100 piece of bleeding-edge hardware. It has capacitive touch, high-fidelity haptics, trackpads, and a gyroscope. It is a masterpiece of modern engineering.

And yet, to play a game on Windows in 2026, you are forced to make it masquerade as a $50 Xbox 360 controller from 2005.

Why? Because Microsoft's driver ecosystem is a rusted-out dumpster fire. Windows is a trillion-dollar operating system that still treats any peripheral more complex than two sticks and a D-pad like it’s absolute witchcraft.

Companies like Valve have to rely on software layers (Steam Input) because the native Windows API (XInput) is physically incapable of comprehending the innovation happening in hardware.

It isn't your fault that you just want to play a game on Epic or GOG without launching Steam.

Since Windows has a smooth brain, we have to smooth the brain of the controller so the OS can understand it. That's what this tool does.

## How to Dumb it Down

1: Install [usbip-win2](https://github.com/vadimgrn/usbip-win2). This is the open-source backbone [VIIPER](https://github.com/Alia5/VIIPER) (the backbone of this app) needs to trick Windows.

2: Close Steam.

3: Run the .exe downloaded from releases.

4: Play your games.

Note: this has only been tested through the Steam Controller 2's Puck, and only with 1 of them connected at a time. Connect more or differently at your own peril.

## What Exactly Is This Program?

SC2SmoothBrainifier lobotomizes the Steam Controller 2026 just enough to make Windows happy, without needing Steam running in the background.

It uses hidAPI to intercept the raw data from the controller's puck, disables Valve's "Lizard Mode," and sits in your system tray feeding the data into a virtual Xbox controller created by [VIIPER](https://github.com/Alia5/VIIPER).

Translating the future into 2005's standards. Complicated thing in, simple thing out.

## What Survived the Lobotomy?

Here is what XInput can actually understand, which this app supports:
* Left and Right analog sticks, and their clicks
* ABXY
* DPAD
* Bumpers
* Triggers
* Start, Back, and Guide buttons

Here are the casualties of Microsoft's neglect (Unsupported):
* Either trackpad
* Either trackpad click
* Capsense on top of the sticks
* Either grip sense
* The back buttons
* Gyro
* Rumble (the app can be sent rumble events, however it currently cannot send them to the controller)

There is no GUI. There is no rebinding. If you want those advanced features back, you have to go back to Steam, or use [SISR](https://github.com/Alia5/SISR) for non Steam games.

## Running Steam At The Same Time?

Unfortunately, this does not work. Because this utility is manually disabling lizard mode, with Steam running and also trying to disable lizard mode conflicts happen.

In hindsight, if I had been smart and used SDL instead of the full manual mode here, it might be possible? However, as this app is at a place where it currently serves its function; close steam, open app, play games as if you are using a xbox controller. As a result, I'm not spending more time on it.

## Why [VIIPER](https://github.com/Alia5/VIIPER) and not Vigembus?

Because Vigembus is dead, much like Microsoft's innovation in controller APIs.

[VIIPER](https://github.com/Alia5/VIIPER) is actively maintained, operates entirely in user-space, and is the exact same backbone that [SISR](https://github.com/Alia5/SISR) is built on. By installing [usbip-win2](https://github.com/vadimgrn/usbip-win2) to run this app, you've already met the prerequisites for [SISR](https://github.com/Alia5/SISR). Consider it an easy upgrade path when you're ready to unlock the rest of your controller but still intend to use it for non Steam games.

## Why Vibe Coded?

Because that's really as much as this simple app deserves.
