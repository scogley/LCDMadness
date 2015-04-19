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
            
            try
            {
                MyDataBase db = new MyDataBase();
                // call the GetAllCustomers method and store the customer objects in a List
                List<Person> customers = db.GetAllCustomers();                
                if (customers != null)
                {
                    foreach (Person person in customers)
                    {
                        //iterate each person object field
                        Console.WriteLine(person.firstName);
                        Console.WriteLine(person.lastName);
                        Console.WriteLine(person.phoneNumber);
                        Console.WriteLine(person.temperaturePreferanceF);
                        Console.WriteLine(person.zipCode);
                        Console.WriteLine(person.scheduleTime);

                        // send a text to the user
                        wunderground.GetWeather(person.phoneNumber);
                    }
                }
                Console.ReadKey(); // DEBUGGING: to keep the console window open 
                // get the weather forecast and send what to wear suggestion to customer
                //wunderground.GetWeather();
            }
            catch(Exception e) 
            {
                Console.WriteLine(e.ToString());
                //TODO: SOME ERROR HANDLING HERE
            }            
        }

       
    }
}
