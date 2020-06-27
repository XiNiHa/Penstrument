using System.Collections.Generic;

namespace Penstrument_Win32
{
    class VariableValue : BoundValue
    {
        public static Dictionary<int, VariableValue> Variables { get; } = new Dictionary<int, VariableValue>();

        public int ID { get; private set; }
        public byte Value { get; private set; }

        public VariableValue(int id) : base("Variable " + id, "Variable", false) 
        {
            ID = id;
        }

        public override void OnTrigger(double newValue, double max)
        {
            Value = ApplyRange(newValue, max);
        }
    }
}
