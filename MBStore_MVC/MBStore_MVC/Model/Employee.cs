using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBStore_MVC.Model
{
    public class Employee
    {
        public int Employee_id
        { get; set; }

        public string Login_id
        { get; set; }

        public string Login_pw
        { get; set; }

        public string Name
        { get; set; }

        public string Gender
        { get; set; }

        public string Social_number
        { get; set; }

        public string Phone
        { get; set; }

        public string Adress
        { get; set; }

        public DateTime Start_date
        { get; set; }

        public DateTime End_date
        { get; set; }

        public string Rank
        { get; set; }
    }
}