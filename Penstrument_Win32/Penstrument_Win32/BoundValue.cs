namespace Penstrument_Win32
{
    public abstract class BoundValue
    {
        public string Name { get; private set; }
        public bool NeedPulse { get; private set; }

        public BoundValue(string name, bool needPulse)
        {
            Name = name;
            NeedPulse = needPulse;
        }

        public abstract void OnTrigger(double newValue, double max);
    }
}
