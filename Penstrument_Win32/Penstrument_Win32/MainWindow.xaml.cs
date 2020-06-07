using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TobiasErichsen.teVirtualMIDI;

namespace Penstrument_Win32
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        TeVirtualMIDI midiPort = new TeVirtualMIDI("Penstrument MIDI Port");

        public MainWindow()
        {
            this.InitializeComponent();

            tabletCanvas.PointerMoved += (sender, e) =>
            {
                var currentPoint = e.GetCurrentPoint(tabletCanvas);
                pointer.SetValue(Canvas.LeftProperty, currentPoint.Position.X - 50);
                pointer.SetValue(Canvas.TopProperty, currentPoint.Position.Y - 50);
                outerCircle.Width = 100 * (currentPoint.Properties.Pressure + 1);
                outerCircle.Height = 100 * (currentPoint.Properties.Pressure + 1);
                midiPort.sendCommand(new byte[] { 0b11100000, 0, (byte)(currentPoint.Position.X / tabletCanvas.ActualWidth * 127) });
                midiPort.sendCommand(new byte[] { 0b10110000, 0, (byte)(currentPoint.Position.Y / tabletCanvas.ActualHeight * 127) });
            };
        }

        private void PulseCC0(object sender, RoutedEventArgs e)
        {
            midiPort.sendCommand(new byte[] { 0b10110000, 0, 127 / 2 });
        }

    }
}
