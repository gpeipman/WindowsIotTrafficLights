using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;
using WindowsIotTrafficLights;

namespace WindowsIotTrafficLightsService
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        private LightsManager _lightsManager;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            var updateClient = new WebScheduleUpdateClient();
            var switcher = GetGpioSwitcher();

            if(switcher == null)
            {
                Debug.WriteLine("Switcer is null, exiting");
                _deferral.Complete();
                return;
            }

            _lightsManager = new LightsManager(switcher, updateClient);
            _lightsManager.Start();
        }

        private LedSwitcher GetGpioSwitcher()
        {
            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                Debug.WriteLine("GPIO controller is missing!");
                return null;
            }

            var redPin = gpio.OpenPin(17);
            var yellowPin = gpio.OpenPin(18);
            var greenPin = gpio.OpenPin(27);

            redPin.Write(GpioPinValue.Low);
            redPin.SetDriveMode(GpioPinDriveMode.Output);

            yellowPin.Write(GpioPinValue.Low);
            yellowPin.SetDriveMode(GpioPinDriveMode.Output);

            greenPin.Write(GpioPinValue.Low);
            greenPin.SetDriveMode(GpioPinDriveMode.Output);

            return new LedSwitcher(redPin, yellowPin, greenPin);
        }
    }
}
