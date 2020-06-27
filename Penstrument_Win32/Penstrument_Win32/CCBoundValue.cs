namespace Penstrument_Win32
{
    class CCBoundValue : MIDIBoundValue
    {
        private byte _ccNumber;

        public byte CCNumber {
            get => _ccNumber;
            set
            {
                _ccNumber = value;
                Name = "CC " + _ccNumber;
            }
        }

        public CCBoundValue(byte ccNumber) : base("CC " + ccNumber, "Control Change (CC)", true)
        {
            CCNumber = ccNumber;
            Builder = value => new byte[] { 0b10110000, CCNumber, value };
        }
    }
}
