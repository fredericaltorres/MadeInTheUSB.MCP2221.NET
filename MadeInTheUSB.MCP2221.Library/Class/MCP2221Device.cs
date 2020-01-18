using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCP2221;


namespace MadeInTheUSB.MCP2221.Lib
{
    public interface IGPIO
    {
        PinState DigitalRead(int index);
        void DigitalWrite(int index, PinState on);
        void DigitalWrite(int index, bool high);

        void SetPinDirection(int index, PinDirection direction);
    }
    /// <summary>
    /// Reference Document: MCP2221 DLL User Manual.pdf
    /// https://www.microchip.com/wwwproducts/en/MCP2221
    /// Datasheet http://ww1.microchip.com/downloads/en/DeviceDoc/20005292C.pdf
    /// MCP2221 Breakout Module User’s Guide - http://ww1.microchip.com/downloads/en/devicedoc/50002282a.pdf
    /// MCP2221 I2C Demonstration Board User’s Guide - http://ww1.microchip.com/downloads/en/devicedoc/50002480a.pdf
    /// </summary>
    public partial class MCP2221Device : IGPIO
    {
        public const int MAX_GPIO = 4;

        public static Dictionary<int, string> ErrorDescriptions = new Dictionary<int, string>()
        {
            {    3 , "Command not allowed The flash is either locked or password protected. If password protected, send correct access password to unlock flash for editing." },
            {   -1 , "Board not found. Check connection and device enumeration" },
            {   -2 , "Wrong device ID. Ensure DLL was initialized properly" },
            {   -3 , "Reading the device failed. Ensure DLL was initialized properly" },
            {   -4 , "Device write failed" },
            {   -5 , "Device read failed" },
            {  -10 , "GP pin not configured as GPIO Configure GP pin as GPIO and try operation again." },
            {  -11 , "I2C Slave data NACK received" },
            {  -12 , "Wrong PEC" },
            {  -13 , "Flash locked" },
            {  -14 , "Password attempt limit reach" },
            {  -15 , "Invalid state" },
            {  -16 , "Invalid data length" },
            {  -17 , "Error copying memory" },
            {  -18 , "Timeout" },
            {  -19 , "I2C send error" },
            {  -20 , "Error setting I2C address" },
            {  -21 , "Error setting I2C speed" },
            {  -22 , "Invalid I2C status" },
            {  -23 , "Address NACK received" },
            { -201 , "Invalid parameter given(1st parameter) 1" },
            { -202 , "Invalid parameter given(2nd parameter) 2" },
            { -203 , "Invalid parameter given(3rd parameter) 3" },
            { -204 , "Invalid parameter given(4th parameter) 4" },
            { -205 , "Invalid parameter given(5th parameter) 5" },
            { -206 , "Invalid parameter given(6th parameter) 6" },
            { -207 , "Invalid parameter given(7th parameter) 7" },
            { -208 , "Invalid parameter given(8th parameter) 8" },
            { -209 , "Invalid parameter given(9th parameter) 9" },
        };

        private static MchpUsbI2c _mchpUsbI2c;
        public static bool Detect(int index = 0)
        {
            if (_mchpUsbI2c == null)
                _mchpUsbI2c = new MchpUsbI2c();

            if (_mchpUsbI2c.Settings.GetConnectionStatus())
                return _mchpUsbI2c.Management.SelectDev(index) == 0;

            return false;
        }

        public MCP2221Device(int index)
        {
            CheckErrorCode(_mchpUsbI2c.Management.SelectDev(index), $"Select device {index}");
        }

        public bool SetGpioSettings(List<GPIO> gpios)
        {
            var r = new List<GPIO>();

            byte[] ioPinDesignations = new byte[MAX_GPIO];
            byte[] ioPinDirections = new byte[MAX_GPIO];
            byte[] ioPinValues = new byte[MAX_GPIO];

            for (var i = 0; i < MAX_GPIO; i++)
            {
                ioPinDesignations[i] = (byte)gpios[i].Designation;
                ioPinDirections[i] = (byte)gpios[i].Direction;
                ioPinValues[i] = (byte)gpios[i].State;
            }

            CheckErrorCode(_mchpUsbI2c.Settings.SetGpPinConfiguration(DllConstants.CURRENT_SETTINGS_ONLY, ioPinDesignations, ioPinDirections, ioPinValues), nameof(SetGpioSettings));

            return true;
        }
        
