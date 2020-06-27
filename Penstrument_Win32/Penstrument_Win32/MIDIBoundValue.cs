using System;

namespace Penstrument_Win32
{
    public abstract class MIDIBoundValue : BoundValue
    {
        public Func<byte, byte[]> Builder { get; set; }

        public MIDIBoundValue(string name, string type, bool needPulse) : base(name, type, needPulse){}

        public override void OnTrigger(double newValue, double max)
        {
            MainWindow.MIDIPort.sendCommand(Builder?.Invoke(ApplyRange(newValue, max)));
        }
    }
}
