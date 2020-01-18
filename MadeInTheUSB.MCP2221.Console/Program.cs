using MadeInTheUSB.Adafruit;
using MadeInTheUSB.MCP2221.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MadeInTheUSB.MCP2221.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (MCP2221Device.Detect())
                {
                    var d = new MCP2221Device(0);
                    System.Console.WriteLine(d.ToString());
                    var gpios = d.GetGpioSettings();

                    //d.SetPinDirection(d.GpioIndexes, PinDirection.Output, PinState.Low);

                    //foreach (var index in d.GpioIndexes)
                    //{
                    //    d.DigitalWrite(index, PinState.High);
                    //    Thread.Sleep(1 * 200);
                    //}

                    //foreach (var index in d.GpioIndexes)
                    //{
                    //    d.DigitalWrite(index, PinState.Low);
                    //    Thread.Sleep(1 * 200);
                    //}

                    //gpios = d.GetGpioSettings();
                    //d.SetPinDirection(0, PinDirection.Input, PinState.Low);
                    //while (true)
                    //{
                    //    System.Console.WriteLine($"Pin[0] Read:{d.DigitalRead(0)}");
                    //    Thread.Sleep(1 * 1000);
                    //}


                    var ledBackpack = new LEDBackpack(8, 8, d.GetI2CDeviceInstance(0x70));
                    if(ledBackpack.Detect(0x70))
                    {
                        ledBackpack.DrawRect(0, 0, 6, 6, true);
                        ledBackpack.WriteDisplay();
                    }
                }
            }
            catch(Exception ex)
            {
                System.Console.WriteLine($"Exception {ex.Message}");
            }
        }
    }
}
