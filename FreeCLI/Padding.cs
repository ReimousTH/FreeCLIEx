using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCLI
{
    public class Padding
    {
        public static int FixPadding(int value, int padding)
        {
            while (value % padding != 0)
            {
                value++;
            }
            return value;
        }

        public static int FixPaddingNew(int value, int padding)
        {
            if (value % padding == 0) value += padding;

            while (value % padding != 0)
            {
                value++;
            }
            return value;
        }

        public static int FixPaddingFixedL(int value, int padding)
        {
            var z = (value + ((padding + padding) - 1)) & ~(padding - 1);
            if (z % padding != 0)
            {
                return FixPaddingFixedL(value, padding);
            }
            else
            {
                return z;
            }


        }

        public static int FixPaddingFixed(int value, int padding)
        {

            return (value + ((padding + padding) - 1)) & ~(padding - 1);
        }
        public static int FixPaddingFixedEX(int value, int padding)
        {

            return (value + ((padding) - 1)) & ~(padding - 1);
        }
        public static long FixPaddingFixed(long value, int padding)
        {

            return (value + padding + padding - 1) & ~(padding - 1);
        }
        public static int FixPaddingFixedX(int value, int padding)
        {
            return (value + padding - 1) & ~(padding - 1);
        }
        public static uint FixPaddingFixedX(uint value, uint padding)
        {
            return (value + padding - 1) & ~(padding - 1);
        }
        public static int FixPaddingFixedX2(int value, int pad1, int pad2)
        {
            return (value + pad1 - 1) & ~(pad2 - 1);
        }
   
      

       
    }
}
