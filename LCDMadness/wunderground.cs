using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using Twilio;



namespace LCDMadness
{
    class wunderground
    {
        //1 Using wunderground API to get forecast data. Parse the returned XML and assign values.
        //2 Using icon text as a short string value for conditions (sunny, cloudy, rain, etc) lookup suggested clothing based on logic in SuggestClothing method
        //3 Send an SMS message to the recipient with clothing suggestion, forecast text and weather icon
        
        //TODO: Update this method to take Zip Code as input so we can lookup the weather for each user location
        public static void GetWeather(string recipientPhone)
        {
            try {
                //Start wunderground API request
                Console.WriteLine("Starting Weather-based Clothing Suggester v1.0");
                string wunderground_key = "5f9f7844dd2b0623"; // You'll need to goto http://www.wunderground.com/weather/api/, and get a key to use the API.

                //TODO: I need to dynamically lookup the URL for the desired city using the customer's zip code. Right now, this is hard-coded: "/forecast/q/VA/Seattle.xml"
                //I need to use this: http://api.wunderground.com/api/5f9f7844dd2b0623/geolookup/q/98021.xml
                // it will return <city>Bothell</city>
                // use this to construct the parseForecast url below
                string city = parseGeolookup("http://api.wunderground.com/api/" + wunderground_key + "/geolookup/q/98021.xml");
                

                //parseConditions("http://api.wunderground.com/api/" + wunderground_key + "/conditions/q/VA/Seattle.xml");
                //string[] messageContentArray = parseForecast("http://api.wunderground.com/api/" + wunderground_key + "/forecast/q/VA/Seattle.xml");
                string[] messageContentArray = parseForecast("http://api.wunderground.com/api/" + wunderground_key + "/forecast/q/VA/" + city + ".xml");
                
                // send SMS message            
                sendSMS(recipientPhone, messageContentArray);  
            }
            catch (Exception e)
            {
                //TODO write some logging or error handling here
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
            
        }        
        
        private static string parseGeolookup(string input_xml)
        {            
            // <summary>Returns an array of city, state and country strings.
            // <para>Zip code
            // </summary>
            // info on parsing XML is here: https://msdn.microsoft.com/en-us/library/system.xml.xmlreader.read(v=vs.110).aspx
            // info on parsing XML with elements of the same name: http://stackoverflow.com/questions/13642633/using-xmlreader-class-to-parse-xml-with-elements-of-the-same-name
            // tips on using Linq and XElement are here: http://www.dotnetperls.com/xelement

            // parse Geolookup
            //Variables            
            string city = "";
            string state = "";
            string country = "";
            
            var cli = new WebClient();
            string geoLookup = cli.DownloadString(input_xml);

            using (XmlReader reader = XmlReader.Create(new StringReader(geoLookup)))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                // I only want to set the value for fcttext to the first occurence (which is today's forecast).
                                // Using IF to prevent updating this value again when looping and encountering element name fcttext again
                                if (city == "")
                                {
                                    if (reader.Name.Equals("city"))
                                    {
                                        reader.Read();
                                        city = reader.Value;
                                    }
                                }                                                          
                                break;
                            }
                    }
                }
            }

            // clothingSuggest will return the string value for suggested clothing based on the forecast high temperature
            Console.WriteLine(city + " " + state + " " + country);
            Console.ReadLine();
            // info for body text of SMS message
            //string[] geoContentArray = new string[] { city, state, country };
            //return geoContentArray;
            return city;

        }// end parse Geolookup
        public static string[] parseForecast(string input_xml)
        {
            // info on parsing XML is here: https://msdn.microsoft.com/en-us/library/system.xml.xmlreader.read(v=vs.110).aspx
            // info on parsing XML with elements of the same name: http://stackoverflow.com/questions/13642633/using-xmlreader-class-to-parse-xml-with-elements-of-the-same-name
            // tips on using Linq and XElement are here: http://www.dotnetperls.com/xelement

            // parse Forecast
            //Variables
            string fcttext = "";
            string todayForecastHiTempF = "";
            string clothingSuggest = "";
            string icon = "";
            string icon_url = "";

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
                                else if (icon_url == "")
                                {
                                    if (reader.Name.Equals("icon_url"))
                                    {
                                        reader.Read();
                                        icon_url = reader.Value;
                                    }
                                }
                                break;
                            }
                    }
                }
            }

            // clothingSuggest will return the string value for suggested clothing based on the forecast high temperature
            clothingSuggest = SuggestClothing(todayForecastHiTempF, icon, clothingSuggest);
            
            // info for body text of SMS message
            string[] messageContentArray = new string[] { clothingSuggest, fcttext, icon_url };
            return messageContentArray;
            
        }// end parse forecast
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
        private static void sendSMS(string recipientPhone, string[] weatherArgsArray)
        {
            // Find your Account Sid and Auth Token at twilio.com/user/account
            //live creds            
            string AccountSid = "ACf066e5242d04efe5a453ca2110480fad";
            string AuthToken = "e9a235555901b97a651f81cbd4c5ed0f";

            //test creds
            //string AccountSid = "AC6e8e691239a3f5f6d1377423e8c12827";
            //string AuthToken = "e033661f0066b6431462230e96904a9c";            
            
            var twilio = new TwilioRestClient(AccountSid, AuthToken);
            var sms = twilio.SendMessage("+19287234375", recipientPhone, weatherArgsArray[0] + " " + weatherArgsArray[1], new string[] { weatherArgsArray[2] });

            if (sms.RestException != null)
            {
                //an exception occurred making the REST call
                string message = sms.RestException.Message;
                Console.WriteLine(message);
            }
            
        }
    }
}
