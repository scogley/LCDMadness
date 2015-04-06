using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace LCDMadness
{
    public class Customers
    {
        // Fields
        public string firstName;
        public string lastName;
        public int zipCode;
        public string phoneNumber;
        public int temperaturePreferanceF;
        public DateTime scheduleTime;

        //Constructor that takes no args
        public Customers()
        {
            SqlConnection conn = Connect();
            conn.Open(); // open SQL connection
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;

            cmd.CommandText = "SELECT * FROM Customers";
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Connection = conn;
                        
            reader = cmd.ExecuteReader();
            
            // If DataReader has returned results begin reading
            if (reader.HasRows)
            {
                while (reader.Read())
                {                    
                    int count = reader.FieldCount;
                    int firstNameColumnOrdinal = reader.GetOrdinal("FirstName");
                    int lastNameColumnOrdinal = reader.GetOrdinal("LastName");
                    int zipCodeColumnOrdinal = reader.GetOrdinal("ZipCode");
                    int phoneNumberColumnOrdinal = reader.GetOrdinal("PhoneNumber");
                    int TemperaturePreferenceFColumnOrdinal = reader.GetOrdinal("TemperaturePreferenceF");
                    int ScheduleNotificationColumnOrdinal = reader.GetOrdinal("ScheduleNotification");
                    
                    firstName = reader.GetString(firstNameColumnOrdinal);
                    lastName = reader.GetString(lastNameColumnOrdinal);
                    zipCode = reader.GetInt32(zipCodeColumnOrdinal);
                    phoneNumber = reader.GetString(phoneNumberColumnOrdinal);
                    temperaturePreferanceF = reader.GetInt32(TemperaturePreferenceFColumnOrdinal);
                    scheduleTime = reader.GetDateTime(ScheduleNotificationColumnOrdinal);
                }
            }
            else
            {
                Console.WriteLine("no rows found");
            }
            reader.Close(); // close Reader                   
            conn.Close(); // close SQL connection
        }

        // Method SQL Connection
        private SqlConnection Connect()
        { 
            SqlConnectionStringBuilder csBuilder;
            csBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString);
            //After you have built your connection string, you can use the SQLConnection class to connect the SQL Database server:
            SqlConnection conn = new SqlConnection(csBuilder.ToString());
            //conn.Open();
            return conn;
        }
    }
}
