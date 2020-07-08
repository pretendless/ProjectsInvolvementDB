using System;
using System.Net.Http.Headers;

namespace Domain
{
    public class Person
    {
        int id;
        string firstname;
        string middlename;
        string lastname;
        int time;

        public int Id { get => id; set => id = value; }
        public string Firstname { get => firstname; set => firstname = value; }
        public string Middlename { get => middlename; set => middlename = value; }
        public string Lastname { get => lastname; set => lastname = value; }
        public int Time { get => time; set => time = value; }

        public override string ToString()
        {
            return id.ToString() + ": " + Firstname + " " + Middlename + " " + Lastname;
        }
    }

    
}
