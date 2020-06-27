namespace Penstrument_Win32
{
    public class PitchBendValue : MIDIBoundValue
    {
        public PitchBendValue() : base("Pitch Bend", "Pitch Bend", false)
        {
            Builder = (b) => new byte[] { 0b11100000, 0, b };
        }
    }
}
