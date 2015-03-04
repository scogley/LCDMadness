using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.IO.Ports;



namespace LCDMadness
{
    class wunderground
    {
        //Takes a url request to wunderground, parses it, and displays the data.
        public static void GetWeather()
        {
            //Start wunderground API request
            Console.WriteLine("Starting C# Weather Undeground Web API Test...");
            string wunderground_key = "5f9f7844dd2b0623"; // You'll need to goto http://www.wunderground.com/weather/api/, and get a key to use the API.

            //parseConditions("http://api.wunderground.com/api/" + wunderground_key + "/conditions/q/VA/Seattle.xml");
            parseForecast("http://api.wunderground.com/api/" + wunderground_key + "/forecast/q/VA/Seattle.xml");
            //parseForecastLinq("http://api.wunderground.com/api/" + wunderground_key + "/forecast/q/VA/Seattle.xml");
            
            // End.
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
        // info on parsing XML is here: https://msdn.microsoft.com/en-us/library/system.xml.xmlreader.read(v=vs.110).aspx
        // info on parsing XML with elements of the same name: http://stackoverflow.com/questions/13642633/using-xmlreader-class-to-parse-xml-with-elements-of-the-same-name
        // tips on using Linq and XElement are here: http://www.dotnetperls.com/xelement
        

        // parse Conditions
        private static void parseConditions(string input_xml)
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
            string temp_f = "";

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
                            else if (reader.Name.Equals("temp_f"))
                            {
                                reader.Read();
                                temp_f = reader.Value;                                
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
            Console.WriteLine("TempF:)            " + temp_f);
            
            
            // logic for temperature
            //int intTemp_f = Convert.ToInt32(temp_f);

            //if (intTemp_f > 32)
            //{
            //    Console.WriteLine("it's not going to freeze!");
            //}

            // now write to the LCD over the serial port
            string[] weatherArray = new string[] {temperature_string,"^",feelslike};
            writeToSerial(weatherArray);
        }
        // parse Forecast
        private static void parseForecast(string input_xml)
        {
            //Variables
            string fcttext = "";
            string todayForecastHiTempF = "";
          
            var cli = new WebClient();
            string weather = cli.DownloadString(input_xml);

            using (XmlReader reader = XmlReader.Create(new StringReader(weather)))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                        {
                            // I only want to set the value for fcttext to the first occurence (which is today's forecast).
                            // Using IF to prevent updating this value again when looping and encountering element name fcttext again
                            if (fcttext == "")
                            {
                                if (reader.Name.Equals("fcttext"))
                                {
                                    reader.Read();
                                    fcttext = reader.Value;                                    
                                }
                            }
                            // I only want to set the value for the first fahrenheight element (today's forecast).
                            // Using IF to prevent updating this value again when looping and encountering element name fahrenheight again
                            else if (todayForecastHiTempF == "")
                            { 
                                if (reader.Name.Equals("fahrenheit"))                            
                                {
                                    reader.Read();
                                    todayForecastHiTempF = reader.Value;                                                                
                                } 
                            }
                            break;
                        }
                    }
                }                
            }

            //logic check on high temp
            //int intForecastHiF = Convert.ToInt32(todayForecastHiTempF);
            int intForecastHiF = 80;
            if (intForecastHiF > 0 && < 44)
            {
                Console.WriteLine("wear a WARM coat!");
            }// end > 0
            else if (intForecastHiF > 45)
            {
                Console.WriteLine("wear a Light jacket!");
            }// end > 45
            else if (intForecastHiF > 65)
            {                
                Console.WriteLine("t-shirt only OK");
            }// end >65
            else if (intForecastHiF > 72)
            {
                Console.WriteLine("wear some shorts!");
            }// end >72


            Console.WriteLine("********************");
            Console.WriteLine("fcttext:            " + fcttext);
            Console.WriteLine("high temp:          " + todayForecastHiTempF);

            // now write to the LCD over the serial port
            string[] weatherArray = new string[] {fcttext, todayForecastHiTempF};
            writeToSerial(weatherArray);
        }

        private static void parseForecastLinq(string input_xml)             
        {
            //Variables
            string fahrenheit = "";
          
            var cli = new WebClient();
            string weather = cli.DownloadString(input_xml);

            var reader = XmlReader.Create(new StringReader(weather));
            // using XElement from system.xml.linq
            XElement element = XElement.Load(reader, LoadOptions.SetBaseUri);

            IEnumerable<XElement> items = element.DescendantsAndSelf();

            foreach (var xElement in items) 
            {
                fahrenheit = GetAttributeValue("high", xElement);
            }
            
            Console.WriteLine("********************");
            Console.WriteLine("high:            " + fahrenheit);

            // now write to the LCD over the serial port
            string[] weatherArray = new string[] { fahrenheit };
            writeToSerial(weatherArray);
        }
        private static string GetElementValue(string elementName, string attributeName, XElement element)
        {
            XElement xElement = element.Element(elementName);

            string value = string.Empty;

            if (xElement != null)
            {
                XAttribute xAttribute = xElement.Attribute(attributeName);

                if (xAttribute != null)
                {
                    value = xAttribute.Value;
                }
            }

            return value;
        }
        private static string GetAttributeValue(string attributeName, XElement element)
        {
            XAttribute xAttribute = element.Attribute(attributeName);

            string value = string.Empty;
            if (xAttribute != null)
            {
                value = xAttribute.Value;
            }

            return value;
        }
        
        // write to the LCD screen using serial
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

                mySerialPort.Write(array1, 0, 1); // clear the screen
                foreach (string weatherData in weatherArgsArray)
                {                    
                    mySerialPort.WriteLine(weatherData);
                }
            }
            catch(Exception e)
            {
                //TODO: SOME ERROR HANDLING HERE
                Console.WriteLine(e.ToString());
            }
        }
    }
}
