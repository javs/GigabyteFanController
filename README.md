# GigabyteFanController

> **Warning**
> This only will only work on select Gigabyte AMD motherboards. It doesn't check if your motherboard is supported. Only use if you are confident you have this controller on your AMD Gigabyte board, otherwise it may affect your BIOS.

Some recent Gigabyte motherboards have a custom, BIOS based, fan controller that overrides the 2nd set of fans (typically in the ITE8792 IC). This program accepts one option to disable/enable that controller so other program can be used to drive the fans normally.

The controller is typically automatically enabled on boot/resume.

Usage as Admin:
```GigabyteFanController false```

Based on code from LibreHardwareMonitor.
