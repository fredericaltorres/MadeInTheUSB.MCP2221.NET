namespace MadeInTheUSB
{
    public class GPIO
    {
        public PinState State { get; set; }
        public PinDirection Direction { get; set; }
        public int Index { get; set; }
        public PinDesignation Designation { get; set; }
    }
}
