using System;
using System.Diagnostics;
using Windows.Devices.Gpio;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WindowsIotTrafficLights;

namespace WindowsIotTrafficLightsApp
{
    public sealed partial class MainPage : Page
    {
        private LightsManager _lightsManager;

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var updateClient = new WebScheduleUpdateClient();
            var ledSwitcher = GetGpioSwitcher();
            var formSwitcher = new FormSwitcher(RedControl, YellowControl, GreenControl, Dispatcher);

            if(ledSwitcher == null)
            {
                Debug.WriteLine("Switcer is null");
            }

            var switchers = new ISwitcher[] { ledSwitcher, formSwitcher };

            _lightsManager = new LightsManager(switchers, updateClient);
            _lightsManager.ScheduleChanged += async x => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    TitleControl.Text = "Traffic lights: " + x;
                }).AsTask();
            };
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
