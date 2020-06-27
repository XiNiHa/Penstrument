using System;

namespace Penstrument_Win32
{
    public abstract class BoundValue
    {
        private Tuple<byte, byte> _range = new Tuple<byte, byte>(0, 127);

        public string Name { get; protected set; }
        public string Type { get; private set; }
        public bool NeedPulse { get; private set; }
        public Tuple<byte, byte> Range { 
            get { return _range; } 
            set
            {
                if (value.Item1 > value.Item2)
                {
                    _range = new Tuple<byte, byte>(value.Item2, value.Item1);
                }
                else
                {
                    _range = new Tuple<byte, byte>(value.Item1, value.Item2);
                }
            }
        }

        public BoundValue(string name, string type, bool needPulse)
        {
            Name = name;
            Type = type;
            NeedPulse = needPulse;
        }

        public byte ApplyRange(double newValue, double max)
        {
            return (byte)(newValue / max * (Range.Item2 - Range.Item1) + Range.Item1);
        }

        public abstract void OnTrigger(double newValue, double max);
    }
}
