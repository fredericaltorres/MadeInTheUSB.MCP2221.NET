namespace MadeInTheUSB
{
    public enum PinDirection : byte
    {
        Output = 0,
        Input = 1,
        InputPullUp = 2, // Not Support by Nusbio, Just by Arduino controlled by Nusbio
    }
}
