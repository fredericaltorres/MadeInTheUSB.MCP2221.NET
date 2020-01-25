using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCP2221;

namespace MadeInTheUSB.MCP2221.Lib
{
    /// <summary>
    /// Reference Document: MCP2221 DLL User Manual.pdf
    /// https://www.microchip.com/wwwproducts/en/MCP2221
    /// Datasheet http://ww1.microchip.com/downloads/en/DeviceDoc/20005292C.pdf
    /// MCP2221 Breakout Module User’s Guide - http://ww1.microchip.com/downloads/en/devicedoc/50002282a.pdf
    /// MCP2221 I2C Demonstration Board User’s Guide - http://ww1.microchip.com/downloads/en/devicedoc/50002480a.pdf
    /// 
    /// https://blog.zakkemble.net/mcp2221-hid-library/
    /// https://github.com/kelray/MCP2221-Examples-for-Windows
    /// 
    /// https://github.com/kelray/MCP2221-Examples-for-Windows
    /// </summary>
    public partial class MCP2221Device : MCP2221DeviceBase, IGPIO
    {
        public const int MAX_GPIO = 4;

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
            if (_mchpUsbI2c == null)
                throw new ArgumentException("Detect device first");

            CheckErrorCode(_mchpUsbI2c.Management.SelectDev(index), $"Select device {index}");

            var r = _mchpUsbI2c.Management.GetSelectedDevInfo();
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
            var gpios = new List<GPIO>();

            byte[] ioPinDesignations = new byte[MAX_GPIO];
            byte[] ioPinDirections = new byte[MAX_GPIO];
            byte[] ioPinValues = new byte[MAX_GPIO];

            CheckErrorCode(_mchpUsbI2c.Settings.GetGpPinConfiguration(DllConstants.CURRENT_SETTINGS_ONLY, ioPinDesignations, ioPinDirections, ioPinValues), nameof(GetGpioSettings));

            for(var i = 0 ; i < MAX_GPIO; i++)
            {
                var gpio = new GPIO
                {
                    Index = 0,
                    Direction = (PinDirection)ioPinDirections[i],
                    Designation = (PinDesignation)ioPinDesignations[i],
                    State = (PinState)ioPinValues[i], // this seems to always return 1
                };
                if (gpio.Designation == PinDesignation.GPIO)
                    gpio.State = this.DigitalRead(i);

                gpios.Add(gpio);
            }

            return gpios;
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
            sb.Append($"AdcVoltageReference: {_mchpUsbI2c.Settings.GetAdcVoltageReference()}").AppendLine();
            sb.Append($"InitialPinValueLedI2c: {_mchpUsbI2c.Settings.GetInitialPinValueLedI2c()}").AppendLine();

            int r = _mchpUsbI2c.Settings.GetClockPinDividerValue(DllConstants.CURRENT_SETTINGS_ONLY); //  Get the current (SRAM) setting of the clock pin divider value
            sb.Append($"Clock pin divider: {(1 << r)}").AppendLine();
            sb.Append($"ClockPinDividerValue: {(UsbPowerSource)_mchpUsbI2c.Settings.GetClockPinDividerValue(DllConstants.CURRENT_SETTINGS_ONLY)}").AppendLine();
            sb.Append($"GetClockPinDutyCycle: {(UsbPowerSource)_mchpUsbI2c.Settings.GetClockPinDutyCycle(DllConstants.CURRENT_SETTINGS_ONLY)}").AppendLine();

            return sb.ToString();
        }

        public void SetPinDirection(IEnumerable<int> indexes, PinDirection direction, PinState? state = null)
        {
            foreach (var index in indexes)
                this.SetPinDirection(index, direction, state);
        }

        public void SetPinDirection(int index, PinDirection direction)
        {
            this.SetPinDirection(index, direction);
        }

        public void SetPinDirection(int index, PinDirection direction, PinState? state = null)
        {
            var gpios = this.GetGpioSettings();
            gpios[index].Designation = PinDesignation.GPIO;
            gpios[index].Direction = direction;
            if (state.HasValue)
                gpios[index].State = state.Value;
            this.SetGpioSettings(gpios);
        }

        public void SetToAnalogMode(int index)
        {
            var gpios = this.GetGpioSettings();
            gpios[index].Designation = PinDesignation.ADC;
            gpios[index].Direction = PinDirection.Input;
            gpios[index].State = 0;
            this.SetGpioSettings(gpios);
        }

        // _mchpUsbI2c.Settings.GetAdcVoltageReference() +++


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
                return Enumerable.Range(0, MAX_GPIO);
            }
        }

        public I2CDevice GetI2CDeviceInstance(byte address, int clockSpeed = I2CDevice.DEFAULT_I2C_SPEED)
        {
            return new I2CDevice(address, clockSpeed);
        }
        public AnalogDevice GetAnalogDevice(int index)
        {
            return new AnalogDevice(index, this);
        }
    }
}
