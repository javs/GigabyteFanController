# GigabyteFanController

> **Warning**
> This only will only work on select Gigabyte AMD motherboards. It doesn't check if your motherboard is supported.<br>
> **Only use if you are confident you have this controller on your AMD Gigabyte board, otherwise it may affect your BIOS.**<br>
> Use at your own risk.

Some recent Gigabyte motherboards have a custom fan controller firmware loaded in the 2nd Embedded Controller (typically a ITE879x IC), which overrides regular ITE fan control registers. This program accepts one option to disable/enable that controller so other programs can be used to drive the fans.

The controller is typically automatically enabled on boot/resume.

Usage as Admin:
```GigabyteFanController false```

Uses low level memory access code from LibreHardwareMonitor.
