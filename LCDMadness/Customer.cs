using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCDMadness
{
    public class Customer
    {
        // Field
        public string firstname;
        public string lastname;
        public int zipcode;

        // Constructor that takes multiple args
        public Customer(string fn, string ln, int zc)
        {
            firstname = fn;
            lastname = ln;
            zipcode = zc;
        }

        // Method
        public void SetName(string fn, string ln)
        {
            firstname = fn;
            lastname = ln;
        }

        private void GetUserDetails()
        { 
            
        }
    }
}
