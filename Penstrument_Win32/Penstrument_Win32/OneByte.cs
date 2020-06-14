namespace Penstrument_Win32
{
    class OneByte : ValueType
    {
        readonly byte value;

        public OneByte(byte value)
        {
            this.value = value;
        }

        public byte ToByte()
        {
            return value;
        }
    }
}
