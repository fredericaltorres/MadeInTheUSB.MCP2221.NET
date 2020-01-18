namespace MadeInTheUSB.MCP2221.Lib
{
    public interface II2CDevice
    {
        bool Write(byte[] buffer);
        byte[] Read(int count, byte[] buffer = null);
        void SetAddress(byte address);
    }
}
