using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.IO;
using System.IO.Ports;


namespace LCDMadness
{
    class wunderground
    {
        public static void getWeather()
        {
            //Start wunderground API request
            Console.WriteLine("Starting C# Weather Undeground Web API Test...");
            string wunderground_key = "5f9f7844dd2b0623"; // You'll need to goto http://www.wunderground.com/weather/api/, and get a key to use the API.

            parse("http://api.wunderground.com/api/" + wunderground_key + "/conditions/q/VA/Seattle.xml");
            //parse("http://api.wunderground.com/api/" + wunderground_key + "/conditions/q/VA/Springfield.xml");
            //parse("http://api.wunderground.com/api/" + wunderground_key + "/conditions/q/NY/New_York.xml");
            //parse("http://api.wunderground.com/api/" + wunderground_key + "/conditions/q/CA/Oceanside.xml");
            //parse("http://api.wunderground.com/api/" + wunderground_key + "/conditions/q/CA/Mission_Beach.xml");
            //parse("http://api.wunderground.com/api/" + wunderground_key + "/conditions/q/VA/Lorton.xml");


            // End.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
        //Takes a url request to wunderground, parses it, and displays the data.
        private static void parse(string input_xml)
        {
            //Variables
            string place = "";
            string obs_time = "";
            string weather1 = "";
            string temperature_string = "";
            string relative_humidity = "";
            string wind_string = "";
            string pressure_mb = "";
            string dewpoint_string = "";
            string visibility_km = "";
            string latitude = "";
            string longitude = "";
            string feelslike = "";

            var cli = new WebClient();
            string weather = cli.DownloadString(input_xml);

            using (XmlReader reader = XmlReader.Create(new StringReader(weather)))
            {
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.Equals("full"))
                            {
                                reader.Read();
                                place = reader.Value;
                            }
                            else if (reader.Name.Equals("observation_time"))
                            {
                                reader.Read();
                                obs_time = reader.Value;
                            }
                            else if (reader.Name.Equals("weather"))
                            {
                                reader.Read();
                                weather1 = reader.Value;
                            }
                            else if (reader.Name.Equals("temperature_string"))
                            {
                                reader.Read();
                                temperature_string = reader.Value;
                            }
                            else if (reader.Name.Equals("relative_humidity"))
                            {
                                reader.Read();
                                relative_humidity = reader.Value;
                            }
                            else if (reader.Name.Equals("wind_string"))
                            {
                                reader.Read();
                                wind_string = reader.Value;
                            }
                            else if (reader.Name.Equals("pressure_mb"))
                            {
                                reader.Read();
                                pressure_mb = reader.Value;
                            }
                            else if (reader.Name.Equals("dewpoint_string"))
                            {
                                reader.Read();
                                dewpoint_string = reader.Value;
                            }
                            else if (reader.Name.Equals("visibility_km"))
                            {
                                reader.Read();
                                visibility_km = reader.Value;
                            }
                            else if (reader.Name.Equals("latitude"))
                            {
                                reader.Read();
                                latitude = reader.Value;
                            }
                            else if (reader.Name.Equals("longitude"))
                            {
                                reader.Read();
                                longitude = reader.Value;
                            }
                            else if (reader.Name.Equals("feelslike_string"))
                            {
                                reader.Read();
                                feelslike = reader.Value;
                            }

                            break;
                    }
                }
            }

            Console.WriteLine("********************");
            Console.WriteLine("Place:             " + place);
            Console.WriteLine("Observation Time:  " + obs_time);
            Console.WriteLine("Weather:           " + weather1);
            Console.WriteLine("Temperature:       " + temperature_string);
            Console.WriteLine("Relative Humidity: " + relative_humidity);
            Console.WriteLine("Wind:              " + wind_string);
            Console.WriteLine("Pressure (mb):     " + pressure_mb);
            Console.WriteLine("Dewpoint:          " + dewpoint_string);
            Console.WriteLine("Visibility (km):   " + visibility_km);
            Console.WriteLine("Location:          " + longitude + ", " + latitude);
            Console.WriteLine("FeelsLike:)        " + feelslike);
            // now write to the LCD over the serial port
            string[] weatherArray = new string[] {temperature_string,"^",feelslike};
            writeToSerial(weatherArray);
        }
        private static void writeToSerial(string [] weatherArgsArray)
        {
            try
            {
                string serialPort = "COM4"; //com3 for home pc com4 for devbox
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


                char[] array1 = { '^', '~', '@', 'E', '>' };

                //mySerialPort.Write(array1, 1, 1); // turn LCD on
                //mySerialPort.Write(array1, 0, 1); // clear the screen
                //string timeNow = System.DateTime.Now.ToShortTimeString();
                ////mySerialPort.WriteLine("hello world!");
                //mySerialPort.Write(array1, 4, 1); // form feed
                //mySerialPort.WriteLine(timeNow);
                //mySerialPort.Write(array1, 3, 1); // turn LCD off              
                //mySerialPort.Close();

                foreach (string weatherData in weatherArgsArray)
                {
                    //mySerialPort.Write(array1, 0, 1); // clear the screen
                    mySerialPort.WriteLine(weatherData);
                }
            }
            catch
            {
                //TODO: SOME ERROR HANDLING HERE
            }
        }
    }
}
