using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Project
    {
        int id;
        string name;

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }

        public override string ToString()
        {
            return id.ToString() + ": " + Name;
        }
    }
}
