using System.Collections.Generic;
using MCP2221;


namespace MadeInTheUSB.MCP2221.Lib
{
    public class MCP2221DeviceBase
    {
        protected static MchpUsbI2c _mchpUsbI2c;

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
        protected void CheckErrorCode(int r, string message)
        {
            if (ErrorDescriptions.ContainsKey(r))
                throw new MCP2221DeviceException($"{message}, Code:{r} {ErrorDescriptions[r]}");

            if (r < 0) // Un documented error
                throw new MCP2221DeviceException($"{message}, Code:{r}");
        }
    }
}
