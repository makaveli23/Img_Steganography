using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Img_Steganography.Functionality
{
    public static class ByteArrayExtension
    {
        public static byte SetLastBit(this byte aByte, bool value)
        {
            return value ? (byte)(aByte | (1 << 0)) : (byte)(aByte & ~(1 << 0));           
        }

        public static byte Set2LastBits(this byte aByte, bool value1, bool value2)
        {
            byte temp;
            temp = value1 ? (byte)(aByte | (1 << 1)) : (byte)(aByte & ~(1 << 1));
            temp = value2 ? (byte)(temp | (1 << 0)) : (byte)(temp & ~(1 << 0));
            return temp;
        }

        public static bool GetBit(byte aByte, int pos)
        {
            //left-shift 1, then bitwise AND, then check for non-zero
            return ((aByte & (1 << pos)) != 0);
        }
    }
}
