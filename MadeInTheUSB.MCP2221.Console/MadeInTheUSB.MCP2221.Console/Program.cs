using MadeInTheUSB.MCP2221.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadeInTheUSB.MCP2221.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            if(MCP2221Device.Detect())
            {
                var d = new MCP2221Device(0);
                System.Console.WriteLine(d.ToString());
                var gpios = d.GetGpioSettings();

                foreach(var index in d.GpioIndexes) 
                    d.SetPinDirection(index, PinDirection.Output);

                foreach (var index in d.GpioIndexes)
                    d.DigitalWrite(index, PinState.High);

                gpios = d.GetGpioSettings();
                //d.SetGpioSettings(gpios);
            }
        }
    }
}
