# Microbit-Unity
Unity scripts to allow the BBC Microbit to be used as a Unity Controller

**Note that these scripts current only work in Windows, but can be altered to work in other operating systems.**


### Installation

Simply clone the repo and open it in Unity as a Unity Project (Written in Unity 5.3.6).

Inside the folder there will be a .hex file. Plug in your Microbit via USB and drag this hex file into the Microbit.

### Quick Start

Inside the project there will be two prefabs:

+ **P1Input**
+ **SerialToController**

1. Create two Unity objects using the two prefabs above.
2. Set the reference of the serial input in P1Input to the SerialToController script.
3. Change the COM port to the one used by the Microbit on your machine (So far on all the machines
I have tested, this is always COM3).
4. Use the public get functions in the P1Input's SerialToInputs script to get the current x y z
rotational values and the current status of the A and B buttons on the Microbit.


### Issues
---
> The type or namespace name `Ports' does not exist in the namespace `System.IO'. Are you missing an assembly reference?

Go to Edit > Project Settings > Player > Optimization > Api Compatibility Level

Set this to .Net 2.0 (not subset)

---

