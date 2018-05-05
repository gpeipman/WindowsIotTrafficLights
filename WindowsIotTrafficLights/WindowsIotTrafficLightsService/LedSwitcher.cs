using Windows.Devices.Gpio;
using WindowsIotTrafficLights;

namespace WindowsIotTrafficLightsService
{
    internal class LedSwitcher : ISwitcher
    {
        private GpioPin _redPin;
        private GpioPin _yellowPin;
        private GpioPin _greenPin;

        public LedSwitcher(GpioPin redPin, GpioPin yellowPin, GpioPin greenPin)
        {
            _redPin = redPin;
            _yellowPin = yellowPin;
            _greenPin = greenPin;
        }

        public void SwitchToItem(ScheduleItem item)
        {
            _redPin.Write(item.Red ? GpioPinValue.High : GpioPinValue.Low);
            _yellowPin.Write(item.Yellow ? GpioPinValue.High : GpioPinValue.Low);
            _greenPin.Write(item.Green ? GpioPinValue.High : GpioPinValue.Low);
        }
    }
}
