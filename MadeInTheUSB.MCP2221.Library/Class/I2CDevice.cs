namespace MadeInTheUSB.MCP2221.Lib
{
    public enum ClockDivider { 
        MCP2221_CLKDIV_RESERVED = 0,    /**< Invalid */
        MCP2221_CLKDIV_2 = 1,           /**< 24MHz */
        MCP2221_CLKDIV_4 = 2,           /**< 12MHz */
        MCP2221_CLKDIV_8 = 3,           /**< 6MHz */
        MCP2221_CLKDIV_16 = 4,          /**< 3MHz */
        MCP2221_CLKDIV_32 = 5,          /**< 1.5MHz */
        MCP2221_CLKDIV_64 = 6,          /**< 750KHz */
        MCP2221_CLKDIV_128 = 7,         /**< 375KHz */
    };

    public class I2CDevice : MCP2221DeviceBase, II2CDevice
    {
        public const int DEFAULT_I2C_SPEED = 400 * 512;
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