        public List<GPIO> GetGpioSettings()
        {
            var r = new List<GPIO>();

            byte[] ioPinDesignations = new byte[MAX_GPIO];
            byte[] ioPinDirections = new byte[MAX_GPIO];
            byte[] ioPinValues = new byte[MAX_GPIO];

            CheckErrorCode(_mchpUsbI2c.Settings.GetGpPinConfiguration(DllConstants.CURRENT_SETTINGS_ONLY, ioPinDesignations, ioPinDirections, ioPinValues), nameof(GetGpioSettings));

            for(var i=0; i < MAX_GPIO; i++)
                r.Add(new GPIO { Index = 0, Direction = (PinDirection)ioPinDirections[i], State = (PinState)ioPinValues[i], Designation = (PinDesignation)ioPinDesignations[i] });

            return r;
        }

        private void CheckErrorCode(int r, string message)
        {
            if (ErrorDescriptions.ContainsKey(r))
                throw new MCP2221DeviceException($"{message}, Code:{r} {ErrorDescriptions[r]}");

            if(r < 0) // Un documented error
                throw new MCP2221DeviceException($"{message}, Code:{r}");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append($"UsbStringDescriptor: {_mchpUsbI2c.Settings.GetUsbStringDescriptor()}").AppendLine();
            sb.Append($"UsbStringManufacturer: {_mchpUsbI2c.Settings.GetUsbStringManufacturer()}").AppendLine();
            sb.Append($"FirmwareVersion: {_mchpUsbI2c.Settings.GetFirmwareVersion()}").AppendLine();
            sb.Append($"SerialNumber: {_mchpUsbI2c.Settings.GetSerialNumber()}").AppendLine();
            sb.Append($"ConnectionStatus: {_mchpUsbI2c.Settings.GetConnectionStatus()}").AppendLine();
            sb.Append($"FlashProtectionState: {(FlashProtectionState)(_mchpUsbI2c.Settings.GetFlashProtectionState())}").AppendLine();
            sb.Append($"InitialPinValueSspnd(!): {_mchpUsbI2c.Settings.GetInitialPinValueSspnd()}").AppendLine();
            sb.Append($"InitialPinValueUsbcfg: {_mchpUsbI2c.Settings.GetInitialPinValueUsbcfg()}").AppendLine();
            sb.Append($"InitialPinValueLedI2c: {_mchpUsbI2c.Settings.GetInitialPinValueLedI2c()}").AppendLine();
            sb.Append($"UsbCurrentRequirement: {_mchpUsbI2c.Settings.GetUsbCurrentRequirement()} mA").AppendLine();
            sb.Append($"UsbPowerSource: {(UsbPowerSource)_mchpUsbI2c.Settings.GetUsbPowerSource()}").AppendLine();

            int r = _mchpUsbI2c.Settings.GetClockPinDividerValue(DllConstants.CURRENT_SETTINGS_ONLY); //  Get the current (SRAM) setting of the clock pin divider value
            sb.Append($"Clock pin divider: {(1 << r)}").AppendLine();

            return sb.ToString();
        }

        public void SetPinDirection(int index, PinDirection direction)
        {
            var gpios = this.GetGpioSettings();
            gpios[index].Designation = PinDesignation.GPIO;
            gpios[index].Direction = direction;
            this.SetGpioSettings(gpios);
        }

        public PinState DigitalRead(int index)
        {
            var r = _mchpUsbI2c.Functions.ReadGpioPinValue((byte)index);
            if (r < 0)
                CheckErrorCode(r, nameof(DigitalRead));
            return r == 0 ? PinState.Low : PinState.High;
        }

        public void DigitalWrite(int index, bool high)
        {
            var r = _mchpUsbI2c.Functions.WriteGpioPinValue((byte)index, (byte)(high ? 1 : 0));
            CheckErrorCode(r, nameof(DigitalRead));
        }

        public void DigitalWrite(int index, PinState on)
        {
            DigitalWrite(index, on == PinState.High);
        }

        public IEnumerable<int> GpioIndexes
        {
            get
            {
                return Enumerable.Range(1, MAX_GPIO).Select(x => x * x);
            }
        }
    }
}
