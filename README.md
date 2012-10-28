# What is it

Power Plan Switcher is a simple Windows tray app which only task is to 
switch to "Maximal performance" plan on AC connect, and to "Power saver"
on disconnect.

It also shows current power plan, allows manual switching between the default 
schemes and displays power cable state and battery percentage. 

And it is as minimalistic as possible.


# Reasons

Well, default Windows behavior is to use two set of settings for each power 
scheme: AC on and AC off. As for me, it's easier to switch between those 
schemes.


# Code details

Written in C# (3.5, I guess). Well, I'd be more comfortable using something 
JVM-based, but, honestly, Java on Windows desktop still sucks.

