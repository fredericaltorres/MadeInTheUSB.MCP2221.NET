using MCP2221;
using System;

namespace MadeInTheUSB.MCP2221.Lib
{
    public enum ClockDivider 
    { 
        MCP2221_CLKDIV_RESERVED = 0,    /**< Invalid */
        MCP2221_CLKDIV_2 = 1,           /**< 24MHz */
        MCP2221_CLKDIV_4 = 2,           /**< 12MHz */
        MCP2221_CLKDIV_8 = 3,           /**< 6MHz */
        MCP2221_CLKDIV_16 = 4,          /**< 3MHz */
        MCP2221_CLKDIV_32 = 5,          /**< 1.5MHz */
        MCP2221_CLKDIV_64 = 6,          /**< 750KHz */
        MCP2221_CLKDIV_128 = 7,         /**< 375KHz */
    };

    public enum ReferenceVoltage
    {
        r_Vdd = 0,
        r_1_024V = 1,
        r_2_048V = 2,
        r_4_096V = 33
    }

    public class AnalogDevice : MCP2221DeviceBase
    {
        private readonly MCP2221Device mcp2221;
        public int Index;

        public AnalogDevice(int index, MCP2221Device mcp2221)
        {
            if (!(index >= 1 && index <= 3))
                throw new ArgumentException($"GetAdc index:{index}");
            this.Index = index;
            this.mcp2221 = mcp2221;
            this.mcp2221.SetToAnalogMode(index);
            this.SetAdcVoltageReference(ReferenceVoltage.r_Vdd);
        }
        public double GetVoltageReferenceValue()
        {
            switch(GetVoltageReference())
            {
                case ReferenceVoltage.r_Vdd: return 3.3;
                case ReferenceVoltage.r_1_024V: return 1.024;
                case ReferenceVoltage.r_2_048V: return 2.048;
                case ReferenceVoltage.r_4_096V: return 4.096;
            }
            throw new ArgumentException($"Invalid voltage reference {GetVoltageReference()}");
        }

        public ReferenceVoltage GetVoltageReference()
        {
            return (ReferenceVoltage)_mchpUsbI2c.Settings.GetAdcVoltageReference();
        }
        public void SetAdcVoltageReference(ReferenceVoltage reference)
        {
            base.CheckErrorCode(_mchpUsbI2c.Settings.SetAdcVoltageReference(DllConstants.CURRENT_SETTINGS_ONLY, (int)reference), "SetAdcVoltageReference");
        }
        public int GetDigitalValue()
        {
            var adcData = new ushort[6];
            var r = _mchpUsbI2c.Functions.GetAdcData(adcData);
            return adcData[this.Index - 1];
        }
        public double GetVoltage()
        {
            var d = this.GetDigitalValue();
            var r = this.GetVoltageReferenceValue();
            var v = d / 1024.0; /* 10 bit adc*/
            var vv = v * r;
            return vv;
        }
    }

    public class I2CDevice : MCP2221DeviceBase, II2CDevice
    {
        public const int DEFAULT_I2C_SPEED = 400 * 1000;
        private byte _address;
        private readonly int _clockSpeed;

        public I2CDevice(byte address, int clockSpeed = DEFAULT_I2C_SPEED)
        {
            this._address = address;
            this._clockSpeed = clockSpeed;
        }

        public void SetAddress(byte address)
        {
            this._address = address;
        }

        public bool Write(byte [] buffer)
        {
            _mchpUsbI2c.Functions.StopI2cDataTransfer();
            base.CheckErrorCode(_mchpUsbI2c.Functions.WriteI2cData(this._address, buffer, (uint)buffer.Length, (uint)this._clockSpeed), $"{this.GetType().Name}.{nameof(Write)}");
            return true;
        }

        public byte [] Read(int count, byte [] buffer = null)
        {
            if(buffer == null)
                buffer = new byte[count];

            base.CheckErrorCode(_mchpUsbI2c.Functions.ReadI2cData(this._address, buffer, (uint)count, (uint)this._clockSpeed), $"{this.GetType().Name}.{nameof(Write)}");
            
            return buffer;
        }
    }
}
