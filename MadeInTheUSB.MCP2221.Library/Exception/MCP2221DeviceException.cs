using System;

namespace MadeInTheUSB.MCP2221.Lib
{
    [Serializable]
    class MCP2221DeviceException : Exception
    {
        public MCP2221DeviceException()
        {
        }

        public MCP2221DeviceException(string message) : base(message)
        {
        }
    }
}
