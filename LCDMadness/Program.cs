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
                string serialPort = "COM3";
                SerialPort mySerialPort = new SerialPort(serialPort);
                //SerialPort mySerialPort = new SerialPort("COM3");

                mySerialPort.BaudRate = 9600;
                mySerialPort.Parity = Parity.None;
                mySerialPort.StopBits = StopBits.One;
                mySerialPort.DataBits = 8;
                mySerialPort.Handshake = Handshake.None;
                mySerialPort.Open();

                //create a new char array to store the various commands we want to send
                //this char value will be converted to a dec value by the arduino code SerialTalkLCD
                          

                char[] array1 = { '^','~','@','E','>' };

                //mySerialPort.Write(array1, 1, 1); // turn LCD on
                //mySerialPort.Write(array1, 0, 1); // clear the screen
                //string timeNow = System.DateTime.Now.ToShortTimeString();
                ////mySerialPort.WriteLine("hello world!");
                //mySerialPort.Write(array1, 4, 1); // form feed
                //mySerialPort.WriteLine(timeNow);
                //mySerialPort.Write(array1, 3, 1); // turn LCD off              
                //mySerialPort.Close();

                wunderground.getWeather();
                //mySerialPort.WriteLine();
            }
            catch 
            {
                //TODO: SOME ERROR HANDLING HERE
            }            
        }

       
    }
}
