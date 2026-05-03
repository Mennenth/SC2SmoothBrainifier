# Steam Controller 2 Smooth Brain-ifier
Congratulations. You bought a $100 piece of highly advanced hardware, only to demand it function exactly like a $50 plastic brick without Steam running. You want to take a device that vastly exceeds the capabilities of a standard Xbox controller and lobotomize it.  Why? I don't know, but here we are. Personally, I think this situation is ridiculous.

Non-Steam compatibility was "solved" a decade ago with the original Steam Controller. The community built tools like GlosSI, and now they've built [SISR](https://github.com/Alia5/SISR). But apparently, following a basic setup guide is too much work for you. You complained that [SISR](https://github.com/Alia5/SISR) "still relies on Steam". I suppose it's a case of "monkey see basic controls, monkey want plug and play". You are willingly missing the point of having a highly advanced controller backed by highly advanced customization software.

Well, fine. Here is your pacifier.

## How to Use This Dumb Thing

1. Install [usbip-win2](https://github.com/vadimgrn/usbip-win2). This is required for [VIIPER](https://github.com/Alia5/VIIPER) to work.
2. Close Steam.
3. Run the .exe downloaded from releases.
4. Enjoy your expensive Xbox controller.

## What Exactly Is This Program?

SC2SmoothBrainifier does one thing: it converts the basic controls of your Steam Controller 2026 into a basic Xbox 360 controller, completely bypassing Steam.

It uses hidAPI to find the controller's puck and violently kills Lizard Mode. Then, it sits in your system tray and feeds the raw HID data from your Steam Controller 2026 into a virtual Xbox controller created by [VIIPER](https://github.com/Alia5/VIIPER).

If that's too much technical jargon for you: complicated thing in, simple thing out.

## What Actually Works?

Here is the basic stuff that is mapped for you:
* Left and Right analog sticks.
* Left and Right stick clicks.
* ABXY.
* DPAD.
* Bumpers.
* Triggers.
* Start, Back, and Guide buttons.

Here is what is explicitly ignored, because you asked for it:
* Either trackpad.
* Either trackpad click.
* Capsense on top of the sticks.
* Either grip sense.
* The back buttons.
* Gyro.

This program is simple: it has no GUI, no rebinding, and absolutely no advanced features. 

... Oh, you wanted those advanced features? Go use Steam Input. Or if you're playing non-Steam games, use [SISR](https://github.com/Alia5/SISR).

## Running Steam At The Same Time as [SISR](https://github.com/Alia5/SISR) (For Masochists)

Technically, multiple programs can read the same HID reports at once. If you insist on keeping the basic controls basic but want to run Steam alongside this app, you can.

To prevent your PC from registering double inputs, you must:
* Disable Xbox controller support in Steam. This prevents Steam from hooking into the virtual Xbox controller VIIPER just made.
* Ensure the "basic controls" of the Steam Controller 2026 are completely blanked out in your Steam configurations.

From there, SC2SmoothBrainifier handles the basic controls, and Steam Input can handle the advanced stuff like trackpads and gyro.

The catch? Any configuration changes you make in Steam Input cannot modify the basic controls in any way. Honestly, it would be much simpler to just use Steam Input for everything.

## Why [VIIPER](https://github.com/Alia5/VIIPER) and not Vigembus?

Because Vigembus is dead and no longer maintained.

[VIIPER](https://github.com/Alia5/VIIPER) is actively maintained, newer, and is the exact same backbone that [SISR](https://github.com/Alia5/SISR) is built on. By forcing you to install [usbip-win2](https://github.com/vadimgrn/usbip-win2) to run SC2SmoothBrainifier, you already have the prerequisites for [SISR](https://github.com/Alia5/SISR). Consider it a clear upgrade path for when you eventually realize what you've done.

## Why Vibe Coded?

For a task this simple, vibe coding is all that's needed. Or rather, it's all this dumb project deserved.

This utility was generated using the Gemini 3.1 Pro Preview model. As of the time of the initial release, 40k tokens were used to create and debug the program.
