using System;

namespace Penstrument_Win32
{
    public class MIDIBoundValue : BoundValue
    {
        public Func<byte, byte[]> Builder { get; set; }

        public MIDIBoundValue(string name, bool needPulse) : base(name, needPulse) {}

        public override void OnTrigger(double newValue, double max)
        {
            MainWindow.MIDIPort.sendCommand(Builder?.Invoke((byte)(newValue / max * 127)));
        }
    }
}
