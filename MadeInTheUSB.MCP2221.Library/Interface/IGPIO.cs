namespace MadeInTheUSB.MCP2221.Lib
{
    public interface IGPIO
    {
        PinState DigitalRead(int index);
        void DigitalWrite(int index, PinState on);
        void DigitalWrite(int index, bool high);

        void SetPinDirection(int index, PinDirection direction);
    }
}
