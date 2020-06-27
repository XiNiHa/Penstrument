using System;
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

        string[] BoundTypes { get; } = new string[] { "Note I/O", "Pitch Bend", "Control Change (CC)", "Variable" };

        public MainWindow()
        {
            InitializeComponent();

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
            theCanvas.Width = rootWidth - 220;
            theCanvas.Height = rootHeight;

            outerFrame.Width = theCanvas.Width - 20;
            outerFrame.Height = theCanvas.Height - 20;

            innerFrame.Width = outerFrame.Width - 40;
            innerFrame.Height = outerFrame.Height - 40;
        }

        private void UpdateBoundDialog()
        {
            var label = (string)boundComboBox.SelectedValue;

            noteIOPanel.Visibility = label == "Note I/O" ? Visibility.Visible : Visibility.Collapsed;
            ccPanel.Visibility = label == "Control Change (CC)" ? Visibility.Visible : Visibility.Collapsed;
            variablePanel.Visibility = label == "Variable" ? Visibility.Visible : Visibility.Collapsed;
            try
            {
                var noteIOValue = axisComboBox.SelectedValue.As<AxisData>().BoundVariable.As<NoteIOValue>();
                noteINumberBox.Value = noteIOValue.IsInFromVariable ? noteIOValue.InVariableID : noteIOValue.InValue;
                noteISwitch.IsOn = noteIOValue.IsInFromVariable;
                noteONumberBox.Value = noteIOValue.IsOutFromVariable ? noteIOValue.OutVariableID : noteIOValue.OutValue;
                noteOSwitch.IsOn = noteIOValue.IsOutFromVariable;
            }
            catch (Exception)
            {
                noteINumberBox.Value = 0;
                noteISwitch.IsOn = false;
                noteONumberBox.Value = 0;
                noteOSwitch.IsOn = false;
            }

            try
            {
                var ccBoundValue = axisComboBox.SelectedValue.As<AxisData>().BoundVariable.As<CCBoundValue>();
                ccNumberBox.Value = ccBoundValue.CCNumber;
            }
            catch (Exception)
            {
                ccNumberBox.Value = 0;
            }

            try
            {
                var variableValue = axisComboBox.SelectedValue.As<AxisData>().BoundVariable.As<VariableValue>();
                variableIDBox.Value = variableValue.ID;
            }
            catch (Exception)
            {
                variableIDBox.Value = 0;
            }
        }

        private bool IsValidToBuildBinding()
        {
            switch ((string)boundComboBox.SelectedValue) {
                case "":
                    return false;
                case "Pitch Bend":
                    return true;
                case "Note I/O":
                    if (noteISwitch.IsOn && !VariableValue.Variables.ContainsKey((int)noteINumberBox.Value)) return false;
                    if (noteOSwitch.IsOn && !VariableValue.Variables.ContainsKey((int)noteONumberBox.Value)) return false;
                    return true;
                case "Control Change (CC)":
                    if (ccNumberBox.Value > 119 || ccNumberBox.Value < 0) return false;
                    return true;
                case "Variable":
                    if (VariableValue.Variables.ContainsKey((int)variableIDBox.Value)) return false;
                    return true;
            }

            return false;
        }

        private void AxisComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            boundTextBox.Text = axisComboBox.SelectedValue?.As<AxisData>().BoundVariable?.Name ?? "";
        }

        private void BoundComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateBoundDialog();
        }

        private void PulseButton_Click(object sender, RoutedEventArgs e)
        {
            axisComboBox.SelectedValue.As<AxisData>().BoundVariable.OnTrigger(0, axisComboBox.SelectedValue.As<AxisData>().MaxSupplier(innerFrame));
        }

        private void MinSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            axisComboBox.SelectedValue.As<AxisData>().BoundVariable.Range = new Tuple<byte, byte>((byte)minSlider.Value, (byte)maxSlider.Value);
        }

        private void MaxSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            axisComboBox.SelectedValue.As<AxisData>().BoundVariable.Range = new Tuple<byte, byte>((byte)minSlider.Value, (byte)maxSlider.Value);
        }

        private async void BoundButton_Click(object sender, RoutedEventArgs e)
        {
            var currentBoundVar = axisComboBox.SelectedValue.As<AxisData>().BoundVariable;
            boundComboBox.SelectedValue = currentBoundVar == null ? "None" : currentBoundVar.Type;
            UpdateBoundDialog();

            var result = await boundValueDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (IsValidToBuildBinding())
                {
                    switch((string)boundComboBox.SelectedValue)
                    {
                        case "Pitch Bend":
                            axisComboBox.SelectedValue.As<AxisData>().BoundVariable = new PitchBendValue();
                            break;
                        case "Note I/O":
                            axisComboBox.SelectedValue.As<AxisData>().BoundVariable = new NoteIOValue
                            (
                                noteISwitch.IsOn,
                                (int)noteINumberBox.Value,
                                noteOSwitch.IsOn,
                                (int)noteONumberBox.Value
                            );
                            break;
                        case "Control Change (CC)":
                            axisComboBox.SelectedValue.As<AxisData>().BoundVariable = new CCBoundValue((byte)ccNumberBox.Value);
                            break;
                        case "Variable":
                            var vb = new VariableValue((int)variableIDBox.Value);
                            axisComboBox.SelectedValue.As<AxisData>().BoundVariable = vb;
                            VariableValue.Variables.Add((int)variableIDBox.Value, vb);
                            break;
                    }

                    var boundValue = axisComboBox.SelectedValue.As<AxisData>().BoundVariable;
                    pulseButton.Visibility = boundValue.NeedPulse ? Visibility.Visible : Visibility.Collapsed;
                    pulseButton.Content = "Pulse " + boundValue.Name;

                    minSlider.Value = boundValue.Range.Item1;
                    maxSlider.Value = boundValue.Range.Item2;

                    boundTextBox.Text = boundValue.Name;
                }
            }
        }
    }
}
