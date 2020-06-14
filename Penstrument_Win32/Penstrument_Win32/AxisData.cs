using Microsoft.UI.Xaml.Shapes;
using System;
using Windows.UI.Input;

namespace Penstrument_Win32
{
    public class AxisData
    {
        public string Name { get; private set; }
        public Func<PointerPoint, double> ValueSupplier { get; set; }
        public Func<Rectangle, double> MaxSupplier { get; set; }
        public BoundValue BoundVariable { get; set; }

        public AxisData(string name)
        {
            Name = name;
        }
    }
}
