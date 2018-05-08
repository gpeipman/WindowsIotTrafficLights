using System;
using System.Diagnostics;
using Windows.Devices.Gpio;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
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
            //var formSwitcher = new FormSwitcher(RedControl, YellowControl, GreenControl, Dispatcher);
            var eventSwicther = new EventBasedSwitcher();
            eventSwicther.ItemSwitched += EventSwicther_ItemSwitched;

            var switchers = new ISwitcher[] { ledSwitcher, eventSwicther };

            _lightsManager = new LightsManager(switchers, updateClient);
            _lightsManager.ScheduleChanged += async x => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    TitleControl.Text = "Traffic lights: " + x;
                }).AsTask();
            };
            _lightsManager.Start();
        }

        private async void EventSwicther_ItemSwitched(ScheduleItem item)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                RedControl.Fill = new SolidColorBrush(item.Red ? Colors.Red : Colors.Black);
                YellowControl.Fill = new SolidColorBrush(item.Yellow ? Colors.Yellow : Colors.Black);
                GreenControl.Fill = new SolidColorBrush(item.Green ? Colors.Green : Colors.Black);
            }).AsTask();
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
