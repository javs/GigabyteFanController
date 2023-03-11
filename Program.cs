using GigabyteFanController;

bool enabled;

try
{
    enabled = Convert.ToBoolean(args[0]);
} catch {
    Console.WriteLine("Usage {0} <enabled: true or false>", System.AppDomain.CurrentDomain.FriendlyName);
    return 1;
}

GigabyteController controller = new GigabyteController(GigabyteFanController.LibreHardwareMonitor.Model.B550_AORUS_PRO);

if (!controller.Enable(enabled))
{
    Console.WriteLine("Failed to set controller");
    return 1;
} else {
    Console.WriteLine("Controller set to {0}", enabled);
}

return 0;
