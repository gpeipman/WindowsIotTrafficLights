using System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WindowsIotTrafficLights;

namespace WindowsIotTrafficLightsApp
{
    internal class FormSwitcher : ISwitcher
    {
        private readonly Shape _redLight;
        private readonly Shape _yellowLight;
        private readonly Shape _greenLight;
        private readonly CoreDispatcher _dispatcher;

        public FormSwitcher(Shape redLight, Shape yellowLight, Shape greenLight, CoreDispatcher dispatcher)
        {
            _redLight = redLight;
            _yellowLight = yellowLight;
            _greenLight = greenLight;
            _dispatcher = dispatcher;
        }

        public async void SwitchToItem(ScheduleItem item)
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                _redLight.Fill = new SolidColorBrush(item.Red ? Colors.Red : Colors.Black);
                _yellowLight.Fill = new SolidColorBrush(item.Yellow ? Colors.Yellow : Colors.Black);
                _greenLight.Fill = new SolidColorBrush(item.Green ? Colors.Green : Colors.Black);
            }).AsTask();
        }
    }
}
