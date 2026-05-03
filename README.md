# SC2SmoothBrainifier
What's up, Gamers(tm)!

So I hear you bought a Steam Controller 2026 and want to use it for non Steam games.

You can add games to Steam and launch them from there, but this does not always work.

You could use [SISR](https://github.com/Alia5/SISR), but I've also heard people complain that it "still relies on Steam".

I've heard you all want to be able to use your new 100 dollar pc controller as an old 50 dollar controller on your pc, without Steam running. Said another way, you want to use the thing that vastly exceeds the capabilities of an XBox controller, as if it's an XBox controller. 

Why? I don't know, but here we are. Personally I think this is a crazy silly situation. "Non steam compatibility" was an "issue" a decade ago with Steam Controller 2015. The community created solutions then (the older glosc, the slightly newer glossi, sc-controller as a 3rd party rebinder), the community is creating solutions now ([SISR](https://github.com/Alia5/SISR), and now this program). It's a solved problem.

But apparently those community created tools are just too dang complicated to use for people. Monkey see basic controls, monkey want plug and play. Even if that misses the point of having a highly advanced controller backed by a highly advanced customization software.

Well, here you go!

## How to use SC2SmoothBrainifier/Dependencies

Install [usbip-win2](https://github.com/vadimgrn/usbip-win2) (required for viiper to work), close steam, run this .exe, and enjoy!

## What exactly is this program?

SC2SmoothBrainifier's purpose is to convert the basic controls on the Steam Controller 2026 into a basic XBox 360 controller without the need for Steam to be running.

It does this by using hidAPI to find the Steam Controller 2026's puck, disabling lizard mode, then feeding the hid data from the Steam Controller 2026 into a virtual XBox Controller created by [VIIPER](https://github.com/Alia5/VIIPER).

If that's too complicated an explanation; complicated thing in, simple thing out.

## What inputs from Steam Controller 2026 does this program support?

It supports:

* Left and Right analog sticks
* Left and Right stick clicks
* ABXY
* DPAD
* Bumpers
* Triggers
* Start, Back, and Guide buttons

It DOES NOT support:

* either trackpad
* either trackpad click
* capsense ontop of the sticks
* either grip sense
* the back buttons
* gyro

It is simple:

* No GUI
* No rebinding
* No advanced features

## ... Oh. You wanted those advanced features?

Use Steam Input.

Or [SISR](https://github.com/Alia5/SISR), if you are playing non steam games.

Or if you insist on keeping the basic controls basic...

## Having Steam Running Alongside SC2SmoothBrainifier

This is technically possible, as multiple programs can read the same hid reports at the same time.

However, to prevent double inputs you must:

* Disable XBox controller support in Steam. This prevents Steam from using the virtual XBox Controller that VIIPER makes.
* Ensure the "basic controls" of the Steam Controller 2026 are blanked out in your configurations

From there, you can use SC2SmoothBrainifier to make the basic controls act as a XBox controller anywhere and then supplement that functionality with Steam Input using the advanced features such as the trackpads and gyro.

The catch is that if you configure things in Steam Input, those things cannot do anything to modify the basic controls.

It would honestly be simpler to just completely use Steam Input.

## Why [VIIPER](https://github.com/Alia5/VIIPER), not vigembus?

Vigembus is well known, but is no longer maintained.

[VIIPER](https://github.com/Alia5/VIIPER) is newer, is actively being maintained, and is what [SISR](https://github.com/Alia5/SISR) is built on. This gives a clear upgrade path for you.

Because you already need [usbip-win2](https://github.com/vadimgrn/usbip-win2) installed to run SC2SmoothBrainifier, no extra steps are needed to get SISR running.

## Why vibe coded?

For a simple task like this, that is all that is needed. Or rather, all that is deserved.

Gemini 3.1 Pro Preview is the model that was used. As of the time of the initial release, 40k tokens were used to create and debug the program.
