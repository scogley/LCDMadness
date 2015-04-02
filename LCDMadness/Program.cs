using System;
using System.Collections.Generic;
using System.Linq;
//using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Diagnostics;


namespace LCDMadness
{
    class Program
    {
        static void Main(string[] args)
        {
            // Arduino Mega 2560 defaults to COM4 on my PC
            // serial port sample code: http://msdn.microsoft.com/en-us/library/system.io.ports.serialport.datareceived(v=vs.110).aspx

            try
            {               
                // get the weather forecast and send what to wear suggestion to customer
                wunderground.GetWeather();
                Customer customer1 = new Customer("sean", "cogley", 98021);
                Customer2 customer2 = new Customer2();
                
            }
            catch(Exception e) 
            {
                Console.WriteLine(e.ToString());
                //TODO: SOME ERROR HANDLING HERE
            }            
        }

       
    }
}
