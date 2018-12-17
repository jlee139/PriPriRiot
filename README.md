# PriPriRiot
CS425 Game Final Project

The actual Final Project

## Game Play:

This is a stat-raising game set in a prison where the player must convince the other inmates within a week to incite a riot so the player can escape to go home for their child’s birthday. This is done by choosing the "right" dialogue option when talking to a prisoner. The lower the "difficulty" of a prisoner, the easier it is to convince them to join the riot.

## Technical Component: 

The main focus will be procedural content generation for creating different prisoners for the player to interact with. Every time the game is run, 15 new prisoners with randomized names, crimes, difficulty level, and random dialogues are created. When the player clicks on a location to talk to a prisoner, a random number of prisoners will be hanging out there. And when the player talks to a prisoner, a random dialogue is chosen for the player to try and convince the prisoner to join their cause.

## The Process:

I began with a [VN tutorial in Unity](http://www.indiana.edu/~gamedev/2015/09/27/creating-a-visual-novel-in-unity/) created by Cody Claborn. This was used to set up the basic story structure of the game. Afterwards, came the map decision. Each location that's clickable has a button attached to it. When you click it, it randomly picks 3-5 prisoners in that location for you to talk to. You pick 1 of them, and that picks a random dialogue from that prisoner's specific list. If you convince the prisoner, they are added to a list of converted prisoners, which is reflected on the daily report. When you hit the required number to start a riot, you set off the Success flag. Otherwise, you failed to achieve your goal and you return to your daily grind.
    
## Future Additions I Would Like:

* A gallery of prisoners to check in on their status and dialogues unlocked
* A way to meet everyone in a location
* Sprites and sfxs I wasn't able to include this time around
* Multiple endings
* Harada
    
## Music & Sound Credits:

* ["Underwater 01" by Sara Afonso](http://freemusicarchive.org/music/Sara_Afonso/Underwater_EP/Sara_Afonso_-_Underwater_01)
* ["Hand Bells, Singles" by InspectorJ](https://freesound.org/people/InspectorJ/sounds/339809/)
* ["Front Door" by InspectorJ](https://freesound.org/people/InspectorJ/sounds/431117/)
* ["Wrong" by TheBuilder15](https://freesound.org/people/TheBuilder15/sounds/415764/)
    
Presentation Powerpoint [here](https://docs.google.com/presentation/d/1WrbIO2vxqm-QHNBINMgCtlyt6a9y2EIdpYAFNyJSbDU/edit?usp=sharing)
    
Last Updated 12/16/2018
