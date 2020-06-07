using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
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

            Activated += OnActivated;
            innerFrame.PointerMoved += OnPointerMoved;
        }

        private void OnActivated(object sender, WindowActivatedEventArgs e)
        {
            UpdateSizes(Bounds.Height, Bounds.Width);

            SizeChanged += (sender, e) => UpdateSizes(e.Size.Height, e.Size.Width);
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var currentPoint = e.GetCurrentPoint(innerFrame);
            pointer.SetValue(Canvas.LeftProperty, currentPoint.Position.X - 70);
            pointer.SetValue(Canvas.TopProperty, currentPoint.Position.Y - 70);
            outerCircle.Width = 100 * (currentPoint.Properties.Pressure + 1);
            outerCircle.Height = 100 * (currentPoint.Properties.Pressure + 1);
            midiPort.sendCommand(new byte[] { 0b11100000, 0, (byte)(currentPoint.Position.X / innerFrame.ActualWidth * 127) });
            midiPort.sendCommand(new byte[] { 0b10110000, 0, (byte)(currentPoint.Position.Y / innerFrame.ActualHeight * 127) });
            midiPort.sendCommand(new byte[] { 0b10110000, 1, (byte)(currentPoint.Properties.Pressure * 127) });
        }

        private void UpdateSizes(double rootWidth, double rootHeight)
        {
            theCanvas.Width = rootWidth / 5 * 4;
            theCanvas.Height = rootHeight;

            outerFrame.Width = theCanvas.Width - 20;
            outerFrame.Height = theCanvas.Height - 20;

            innerFrame.Width = outerFrame.Width - 40;
            innerFrame.Height = outerFrame.Height - 40;
        }

        private void PulseCC0(object sender, RoutedEventArgs e)
        {
            midiPort.sendCommand(new byte[] { 0b10110000, 0, 127 / 2 });
        }

        private void PulseCC1(object sender, RoutedEventArgs e)
        {
            midiPort.sendCommand(new byte[] { 0b10110000, 1, 127 / 2 });
        }

    }
}
