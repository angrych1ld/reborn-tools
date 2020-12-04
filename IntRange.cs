using System;

namespace RebornTools
{
    public struct IntRange
    {
        public int Min;
        public int Max;

        public static IntRange Zero => new IntRange { Min = 0, Max = 0 };

        public bool IsZero => Min == 0 && Max == 0;
    }
}
