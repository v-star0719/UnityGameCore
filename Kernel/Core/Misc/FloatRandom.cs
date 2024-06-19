using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel.Core
{
    public class FloatRandom
    {
        public int Seed { get; private set; }

        private Random random;

        public FloatRandom(int seed)
        {
            random = new Random(seed);
            Seed = seed;
        }

        ///0~1的数
        public float Value()
        {
            return (float) random.NextDouble();
        }

        public float Range(float a, float b)
        {
            return a + (b - a) * Value();
        }
    }
}
