using System;

namespace Penstrument_Win32
{
    class NoteIOValue : BoundValue
    {
        byte prev = 255;

        public bool IsInFromVariable { get; }
        public byte InValue { get; }
        public int InVariableID { get; }

        public bool IsOutFromVariable { get; }
        public byte OutValue { get; }
        public int OutVariableID { get; }

        public NoteIOValue(bool isInFromVariable, int inValue, bool isOutFromVariable, int outValue) : base("Note I/O", "Note I/O", false) 
        {
            IsInFromVariable = isInFromVariable;
            if (isInFromVariable) InVariableID = inValue;
            else InValue = (byte)inValue;

            IsOutFromVariable = isOutFromVariable;
            if (isOutFromVariable) OutVariableID = outValue;
            else OutValue = (byte)outValue;
        }

        public override void OnTrigger(double newValue, double max)
        {
            var curr = ApplyRange(newValue, max);

            if (prev != curr)
            {
                MainWindow.MIDIPort.sendCommand(new byte[] { 0b10010000, curr, IsInFromVariable ? VariableValue.Variables[InVariableID].Value : InValue });
                if (prev != 255)
                    MainWindow.MIDIPort.sendCommand(new byte[] { 0b10000000, prev, IsOutFromVariable ? VariableValue.Variables[OutVariableID].Value : OutValue });
            }

            prev = curr;
        }
    }
}
