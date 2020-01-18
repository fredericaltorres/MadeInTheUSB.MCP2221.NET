namespace MadeInTheUSB.MCP2221.Lib
{
    public partial class MCP2221Device
    {
        public enum FlashProtectionState
        {
            Unsecured = 0,
            PasswordProtectionEnabled = 1,
            PermanentlyLocked = 2,
        }
    }
}
