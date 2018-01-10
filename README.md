# Octogon R.E.D. - Rapid Explosive Desintegrator

This addon for Kerbal Space Program 1.3 and later allows a mission abort in style. Destroy your creation in an epic fireball when you lose control. 

## How it works
You will find a new part in the *Command and Control* tab. Add this part to your rocket and tweak it's settings via the right-click menu.

If you activate **Abort on AoA limit**, your mission will automatically abort once your angle of attack exceeds the given limit 

If you use **Abort on no control**, the abort sequence (Action Group) will be triggered when the vessel has no control. This will of course be triggered if you attach this part to a stage that is decoupled without a probe core. So you can use this to get rid of burnt out stages. 

If the part action is triggered - you can of course reassign it to any action group other than abort if you like - an explosion will be triggered, destroying the vessel. In the game logic the parts will be extremely overheated. If there is some crew to be saved, your abort action group should include a decoupler and parachutes, so they will not be part of the vessel that is destroyed!

## Prerequisites
The part is placed in the *Flight Control* node within the tech tree. Furthermore although the part will work with a Level I Vehicle Assembly Building, you need to upgrade it to Level II if you want to change the Action Group - for example to decouple the capsule with the pilot. 

## Words of warning
This part can destroy a vessel and it can kill your kerbals! Use at your own risk. 
You can use the part with space planes, but I'd advise against it.

This is my first KSP mod - feel free to give me feedback if you like.

## Other Mods
If you have [TweakScale](https://github.com/pellinor0/TweakScale) installed, the part can be scaled to your liking. 