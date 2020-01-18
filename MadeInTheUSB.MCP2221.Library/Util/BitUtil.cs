using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MadeInTheUSB
{
    public class BitUtil
    {
        public static List<int> ParseBinary(List<string> binaryValues)
        {
            var l = new List<int>();
            foreach (var bv in binaryValues)
                l.Add(ParseBinary(bv));
            return l;
        }

        public static int ParseBinary(string s)
        {
            if (s.ToUpperInvariant().StartsWith("B"))
                return Convert.ToInt32(s.Substring(1), 2);
            else 
                throw new ArgumentException(string.Format("Invalid binary value:{0}", s));
        }

        public static byte UnsetBit(byte value, byte bit)
        {
            value &= ((byte)~bit);
            return value;
        }

        public static ushort Byte2UInt16(byte high_byte, byte low_byte)
        {
            int a = ((high_byte << 8)) | (low_byte);
            return (ushort)a;
        }

        public static List<byte> SliceBuffer(List<byte> buffer, int start, int count)
        {
            var l = new List<byte>();
            for (var i = start; i < start + count; i++)
                l.Add(buffer[i]);
            return l;
        }

        public static byte HighByte(ushort number)
        {
            byte upper = (byte)(number >> 8);
            return upper;
        }

        public static byte LowByte(ushort number)
        {
            byte lower = (byte)(number & 0xff);
            return lower;
        }

        public static bool IsSet(int value, byte bit)
        {
            return IsSet((byte)value, bit);
        }

        public static bool IsSet(byte value, byte bit)
        {
            if (value == 0 && bit == 0)
                return false;
            return (value & bit) == bit;
        }

        public static byte UnsetBitByIndex(byte value, int bitIndex)
        {
            var bit = (byte)Math.Pow(2, bitIndex);
            return UnsetBit(value, bit);
        }

        public static byte SetBitIndex(byte value, int bitIndex)
        {
            var bit = (byte)Math.Pow(2, bitIndex);
            return SetBit(value, bit);
        }

        public static byte SetBit(byte value, byte bit)
        {
            value |= bit;
            return value;
        }

        public static byte SetBitOnOff(int value, byte bit, bool on)
        {
            if (on)
                return SetBit((byte)value, bit);
            else
                return UnsetBit((byte)value, bit);
        }
    }
}

