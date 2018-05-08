# WindowsIotTrafficLights
Traffic lights simulator built on Windows 10 IoT Core and Raspberry Pi. It supports loading of LED blinking 
schedules from remote sources and switches schedules automatically. Schedule switch is controlled by timer thread 
that polls remote machine for new schedule. Sample contains two schedules: normal traffic lights working cycle and 
blinking yellow.

## Electronic components
This is all you need to get things done on hardware size:

* Raspberry Pi
* Cheap breadboard
* Red, yellow and green LED
* Three resistors (I have 270Ω)
* Jumper wires for cabling

If you are not electronics guy then I suggest you to ask correct resistors for LED-s you are buying from electronics shop.

## Wiring
Wiring is simple using breadboard. Just connect everything you have like shown on the following diagram.

![Traffic lights wiring](raspberry-pi-traffic-lights.png)

## Architecture of solution
The solution is made of three projects:

* WindowsIotTrafficLights - interfaces and their default implementations.
* WindowsIotTrafficLightsApp - UWP traffic lights app for Raspberry, has UI with traffic lights.
* WindowsIotTrafficLightsService - traffic lights implemented as Windows 10 IoT Core background task.

The core of traffic lights is shown on the following diagram.

![Traffic lights architecture](raspberry-pi-traffic-lights-diagram.png)

Interfaces:

* IScheduleUpdateClient - used for LED-s schedule updates.
* ISwitcher - used to show traffic lights (be it LED-s or something else).

Classes:

* WebScheduleUpdateClient - schedule updater that requests data from web address.
* LightsManager - turns traffic lights on and off based on schedule.
* Schedule - DTO class for schedule.
* ScheduleItem - DTO class for schedule item.
* LedSwitcher - ISwitcher implementation for LED-s connected to Raspberry Pi.
* FromSwitcher - ISwitcher to bling traffic lights shapes on XAML form.
* EventBasedSwitched - Common switcher that fires ItemSwitched event when new ScheduleItem is set by manager.

## Traffic lights schedule
Part of external files folder of solution is simple PHP script that selects randomly between two JSON files:

* yellow.json - blinks yellow light (traffic lights are turned off)
* traffic.json - simple traffic lights cycle

By default the script is called from my web site. If you want to change the logic of schedule update then you 
can copy the script and json-files to your own web server or build your own end-point (just give the address of 
end-point to constructor of WebScheduleUpdateClient).