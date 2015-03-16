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
using Twilio;



namespace LCDMadness
{
    class wunderground
    {
        //Takes a url request to wunderground, parses it, and displays the data.
        public static void GetWeather()
        {
            try {
                //Start wunderground API request
                Console.WriteLine("Starting Weather-based Clothing Suggester v1.0");
                string wunderground_key = "5f9f7844dd2b0623"; // You'll need to goto http://www.wunderground.com/weather/api/, and get a key to use the API.

                //parseConditions("http://api.wunderground.com/api/" + wunderground_key + "/conditions/q/VA/Seattle.xml");
                parseForecast("http://api.wunderground.com/api/" + wunderground_key + "/forecast/q/VA/Seattle.xml");
                //parseForecastLinq("http://api.wunderground.com/api/" + wunderground_key + "/forecast/q/VA/Seattle.xml");

                // End.
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
            
        }
        // info on parsing XML is here: https://msdn.microsoft.com/en-us/library/system.xml.xmlreader.read(v=vs.110).aspx
        // info on parsing XML with elements of the same name: http://stackoverflow.com/questions/13642633/using-xmlreader-class-to-parse-xml-with-elements-of-the-same-name
        // tips on using Linq and XElement are here: http://www.dotnetperls.com/xelement

        // parse Forecast
        private static void parseForecast(string input_xml)
        {
            //Variables
            string fcttext = "";
            string todayForecastHiTempF = "";
            string clothingSuggest = "";
            string icon = "";

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
                                else if (icon == "")
                                {
                                    if(reader.Name.Equals("icon"))
                                    {
                                        reader.Read();
                                        icon = reader.Value;
                                    }

                                }
                                break;
                            }
                    }
                }
            }
            // clothingSuggest will return the string value for suggested clothing based on the forecast high temperature
            //todayForecastHiTempF = "81";
            //icon = "sunny";
            clothingSuggest = SuggestClothing(todayForecastHiTempF, icon, clothingSuggest);

            Console.WriteLine("********************");
            Console.WriteLine("fcttext:            " + fcttext);
            Console.WriteLine("high temp:          " + todayForecastHiTempF);
            Console.WriteLine("icon text:          " + icon);
            Console.WriteLine(clothingSuggest);

            // now send an SMS message
            // hard-coding recipient numbers
            string[] recipientPhones = new string[] { "+12064455938", "+13604027250" };
            // info for body text of SMS message
            string[] weatherArray = new string[] { clothingSuggest , fcttext};
            // send SMS message
            sendSMS(recipientPhones, weatherArray);
            // write to LCD
            //writeToSerial(weatherArray);
        }// end parse forecast
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
            string[] weatherArray = new string[] { temperature_string, "^", feelslike };
            writeToSerial(weatherArray);
        }
        private static string SuggestClothing(string todayForecastHiTempF, string icon, string clothingSuggest)
        {
            //convert to an int so I can perform logic operations
            
            int intForecastHiF = Convert.ToInt32(todayForecastHiTempF);
            

            //TODO Write a switch to key off of the icon text value. The icon text value is simple string of the forecast conditions for the day.
            // example values:  "partlycloudy" or "sunny" or "rainy". This is preferred over parsing from fcctext string
            // icon info is here: http://www.wunderground.com/weather/api/d/docs?d=resources/icon-sets
            switch (icon)
            {
                #region case "chanceflurries":
                case "chanceflurries":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 40)
                        {
                            clothingSuggest = "VERY warm water proof coat and hat for snow";
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "warm jacket for snow";
                        }                       
                        break;
                    }                    
                #endregion
                #region case "chancerain":
                case "chancerain":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 50)
                        {
                            clothingSuggest = "VERY warm waterproof jacket for rain";
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "light waterproof jacket for rain";
                        }
                        else if (69 <= intForecastHiF && intForecastHiF <= 72)
                        {
                            clothingSuggest = "light waterproof jacket or umbrella";
                        }
                        else if (73 <= intForecastHiF && intForecastHiF >= 75)
                        {
                            clothingSuggest = "very light waterproof jacket or umbrella";
                        }
                        else if (intForecastHiF > 76)
                        {
                            clothingSuggest = "very light waterproof but cool jacket or umbrella";
                        }
                        break;
                    }
                    break;
                #endregion
                #region case "chancesleet":
                case "chancesleet":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 40)
                        {
                            clothingSuggest = "VERY warm water proof coat and hat for sleet";
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "light water proof jacket and hat for sleet";
                        }
                        break;
                    }                    
                #endregion
                #region case "chancesnow":
                case "chancesnow":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 40)
                        {
                            clothingSuggest = "VERY warm water proof coat and hat for snow";
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "light jacket and hat for snow";
                        }                        
                    }
                    break;
                #endregion
                #region case "chancetstorms":
                case "chancetstorms":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 50)
                        {
                            clothingSuggest = "WARM rain coat,boots";
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "Light rain jacket,boots";
                        }
                        else if (69 <= intForecastHiF && intForecastHiF <= 72)
                        {
                            clothingSuggest = "Light and cool rain jacket";
                        }
                        else if (73 <= intForecastHiF && intForecastHiF >= 75)
                        {
                            clothingSuggest = "Very light and cool rain jacket";
                        }
                        else if (intForecastHiF > 76)
                        {
                            clothingSuggest = "light rain jacket for warm weather, shorts or skirt OK";
                        }
                    }
                    break;
                #endregion
                #region case "clear":
                case "clear":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 50)
                        {
                            clothingSuggest = "VERY warm coat and hat and sunglasses";
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "wear a warm jacket and sunglasses";
                        }
                        else if (69 <= intForecastHiF && intForecastHiF <= 72)
                        {
                            clothingSuggest = "wear a long sleeve top and sunglasses";
                        }
                        else if (73 <= intForecastHiF && intForecastHiF >= 75)
                        {
                            clothingSuggest = "nice dress and sunglassses";
                        }
                        else if (intForecastHiF > 76)
                        {
                            clothingSuggest = "sunglasses shorts flip flops";
                        }
                        break;
                    }                    
                #endregion
                #region case "cloudy":
                case "cloudy":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 50)
                        {
                            clothingSuggest = "VERY warm coat and hat, no rain";
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "wear a warm jacket, no rain";
                        }
                        else if (69 <= intForecastHiF && intForecastHiF <= 72)
                        {
                            clothingSuggest = "wear a long sleeve top, no rain";
                        }
                        else if (73 <= intForecastHiF && intForecastHiF >= 75)
                        {
                            clothingSuggest = "nice dress or skirt no rain";
                        }
                        else if (intForecastHiF > 76)
                        {
                            clothingSuggest = "sunglasses for cloudy day, shorts flip flops";
                        }                        
                    }
                    break;
                #endregion
                #region case "flurries":
                case "flurries":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 40)
                        {
                            clothingSuggest = "VERY warm water proof coat and hat for snow";
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "light jacket and hat for snow";
                        }
                    }
                    break;
                #endregion
                #region case "fog":
                case "fog":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 50)
                        {
                            clothingSuggest = "VERY warm coat and hat, no rain";
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "wear a warm jacket, no rain";
                        }
                        else if (69 <= intForecastHiF && intForecastHiF <= 72)
                        {
                            clothingSuggest = "wear a long sleeve top, no rain";
                        }
                        else if (73 <= intForecastHiF && intForecastHiF >= 75)
                        {
                            clothingSuggest = "nice dress or skirt no rain";
                        }
                        else if (intForecastHiF > 76)
                        {
                            clothingSuggest = "sunglasses for cloudy day, shorts flip flops";
                        }                        
                    }
                    break;
                #endregion
                #region case "hazy":
                case "hazy":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 50)
                        {
                            clothingSuggest = "VERY warm coat and hat, no rain";
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "wear a warm jacket, no rain";
                        }
                        else if (69 <= intForecastHiF && intForecastHiF <= 72)
                        {
                            clothingSuggest = "wear a long sleeve top, no rain";
                        }
                        else if (73 <= intForecastHiF && intForecastHiF >= 75)
                        {
                            clothingSuggest = "nice dress or skirt no rain";
                        }
                        else if (intForecastHiF > 76)
                        {
                            clothingSuggest = "sunglasses for cloudy day, shorts flip flops";
                        }                        
                    }
                    break;
                #endregion
                #region case "mostlycloudy":
                case "mostlycloudy":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 50)
                        {
                            clothingSuggest = "VERY warm coat and hat, no rain";
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "wear a warm jacket, no rain";
                        }
                        else if (69 <= intForecastHiF && intForecastHiF <= 72)
                        {
                            clothingSuggest = "wear a long sleeve top, no rain";
                        }
                        else if (73 <= intForecastHiF && intForecastHiF >= 75)
                        {
                            clothingSuggest = "nice dress or skirt no rain";
                        }
                        else if (intForecastHiF > 76)
                        {
                            clothingSuggest = "sunglasses for cloudy day, shorts flip flops";
                        }                        
                    }
                    break;
                #endregion
                #region case "mostlysunny":
                case "mostlysunny":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 50)
                        {
                            clothingSuggest = "VERY warm coat, hat, sunglasses";
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "Regular jacket and sunglasses";
                        }
                        else if (69 <= intForecastHiF && intForecastHiF <= 72)
                        {
                            clothingSuggest = "Light jacket and sunglasses";
                        }
                        else if (73 <= intForecastHiF && intForecastHiF >= 75)
                        {
                            clothingSuggest = "Dress, Shirt, Shorts, sunglasses";
                        }
                        else if (intForecastHiF > 76)
                        {
                            clothingSuggest = "sunglasses shorts flip flops";
                        }
                    }
                    break;
                #endregion
                #region case "partlycloudy":
                case "partlycloudy":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 50)
                        {
                            clothingSuggest = "VERY warm coat and hat, no rain";                            
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "wear a warm jacket, no rain";
                        }
                        else if (69 <= intForecastHiF && intForecastHiF <= 72)
                        {
                            clothingSuggest = "wear a long sleeve top, no rain";                            
                        }
                        else if (73 <= intForecastHiF && intForecastHiF >= 75)
                        {
                            clothingSuggest = "nice dress and sunglassses, no rain";                            
                        }
                        else if (intForecastHiF > 76)
                        {
                            clothingSuggest = "sunglasses shorts flip flops";                            
                        }
                        break;
                    }
                #endregion
                #region case "partlysunny":
                case "partlysunny":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 50)
                        {
                            clothingSuggest = "VERY warm coat, hat, sunglasses";
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "Regular jacket and sunglasses";
                        }
                        else if (69 <= intForecastHiF && intForecastHiF <= 72)
                        {
                            clothingSuggest = "Light jacket and sunglasses";
                        }
                        else if (73 <= intForecastHiF && intForecastHiF >= 75)
                        {
                            clothingSuggest = "Dress, Shirt, Shorts, sunglasses";
                        }
                        else if (intForecastHiF > 76)
                        {
                            clothingSuggest = "sunglasses shorts flip flops";
                        }
                    }
                    break;
                #endregion
                #region case "sleet":
                case "sleet":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 40)
                        {
                            clothingSuggest = "VERY warm water proof coat and hat for sleet";
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "light jacket and hat for sleet";
                        }
                        break;
                    }                    
                #endregion
                #region case "rain":
                case "rain":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 50)
                        {
                            clothingSuggest = "WARM rain coat,boots,umbrella";                            
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "Light rain jacket,boots,umbrella";                            
                        }
                        else if (69 <= intForecastHiF && intForecastHiF <= 72)
                        {
                            clothingSuggest = "Light and cool rain jacket, boots umbrella";                            
                        }
                        else if (73 <= intForecastHiF && intForecastHiF >= 75)
                        {
                            clothingSuggest = "Very light and cool rain jacket, boots umbrella";                            
                        }
                        else if (intForecastHiF > 76)
                        {
                            clothingSuggest = "light rain jacket for warm weather, shorts or skirt OK, umbrella";                            
                        }
                    }
                    break;
                #endregion
                #region case "snow":
                case "snow":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 40)
                        {
                            clothingSuggest = "VERY warm water proof coat and hat for snow";
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "light jacket and hat for snow";
                        }
                        break;
                    }
                    
                #endregion
                #region case "sunny":
                case "sunny":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 50)
                        {
                            clothingSuggest = "VERY warm coat, hat, sunglasses";
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "Regular jacket and sunglasses";
                        }
                        else if (69 <= intForecastHiF && intForecastHiF <= 72)
                        {
                            clothingSuggest = "Light jacket and sunglasses";
                        }
                        else if (73 <= intForecastHiF && intForecastHiF >= 75)
                        {
                            clothingSuggest = "Dress, Shirt, Shorts, sunglasses";                            
                        }
                        else if (intForecastHiF > 76)
                        {
                            clothingSuggest = "sunglasses shorts flip flops";                            
                        }
                    }
                    break;
                #endregion
                #region case "tstorms":
                case "tstorms":
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 50)
                        {
                            clothingSuggest = "WARM rain coat,boots";
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "Light rain jacket,boots";
                        }
                        else if (69 <= intForecastHiF && intForecastHiF <= 72)
                        {
                            clothingSuggest = "Light and cool rain jacket, boots";
                        }
                        else if (73 <= intForecastHiF && intForecastHiF >= 75)
                        {
                            clothingSuggest = "Very light and cool rain jacket, boots";
                        }
                        else if (intForecastHiF > 76)
                        {
                            clothingSuggest = "light rain jacket for warm weather, shorts or skirt OK";
                        }
                    }
                    break;
                #endregion
                #region default:
                default:
                    {
                        if (0 <= intForecastHiF && intForecastHiF <= 50)
                        {
                            clothingSuggest = "VERY warm coat and hat";
                            Console.WriteLine(clothingSuggest);
                            return clothingSuggest;
                        }
                        else if (51 <= intForecastHiF && intForecastHiF <= 68)
                        {
                            clothingSuggest = "wear a warm jacket";
                            Console.WriteLine(clothingSuggest);
                            return clothingSuggest;

                        }
                        else if (69 <= intForecastHiF && intForecastHiF <= 72)
                        {
                            clothingSuggest = "wear a long sleeve top";
                            Console.WriteLine(clothingSuggest);
                            return clothingSuggest;
                        }
                        else if (73 <= intForecastHiF && intForecastHiF >= 75)
                        {
                            clothingSuggest = "nice dress and sunglassses";
                            Console.WriteLine(clothingSuggest);
                            return clothingSuggest;
                        }
                        else if (intForecastHiF > 76)
                        {
                            clothingSuggest = "sunglasses shorts flip flops";
                            Console.WriteLine(clothingSuggest);
                            return clothingSuggest;
                        }

                    }
                    break;
                    #endregion

                    
            }
            Console.WriteLine(clothingSuggest);
            return clothingSuggest;
        }
        
        private static void writeToSerial(string[] weatherArgsArray)
        {
            try
            {
                string serialPort = "COM3"; //com3 for home pc and laptop; com4 for devbox
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
                    //mySerialPort.Write(array1, 4, 1); // form feed                
                    //mySerialPort.Write(array1, 3, 1); // turn LCD off              
                                
                mySerialPort.Write(array1, 0, 1); // clear the screen                
                // loop endlessly updating screen with the clothing suggestion, conditions, high temp
                while (true)
                {
                    foreach (string weatherData in weatherArgsArray)
                    {
                        mySerialPort.Write(array1, 0, 1); // clear the screen
                        mySerialPort.WriteLine(weatherData);
                        mySerialPort.Write(array1, 4, 1);  // form feed
                        System.Threading.Thread.Sleep(3000);
                    }
                }
                mySerialPort.Close();
                
            }
            catch (Exception e)
            {
                //TODO: SOME ERROR HANDLING HERE
                Console.WriteLine(e.ToString());
            }
        }

        private static void sendSMS(string[] recipientPhones,string[] weatherArgsArray)
        {
            // Find your Account Sid and Auth Token at twilio.com/user/account
            //live creds            
            string AccountSid = "ACf066e5242d04efe5a453ca2110480fad";
            string AuthToken = "e9a235555901b97a651f81cbd4c5ed0f";
            
            //test creds
            //string AccountSid = "AC6e8e691239a3f5f6d1377423e8c12827";
            //string AuthToken = "e033661f0066b6431462230e96904a9c";
            foreach (string recipient in recipientPhones)
            {
                var twilio = new TwilioRestClient(AccountSid, AuthToken);                
                var sms = twilio.SendMessage("+19287234375", recipient, weatherArgsArray[0] + " " + weatherArgsArray[1], "");
                
                if (sms.RestException != null)
                {
                    //an exception occurred making the REST call
                    string message = sms.RestException.Message;
                    Console.WriteLine(message);
                }    
            }
        }
        

        //not using linq code right now
        //private static void parseForecastLinq(string input_xml)             
        //{
        //    //Variables
        //    string fahrenheit = "";
          
        //    var cli = new WebClient();
        //    string weather = cli.DownloadString(input_xml);

        //    var reader = XmlReader.Create(new StringReader(weather));
        //    // using XElement from system.xml.linq
        //    XElement element = XElement.Load(reader, LoadOptions.SetBaseUri);

        //    IEnumerable<XElement> items = element.DescendantsAndSelf();

        //    foreach (var xElement in items) 
        //    {
        //        fahrenheit = GetAttributeValue("high", xElement);
        //    }
            
        //    Console.WriteLine("********************");
        //    Console.WriteLine("high:            " + fahrenheit);

        //    // now write to the LCD over the serial port
        //    string[] weatherArray = new string[] { fahrenheit };
        //    writeToSerial(weatherArray);
        //}
        //private static string GetElementValue(string elementName, string attributeName, XElement element)
        //{
        //    XElement xElement = element.Element(elementName);

        //    string value = string.Empty;

        //    if (xElement != null)
        //    {
        //        XAttribute xAttribute = xElement.Attribute(attributeName);

        //        if (xAttribute != null)
        //        {
        //            value = xAttribute.Value;
        //        }
        //    }

        //    return value;
        //}
        //private static string GetAttributeValue(string attributeName, XElement element)
        //{
        //    XAttribute xAttribute = element.Attribute(attributeName);

        //    string value = string.Empty;
        //    if (xAttribute != null)
        //    {
        //        value = xAttribute.Value;
        //    }

        //    return value;
        //}
      
    }
}
