using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace LCDMadness
{
    public class Person
    {
        // Fields
        public string firstName;
        public string lastName;
        public int zipCode;
        public string phoneNumber;
        string temperaturePreferanceF;
        DateTime scheduleTime;

        //Constructor that takes 3 args
        public Person(string fn, string ln, int zp, string pn, string tpf, DateTime st)
        {
            firstName = fn;
            lastName = ln;
            zipCode = zp;
            phoneNumber = pn;
            temperaturePreferanceF = tpf;
            scheduleTime = st;
        }

        // Method Connection
        private void Connect()
        { 
            SqlConnectionStringBuilder csBuilder;
            csBuilder = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            //After you have built your connection string, you can use the SQLConnection class to connect the SQL Database server:
            SqlConnection conn = new SqlConnection(csBuilder.ToString());
            conn.Open();
        }
    }
}
