using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections.Generic;
using TobiasErichsen.teVirtualMIDI;
using WinRT;

namespace Penstrument_Win32
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public static TeVirtualMIDI MIDIPort { get; } = new TeVirtualMIDI("Penstrument MIDI Port");

        List<AxisData> AxisDatas { get; } = new List<AxisData>()
        {
            new AxisData("X")
            {
                ValueSupplier = point => point.Position.X,
                MaxSupplier = v => v.ActualWidth
            },
            new AxisData("Y")
            {
                ValueSupplier = point => point.Position.Y,
                MaxSupplier = v => v.ActualHeight
            },
            new AxisData("Pressure")
            {
                ValueSupplier = point => point.Properties.Pressure,
                MaxSupplier = v => 1.0
            }
        };

        List<BoundValue> BoundValues { get; } = new List<BoundValue>()
        {
            new MIDIBoundValue("None", false),
            new MIDIBoundValue("Pitch Bend", false)
            {
                Builder = newValue => new byte[]{ 0b11100000, 0, newValue }
            },
            new MIDIBoundValue("CC 0", true)
            {
                Builder = newValue => new byte[]{ 0b10110000, 0, newValue }
            },
            new MIDIBoundValue("CC 1", true)
            {
                Builder = newValue => new byte[]{ 0b10110000, 1, newValue }
            },
        };

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

            foreach(var axis in AxisDatas)
            {
                axis.BoundVariable?.OnTrigger(axis.ValueSupplier(currentPoint), axis.MaxSupplier(innerFrame));
            }
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

        private void AxisComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            boundComboBox.SelectedValue = axisComboBox.SelectedValue?.As<AxisData>().BoundVariable ?? BoundValues[0];
        }

        private void BoundComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var boundValue = boundComboBox.SelectedValue?.As<BoundValue>() ?? BoundValues[0];
            axisComboBox.SelectedValue.As<AxisData>().BoundVariable = boundValue;
            pulseButton.Visibility = boundValue.NeedPulse ? Visibility.Visible : Visibility.Collapsed;
            pulseButton.Content = "Pulse " + boundValue.Name;
        }

        private void PulseButton_Click(object sender, RoutedEventArgs e)
        {
            axisComboBox.SelectedValue.As<AxisData>().BoundVariable.OnTrigger(0, axisComboBox.SelectedValue.As<AxisData>().MaxSupplier(innerFrame));
        }
    }
}
