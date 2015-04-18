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
                customerDB db = new customerDB();
                List<object> customers = db.GetAllCustomers();
                foreach (var item in customers)
                {
                    //TODO iterate each property here
                }
                
                //Customer customers = new Customer();
                //customers.SelectAllCustomers();
                //Console.WriteLine(customers.firstName);
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
